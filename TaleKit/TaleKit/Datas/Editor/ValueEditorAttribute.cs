using GKitForUnity;
using System;

namespace TaleKit.Datas.Editor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorAttribute : Attribute {
		public string valueName;
		public ValueEditorLayout layout;
		public ReturnDelegate<bool> displayCondition;

		public ValueEditorAttribute(string valueName, ValueEditorLayout layout = ValueEditorLayout.Normal, ReturnDelegate<bool> displayCondition = null) {
			this.valueName = valueName;
			this.layout = layout;
			this.displayCondition = displayCondition;
		}
	}
}
