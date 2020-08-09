using GKitForUnity;

namespace TaleKit.Datas.ModelEditor {
	public class ValueEditor_TextBoxAttribute : ValueEditorAttribute {

		public int maxLength;
		public bool allowMultiline;

		public ValueEditor_TextBoxAttribute(string valueName, int maxLength = 100, bool allowMultiline = false) : base(valueName) {
			this.maxLength = maxLength;
			this.allowMultiline = allowMultiline;
		}
	}
}
