using GKitForUnity;
using TaleKit.Datas.Asset;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_AssetSelectorAttribute : ValueEditorAttribute {
		public AssetType assetType;

		public ValueEditor_AssetSelectorAttribute(string valueName, AssetType assetType, string visibleCondition = null) : base(valueName, visibleCondition:visibleCondition) {
			this.assetType = assetType;
		}
	}
}
