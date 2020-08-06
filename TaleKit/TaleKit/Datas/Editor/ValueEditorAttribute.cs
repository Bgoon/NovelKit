using GKitForUnity;
using System;

namespace TaleKit.Datas.Editor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorAttribute : Attribute {
		public string valueName;
		public ValueEditorLayout layout;
		public string visibleCondition;

		public ValueEditorAttribute(string valueName, ValueEditorLayout layout = ValueEditorLayout.Normal, string visibleCondition = null) {
			this.valueName = valueName;
			this.layout = layout;
			this.visibleCondition = visibleCondition;
		}
	}
}
