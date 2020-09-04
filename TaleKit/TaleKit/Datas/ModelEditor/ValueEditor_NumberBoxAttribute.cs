using GKitForUnity;

namespace TaleKit.Datas.ModelEditor {
	public class ValueEditor_NumberBoxAttribute : ValueEditorAttribute {
		public NumberType numberType;
		public float minValue;
		public float maxValue;
		public float dragAdjustFactor;

		public ValueEditor_NumberBoxAttribute(string valueName, NumberType numberType = NumberType.Float, float minValue = float.NegativeInfinity, 
			float maxValue = float.PositiveInfinity, float dragAdjustFactor = 1f) : base(valueName) {
			this.numberType = numberType;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.dragAdjustFactor = dragAdjustFactor;
		}
	}
}
