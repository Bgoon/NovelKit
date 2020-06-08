using GKit.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Asset;

namespace TaleKit.Datas.Resource {
	public class AssetItem {
		public readonly AssetManager OwnerAssetManager;
		public TaleData OwnerTaleData => OwnerAssetManager.OwnerTaleData;

		public bool HasKey => !string.IsNullOrEmpty(key);

		public string AssetRelPath {
			get; private set;
		}
		public string AssetMetaFilename => Path.Combine(OwnerTaleData.AssetMetaDir, AssetRelPath);


		[AssetMeta]
		public string key;

		public AssetItem(AssetManager ownerAssetManager, string path) {
			OwnerAssetManager = ownerAssetManager;

			this.AssetRelPath = path;
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
		public void Delete() {
			File.Delete(AssetMetaFilename);
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
