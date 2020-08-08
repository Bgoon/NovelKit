using System;

namespace TaleKit.Datas.Editor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorComponentAttribute : Attribute {
		public string visibleCondition;

		public ValueEditorComponentAttribute() {
		}
	}
}
