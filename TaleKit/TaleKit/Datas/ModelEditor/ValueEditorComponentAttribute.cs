using System;

namespace TaleKit.Datas.ModelEditor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorComponentAttribute : Attribute {
		public string visibleCondition;

		public ValueEditorComponentAttribute() {
		}
	}
}
