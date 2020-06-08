using GKit;
using System;
using System.Collections.Generic;
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
		public TaleData OwnerTaleData {
			get; private set;
		}

		private string AssetDir => OwnerTaleData.AssetDir;
		private string AssetMetaDir => OwnerTaleData.AssetMetaDir;


		public List<AssetItem> assetList;
		public Dictionary<string, AssetItem> pathToAssetDict;
		public Dictionary<string, AssetItem> keyToAssetDict;

		private FileSystemWatcher assetDirWatcher;
		
		public AssetManager(TaleData ownerTaleData) {
			OwnerTaleData = ownerTaleData;

			assetList = new List<AssetItem>();
			pathToAssetDict = new Dictionary<string, AssetItem>();
			keyToAssetDict = new Dictionary<string, AssetItem>();
		}

		private void AssetDirWatcher_Created(object sender, FileSystemEventArgs e) {
			if (IOUtility.IsDirectory(e.FullPath)) {
				//Directory

			} else {
				//File
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);

				if (pathToAssetDict.ContainsKey(assetRelPath))
					return;

				LoadOrCreateMeta(assetRelPath);
			}
		}
		private void AssetDirWatcher_Deleted(object sender, FileSystemEventArgs e) {
			if (IOUtility.IsDirectory(e.FullPath)) {
				//Directory

			} else {
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);

				DeleteMeta(assetRelPath);
			}
		}
		private void AssetDirWatcher_Renamed(object sender, RenamedEventArgs e) {
			if (IOUtility.IsDirectory(e.FullPath)) {
				//Directory

			} else {
				string assetOldRelPath = IOUtility.GetRelativePath(AssetDir, e.OldFullPath);
				string assetNewRelPath = IOUtility.GetRelativePath(AssetDir, e.FullPath);

				//TODO : NewPath가 AssetDir 안쪽이면 Meta Rename,
				//밖이면 Delete
			}
		}

		public void ReloadAssets() {
			ClearCollections();

			string[] assetFiles = Directory.GetFiles(AssetDir, "*.*", SearchOption.AllDirectories);

			for(int fileI=0; fileI<assetFiles.Length; ++fileI) {
				string assetRelPath = IOUtility.GetRelativePath(OwnerTaleData.AssetDir, assetFiles[fileI]);

				LoadOrCreateMeta(assetRelPath);
			}

			//TODO : 누락된 메타파일 / 빈 폴더 정리작업 및 출력하기
		}

		private void ClearCollections() {
			assetList.Clear();
			pathToAssetDict.Clear();
			keyToAssetDict.Clear();
		}

		public void StartWatchAssetDir() {
			assetDirWatcher = new FileSystemWatcher(AssetDir);

			assetDirWatcher.Created += AssetDirWatcher_Created;
			assetDirWatcher.Deleted += AssetDirWatcher_Deleted;
			assetDirWatcher.Renamed += AssetDirWatcher_Renamed;

			assetDirWatcher.EnableRaisingEvents = true;
		}
		public void StopWatchAssetDir() {
			if (assetDirWatcher == null)
				return;

			assetDirWatcher.Dispose();
		}

		private AssetItem LoadOrCreateMeta(string assetRelPath) {
			AssetItem item = new AssetItem(this, assetRelPath);
			pathToAssetDict.Add(assetRelPath, item);
			assetList.Add(item);

			if(!item.LoadMeta()) {
				item.SaveMeta();
			}

			return item;
		}
		private void DeleteMeta(string assetRelPath) {
			if(pathToAssetDict.ContainsKey(assetRelPath)) {
				DeleteMeta(pathToAssetDict[assetRelPath]);
			}
		}
		private void DeleteMeta(AssetItem item) {
			if(pathToAssetDict.ContainsKey(item.AssetRelPath)) {
				pathToAssetDict.Remove(item.AssetRelPath);
			}
			if(item.HasKey && keyToAssetDict.ContainsKey(item.key)) {
				keyToAssetDict.Remove(item.key);
			}
			item.Delete();
		}
	}
}
