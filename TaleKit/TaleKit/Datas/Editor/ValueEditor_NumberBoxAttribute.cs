using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_NumberBoxAttribute : ValueEditorAttribute {
		public NumberType numberType;
		public float defaultValue;
		public float minValue;
		public float maxValue;

		public ValueEditor_NumberBoxAttribute(string header, NumberType numberType = NumberType.Float, float defaultValue = 0f, float minValue = float.NegativeInfinity, float maxValue = float.PositiveInfinity) : base(header) {
			this.numberType = numberType;
			this.defaultValue = defaultValue;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
