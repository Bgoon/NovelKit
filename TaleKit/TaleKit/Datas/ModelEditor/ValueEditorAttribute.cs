using GKitForUnity;
using System;

namespace TaleKit.Datas.ModelEditor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorAttribute : SavableFieldAttribute {
		public string valueName;
		public ValueEditorLayout layout;
		public string visibleCondition;
		public bool noKey;

		public ValueEditorAttribute(string valueName, ValueEditorLayout layout = ValueEditorLayout.Normal, string visibleCondition = null, bool noKey = false) {
			this.valueName = valueName;
			this.layout = layout;
			this.visibleCondition = visibleCondition;
			this.noKey = noKey;
		}
	}
}
