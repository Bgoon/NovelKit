using GKitForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaleKit.Datas.Asset;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Resource;
using UColor = UnityEngine.Color;

namespace TaleKit.Datas.UI.UiItem {
	[Serializable]
	public class UiPanel : UiItemBase {
		[ValueEditorComponent_Header("Panel Attributes")]
		[ValueEditor_ColorBox("Color")]
		public UColor color = UColor.white;

		[ValueEditor_AssetSelector("Image", AssetType.Image)]
		public string imageAssetKey;

		[ValueEditor_Switch("Use NinePatch")]
		public bool useNinePatch;
		public bool UseNinePatch => useNinePatch;
		[ValueEditor_Margin("NinePatch Side Aspect", 0, 1, 0.1f, visibleCondition = nameof(UseNinePatch))]
		public GRect ninePatchSideAspect;

		public UiPanel(UiFile ownerFile) : base(ownerFile, UiItemType.Panel) {

		}

		public AssetItem GetImageAsset() {
			return AssetManager.GetAsset(imageAssetKey);
		}
	}
}
