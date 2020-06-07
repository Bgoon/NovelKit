using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Asset;

namespace TaleKit.Datas.Resource {
	public class AssetItem {
		public readonly AssetManager OwnerAssetManager;

		public string path;

		[AssetMeta]
		public string key;

		public AssetItem(AssetManager ownerAssetManager, string path) {
			OwnerAssetManager = ownerAssetManager;

			this.path = path;
		}
	}
}
