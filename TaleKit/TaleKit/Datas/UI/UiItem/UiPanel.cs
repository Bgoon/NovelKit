using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Asset;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Resource;
using UColor = UnityEngine.Color;

namespace TaleKit.Datas.UI.UiItem {
	public class UiPanel : UiItemBase {
		[ValueEditorComponent_Header("Panel Attributes")]
		[ValueEditor_ColorBox("Color")]
		public UColor color = UColor.white;

		[ValueEditor_AssetSelector("Image", AssetType.Image)]
		public string imageAssetKey;

		public UiPanel(UiFile ownerFile) : base(ownerFile, UiItemType.Panel) {

		}

		public AssetItem GetImageAsset() {
			return AssetManager.GetAsset(imageAssetKey);
		}
	}
}
