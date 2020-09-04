using GKitForUnity;

namespace TaleKit.Datas.ModelEditor {
	public class ValueEditor_MarginAttribute : ValueEditorAttribute {
		public float minValue;
		public float maxValue;
		public float dragAdjustFactor;

		public ValueEditor_MarginAttribute(string valueName, float minValue = 0, float maxValue = float.PositiveInfinity, float dragAdjustFactor = 1f) : base(valueName, ValueEditorLayout.Wide) {
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.dragAdjustFactor = dragAdjustFactor;
		}
	}
}
