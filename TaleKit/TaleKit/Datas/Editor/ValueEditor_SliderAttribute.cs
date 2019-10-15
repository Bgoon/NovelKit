using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_SliderAttribute : ValueEditorAttribute {
		public NumberType numberType;
		public float defaultValue;
		public float minValue;
		public float maxValue;

		public ValueEditor_SliderAttribute(string header, NumberType numberType = NumberType.Float, float defaultValue = 0f, float minValue = 0f, float maxValue = 1f) : base(header) {
			this.numberType = numberType;
			this.defaultValue = defaultValue;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
