using GKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit;
using TaleKit.Datas;
using TaleKit.Datas.Resource;

namespace TaleKit.Datas.Asset {
	//IO작업실패시 기본적으로 3초간격으로 3회 재시도
	public class AssetManager {
		private const int DeletedItemHoldMillisec = 500;

		public TaleData OwnerTaleData {
			get; private set;
		}

		private string AssetDir => OwnerTaleData.AssetDir;
		private string AssetMetaDir => OwnerTaleData.AssetMetaDir;
		private string NormalizedAssetDir => IOUtility.NormalizePath(AssetDir);


		public List<AssetItem> assetList;
		public Dictionary<string, AssetItem> pathToAssetDict;
		public Dictionary<string, AssetItem> nameKeyToAssetDict;

		private FileSystemWatcher assetDirWatcher;

		/// <summary>
		/// 파일 이동을 감지하기 위해 삭제된 에셋 메타데이터를 {DeletedItemHoldMillisec} 만큼 보관한다.
		/// </summary>
		private Dictionary<string, AssetItem> recentlyDeletedItemDict;
		
		public AssetManager(TaleData ownerTaleData) {
			OwnerTaleData = ownerTaleData;

			assetList = new List<AssetItem>();
			pathToAssetDict = new Dictionary<string, AssetItem>();
			nameKeyToAssetDict = new Dictionary<string, AssetItem>();
			recentlyDeletedItemDict = new Dictionary<string, AssetItem>();
		}

		private void AssetDirWatcher_Created(object sender, FileSystemEventArgs e) {
			Debug.WriteLine("Created");
			if (IOUtility.IsDirectory(e.FullPath)) {
				//Directory
				LoadOrCreateMetas(e.FullPath);
			} else {
				//File
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);

				if (pathToAssetDict.ContainsKey(assetRelPath))
					return;

				LoadOrCreateMeta(assetRelPath);
			}
		}
		private void AssetDirWatcher_Deleted(object sender, FileSystemEventArgs e) {
			Debug.WriteLine("Deleted");
			string relPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);
			string metaPath = Path.Combine(AssetMetaDir, relPath);

			if (Directory.Exists(metaPath)) {
				//Directory
				DeleteMetas(e.FullPath, true);
			} else if(File.Exists(metaPath)) {
				//File
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);

				DeleteMeta(assetRelPath, true);
			}
		}
		private void AssetDirWatcher_Renamed(object sender, RenamedEventArgs e) {
			Debug.WriteLine("Renamed");
			string oldAssetRelPath = IOUtility.GetRelativePath(AssetDir, e.OldFullPath);
			string newAssetRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);

			bool oldPathInAssetDir = IOUtility.NormalizePath(e.OldFullPath).StartsWith(NormalizedAssetDir);
			bool newPathInAssetDir = IOUtility.NormalizePath(e.FullPath).StartsWith(NormalizedAssetDir);

			if (IOUtility.IsDirectory(e.FullPath)) {
				//Directory
				if (oldPathInAssetDir) {
					if (newPathInAssetDir) {
						RenameMetas(e.OldFullPath, e.FullPath);
					} else {
						DeleteMetas(e.OldFullPath);
					}
				} else {
					if (newPathInAssetDir) {
						LoadOrCreateMetas(e.FullPath);
					} else {
					}
				}
			} else {
				//File
				if(oldPathInAssetDir) {
					if(newPathInAssetDir) {
						RenameMeta(oldAssetRelPath, newAssetRelPath);
					} else {
						DeleteMeta(oldAssetRelPath);
					}
				} else {
					if (newPathInAssetDir) {
						LoadOrCreateMeta(newAssetRelPath);
					} else {
					}
				}
			}
		}
		private void AssetDirWatcher_Changed(object sender, FileSystemEventArgs e) {
			if (!File.Exists(e.FullPath))
				return;

			string assetRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);
			if (pathToAssetDict.ContainsKey(assetRelPath)) {
				AssetItem item = pathToAssetDict[assetRelPath];

				item.UpdateFileHash();
				Debug.WriteLine($"HashUpdated");
			}
		}

		private void ClearCollections() {
			assetList.Clear();
			pathToAssetDict.Clear();
			nameKeyToAssetDict.Clear();
			recentlyDeletedItemDict.Clear();
		}

		//Watch
		public void StartWatchAssetDir() {
			assetDirWatcher = new FileSystemWatcher(AssetDir);
			assetDirWatcher.IncludeSubdirectories = true;
			assetDirWatcher.NotifyFilter =
				NotifyFilters.FileName | NotifyFilters.DirectoryName |
				NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size;

			assetDirWatcher.Created += AssetDirWatcher_Created;
			assetDirWatcher.Deleted += AssetDirWatcher_Deleted;
			assetDirWatcher.Renamed += AssetDirWatcher_Renamed;
			assetDirWatcher.Changed += AssetDirWatcher_Changed;

			assetDirWatcher.EnableRaisingEvents = true;
		}
		public void StopWatchAssetDir() {
			if (assetDirWatcher == null)
				return;

			assetDirWatcher.Dispose();
		}

		//Save
		public void SaveMetas() {
			foreach(AssetItem item in assetList) {
				item.SaveMeta();
			}
		}

		//Manage meta
		public void ReloadMetas() {
			ClearCollections();

			string[] assetFiles = Directory.GetFiles(AssetDir, "*", SearchOption.AllDirectories);

			for(int fileI=0; fileI<assetFiles.Length; ++fileI) {
				string assetRelPath = IOUtility.GetRelativePath(OwnerTaleData.AssetDir, assetFiles[fileI]);

				LoadOrCreateMeta(assetRelPath);
			}

			Cleanup();
		}
		private void LoadOrCreateMetas(string directory) {
			string[] filenames = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

			foreach(string filename in filenames) {
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, filename);

				LoadOrCreateMeta(assetRelPath);
			}
		}
		public AssetItem LoadOrCreateMeta(string assetRelPath) {
			string filename = Path.Combine(AssetDir, assetRelPath);
			if (!File.Exists(filename))
				return null;

			//Check deleted item recently
			string fileHash = AssetItem.GetFileHash(filename);

			AssetItem item;
			if(recentlyDeletedItemDict.ContainsKey(fileHash)) {
				item = recentlyDeletedItemDict[fileHash];
				RecycleAsset(item);

				RenameMeta(item, assetRelPath);

				item.SaveMeta();
				Debug.WriteLine("Recycled");
			} else {
				item = new AssetItem(this, assetRelPath);
				pathToAssetDict.Add(assetRelPath, item);
				assetList.Add(item);

				if(!item.LoadMeta()) {
					item.SaveMeta();
				}
			}

			return item;
		}
		private void DeleteMetas(string directory, bool detectRenamed = false) {
			string relDirName = IOUtility.NormalizePath(IOUtility.GetRelativePath(AssetDir, directory));

			foreach (AssetItem asset in assetList) {
				if(IOUtility.NormalizePath(asset.AssetRelPath).StartsWith(relDirName)) {
					DeleteMeta(asset);
				}
			}
		}
		private void DeleteMeta(string assetRelPath, bool detectRenamed = false) {
			if(pathToAssetDict.ContainsKey(assetRelPath)) {
				DeleteMeta(pathToAssetDict[assetRelPath], detectRenamed);
			}
		}
		private void DeleteMeta(AssetItem item, bool detectRenamed = false) {
			if(pathToAssetDict.ContainsKey(item.AssetRelPath)) {
				pathToAssetDict.Remove(item.AssetRelPath);

				if(detectRenamed) {
					DeleteAssetWithDetectRenameAsync(item);
				} else {
					if(item.HasKey && nameKeyToAssetDict.ContainsKey(item.nameKey)) {
						nameKeyToAssetDict.Remove(item.nameKey);
					}
				}
			}
			item.DeleteMeta();
		}
		private void RenameMetas(string oldAssetDirectory, string newAssetDirectory) {
			oldAssetDirectory = IOUtility.NormalizePath(oldAssetDirectory);

			string[] newAssetFilenames = Directory.GetFiles(newAssetDirectory, "*", SearchOption.AllDirectories);
			foreach (string newAssetFilename in newAssetFilenames) {

				string oldFilename = newAssetFilename.ToLower().Replace(newAssetDirectory.ToLower(), oldAssetDirectory.ToLower());

				string oldRelAssetPath = IOUtility.GetRelativePath(AssetDir, oldFilename);
				string newRelAssetPath = IOUtility.GetRelativePath(AssetDir, newAssetFilename);

				Directory.CreateDirectory(Path.GetDirectoryName(newAssetFilename));
				string dstFilename = Path.Combine(AssetMetaDir, newRelAssetPath);

				string newMetaFilename = Path.Combine(AssetMetaDir, newRelAssetPath);
				Directory.CreateDirectory(Path.GetDirectoryName(newMetaFilename));
				File.Move(Path.Combine(AssetMetaDir, oldRelAssetPath), Path.Combine(AssetMetaDir, newRelAssetPath));
			}
		}
		private void RenameMeta(string oldAssetRelPath, string newAssetRelPath) {
			if(pathToAssetDict.ContainsKey(oldAssetRelPath)) {
				RenameMeta(pathToAssetDict[oldAssetRelPath], newAssetRelPath);
			} else {
				LoadOrCreateMeta(newAssetRelPath);
			}
		}
		private void RenameMeta(AssetItem item, string newAssetRelPath) {
			pathToAssetDict.Remove(item.AssetRelPath);

			item.RenameMeta(newAssetRelPath);

			pathToAssetDict.Add(newAssetRelPath, item);
		}


		/// <summary>
		/// 누락된 메타파일 / 빈 폴더 정리작업
		/// </summary>
		private void Cleanup() {
			CleanupUnusedMetaFiles();
			CleanupEmptyMetaDirectories();
		}
		private void CleanupEmptyMetaDirectories() {
			CleanupEmptyMetaDirectories(AssetMetaDir);
		}
		private void CleanupEmptyMetaDirectories(string directory) {
			string[] dirNames = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

			foreach(string dirName in dirNames) {
				CleanupEmptyMetaDirectories(dirName);
			}

			if (Directory.GetDirectories(directory).Length + Directory.GetFiles(directory).Length == 0) {
				Directory.Delete(directory);
			}
		}
		private void CleanupUnusedMetaFiles() {
			string[] assetMetaFiles = Directory.GetFiles(AssetMetaDir, "*", SearchOption.AllDirectories);

			foreach(string assetMetaFile in assetMetaFiles) {
				string assetRelPath = IOUtility.GetRelativePath(AssetMetaDir, assetMetaFile);

				if(!pathToAssetDict.ContainsKey(assetRelPath)) {
					File.Delete(assetMetaFile);
				}
			}
		}

		private async void DeleteAssetWithDetectRenameAsync(AssetItem item) {
			item.SetFileHashLock(true);
			recentlyDeletedItemDict.Add(item.fileHash, item);

			await Task.Delay(DeletedItemHoldMillisec);

			if(recentlyDeletedItemDict.ContainsKey(item.fileHash)) {
				recentlyDeletedItemDict.Remove(item.fileHash);
				
				DeleteMeta(item);
				Debug.WriteLine("DeletedAsync");
				
				item.SetFileHashLock(false);
			}
		}
		private void RecycleAsset(AssetItem item) {
			recentlyDeletedItemDict.Remove(item.fileHash);

			item.SetFileHashLock(false);
		}
	}
}
