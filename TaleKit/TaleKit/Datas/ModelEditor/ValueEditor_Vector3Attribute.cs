using GKitForUnity;

namespace TaleKit.Datas.ModelEditor {
	public class ValueEditor_Vector3Attribute : ValueEditorAttribute {
		public NumberType numberType;
		public float minValue;
		public float maxValue;

		public ValueEditor_Vector3Attribute(string valueName, NumberType numberType = NumberType.Float, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : base(valueName) {
			this.numberType = numberType;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
