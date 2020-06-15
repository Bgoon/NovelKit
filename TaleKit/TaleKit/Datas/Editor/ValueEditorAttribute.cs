using System;

namespace TaleKit.Datas.Editor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorAttribute : Attribute {

		public string valueName;
		public ValueEditorLayout layout;

		public ValueEditorAttribute(string valueName, ValueEditorLayout layout = ValueEditorLayout.Normal) {
			this.valueName = valueName;
			this.layout = layout;
		}
	}
}
