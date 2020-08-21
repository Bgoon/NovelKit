using GKitForUnity;
using System;

namespace TaleKit.Datas.ModelEditor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorAttribute : Attribute {
		public string valueName;
		public ValueEditorLayout layout;
		public string visibleCondition;
		public bool isStatic;

		public ValueEditorAttribute(string valueName, ValueEditorLayout layout = ValueEditorLayout.Normal, string visibleCondition = null, bool isStatic = false) {
			this.valueName = valueName;
			this.layout = layout;
			this.visibleCondition = visibleCondition;
			this.isStatic = isStatic;
		}
	}
}
