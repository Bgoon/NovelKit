using GKit.Json;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using TaleKit.Datas.Asset;
using TaleKit.Datas.ModelEditor;

namespace TaleKit.Datas.Resource {
	public class AssetItem : EditableModel {
		public readonly AssetManager OwnerAssetManager;
		public TaleData OwnerTaleData => OwnerAssetManager.OwnerTaleData;

		public bool HasKey => !string.IsNullOrEmpty(Key);

		public string AssetRelPath {
			get; private set;
		}
		public string AssetFilename => Path.Combine(OwnerTaleData.AssetDir, AssetRelPath);
		public string AssetMetaFilename => Path.Combine(OwnerTaleData.AssetMetaDir, AssetRelPath);

		public bool FileHashLocked {
			get; private set;
		}

		public AssetType Type {
			get {
				if (string.IsNullOrEmpty(AssetRelPath))
					return AssetType.Unknown;

				return GetAssetType(Path.GetExtension(AssetFilename));
			}
		}

		[AssetMeta]
		public string fileHash;

		[AssetMeta]
		[ValueEditorComponent_Header("메타데이터")]
		[ValueEditor_TextBox("NameKey")]
		public string Key;

		[ValueEditorComponent_Header("미리보기")]
		[ValueEditorComponent_FilePreview]
		private string previewImageSource;

		public static string GetFileHash(string filename) {
			return IOUtility.GetMetadataHash(filename);
		}
		public static AssetType GetAssetType(string ext) {
			ext = ext.Replace(".", "").ToLower();

			switch(ext) {
				case "bmp":
				case "jpg":
				case "jpeg":
				case "gif":
				case "tiff":
				case "png":
					return AssetType.Image;
				case "txt":
				case "md":
				case "log":
				case "script":
				case "html":
				case "css":
				case "xml":
				case "json":
				case "bson":
					return AssetType.Text;
				default:
					return AssetType.Unknown;
			}
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

		public void UpdatePreviewImageSource() {
			previewImageSource = AssetFilename;
		}

		public JObject ToJObject() {
			JObject jAssetMeta = new JObject();

			JObject jFields = new JObject();
			jAssetMeta.Add("Fields", jFields);
			jFields.AddAttrFields<AssetMetaAttribute>(this);

			return jAssetMeta;
		}
	}
}
