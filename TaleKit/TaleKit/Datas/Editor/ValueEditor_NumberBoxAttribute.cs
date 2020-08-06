using GKitForUnity;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_NumberBoxAttribute : ValueEditorAttribute {
		public NumberType numberType;
		public float minValue;
		public float maxValue;

		public ValueEditor_NumberBoxAttribute(string valueName, NumberType numberType = NumberType.Float, float minValue = float.NegativeInfinity, 
			float maxValue = float.PositiveInfinity, string visibleCondition = null) : base(valueName, visibleCondition:visibleCondition) {
			this.numberType = numberType;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
