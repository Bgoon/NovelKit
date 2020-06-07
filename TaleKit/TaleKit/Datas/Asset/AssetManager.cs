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
	public class AssetManager {
		public TaleData OwnerTaleData {
			get; private set;
		}

		private string AssetDir => OwnerTaleData.AssetDir;
		private string AssetMetaDir => OwnerTaleData.AssetMetaDir;


		public List<AssetItem> assetList;
		public Dictionary<string, AssetItem> pathToAssetDict;
		public Dictionary<string, AssetItem> pathToAssetMetaDict;
		public Dictionary<string, AssetItem> keyToAssetDict;
		
		public AssetManager(TaleData ownerTaleData) {
			OwnerTaleData = ownerTaleData;

			assetList = new List<AssetItem>();
			pathToAssetDict = new Dictionary<string, AssetItem>();
			pathToAssetMetaDict = new Dictionary<string, AssetItem>();
			keyToAssetDict = new Dictionary<string, AssetItem>();
		}

		public void ReloadAssets() {
			string[] assetFiles = Directory.GetFiles(AssetDir, "*.*", SearchOption.AllDirectories);

			for(int fileI=0; fileI<assetFiles.Length; ++fileI) {
				string assetRelPath = IOUtility.GetRelativePath(OwnerTaleData.AssetDir, assetFiles[fileI]);

				AssetItem item = new AssetItem(this, assetRelPath);
				pathToAssetDict.Add(assetRelPath, item);
				assetList.Add(item);

				if(item.IsMetaExists()) {
					item.LoadMeta();
				} else {
					item.SaveMeta();
				}
			}

			//TODO : 누락된 메타파일 / 빈 폴더 정리작업 및 출력하기
		}
		private void ClearCollections() {
			assetList.Clear();
			pathToAssetDict.Clear();
			pathToAssetMetaDict.Clear();
			keyToAssetDict.Clear();
		}

	}
}
