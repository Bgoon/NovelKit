using GKitForUnity;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_MarginAttribute : ValueEditorAttribute {

		public ValueEditor_MarginAttribute(string valueName, string visibleCondition = null) : base(valueName, ValueEditorLayout.Wide, visibleCondition:visibleCondition) {

		}
	}
}
