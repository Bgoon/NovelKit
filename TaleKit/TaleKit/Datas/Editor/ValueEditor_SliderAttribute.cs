namespace TaleKit.Datas.Editor {
	public class ValueEditor_SliderAttribute : ValueEditorAttribute {
		public NumberType numberType;
		public float minValue;
		public float maxValue;

		public ValueEditor_SliderAttribute(string valueName, NumberType numberType = NumberType.Float, float minValue = 0f, float maxValue = 1f) : base(valueName) {
			this.numberType = numberType;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
