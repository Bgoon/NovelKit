using GKit.Json;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using TaleKit.Datas.Asset;

namespace TaleKit.Datas.Resource {
	public class AssetItem {
		public readonly AssetManager OwnerAssetManager;
		public TaleData OwnerTaleData => OwnerAssetManager.OwnerTaleData;

		public bool HasKey => !string.IsNullOrEmpty(nameKey);

		public string AssetRelPath {
			get; private set;
		}
		public string AssetFilename => Path.Combine(OwnerTaleData.AssetDir, AssetRelPath);
		public string AssetMetaFilename => Path.Combine(OwnerTaleData.AssetMetaDir, AssetRelPath);

		public bool FileHashLocked {
			get; private set;
		}

		[AssetMeta]
		public string nameKey;
		[AssetMeta]
		public string fileHash;

		public static string GetFileHash(string filename) {
			return IOUtility.GetMetadataHash(filename);
		}
		public AssetItem(AssetManager ownerAssetManager, string path) {
			OwnerAssetManager = ownerAssetManager;

			this.AssetRelPath = path;

			UpdateFileHash();
		}

		public bool IsMetaExists() {
			return File.Exists(AssetMetaFilename);
		}

		public bool LoadMeta() {
			if (!IsMetaExists())
				return false;

			string jsonString = File.ReadAllText(AssetMetaFilename, Encoding.UTF8);
			this.LoadAttrFields<AssetMetaAttribute>(JObject.Parse(jsonString));

			return true;
		}
		public void SaveMeta() {
			Directory.CreateDirectory(Path.GetDirectoryName(AssetMetaFilename));
			File.WriteAllText(AssetMetaFilename, ToJObject().ToString(), Encoding.UTF8);
		}
		public void DeleteMeta() {
			File.Delete(AssetMetaFilename);
		}
		public void RenameMeta(string assetRelPath) {
			if (this.AssetRelPath == assetRelPath)
				return;

			string oldMetaFilename = AssetMetaFilename;
			AssetRelPath = assetRelPath;

			if (File.Exists(oldMetaFilename)) {
				File.Move(oldMetaFilename, AssetMetaFilename);
			}
		}

		public void SetFileHashLock(bool useLock) {
			FileHashLocked = useLock;
		}
		public void UpdateFileHash() {
			if (FileHashLocked)
				return;

			fileHash = GetFileHash(AssetFilename);
		}


		public JObject ToJObject() {
			JObject jAssetMeta = new JObject();

			JObject jAttributes = new JObject();
			jAssetMeta.Add("Attributes", jAttributes);
			jAttributes.AddAttrFields<AssetMetaAttribute>(this);

			return jAssetMeta;
		}
	}
}
