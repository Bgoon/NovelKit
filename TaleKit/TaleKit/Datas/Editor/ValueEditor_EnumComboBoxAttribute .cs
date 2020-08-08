using GKitForUnity;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_EnumComboBoxAttribute : ValueEditorAttribute {

		public ValueEditor_EnumComboBoxAttribute(string valueName, string visibleCondition = null) : base(valueName, visibleCondition:visibleCondition) {
		}
	}
}
