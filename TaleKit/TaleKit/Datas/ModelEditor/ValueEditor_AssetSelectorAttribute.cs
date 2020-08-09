using GKitForUnity;
using TaleKit.Datas.Asset;

namespace TaleKit.Datas.ModelEditor {
	public class ValueEditor_AssetSelectorAttribute : ValueEditorAttribute {
		public AssetType assetType;

		public ValueEditor_AssetSelectorAttribute(string valueName, AssetType assetType) : base(valueName) {
			this.assetType = assetType;
		}
	}
}
