using GKitForUnity;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_AnchorPresetAttribute : ValueEditorAttribute {

		public ValueEditor_AnchorPresetAttribute(string valueName, string visibleCondition = null) : base(valueName, ValueEditorLayout.Wide, visibleCondition) {
		}
	}
}
