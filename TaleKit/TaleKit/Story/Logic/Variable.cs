using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Story.Logic {
	public class Variable {
		public VariableType type;
		public dynamic value;
		public bool IsCorrectType => CheckCorrectType(value, type);

		public static bool CheckCorrectType(dynamic value, VariableType type) {
			switch (type) {
				default:
					throw new Exception("알 수 없는 타입으로 검사를 시도했습니다.");
				case VariableType.IntNumber:
					return
						value is sbyte || value is byte ||
						value is short || value is ushort ||
						value is int || value is uint ||
						value is long || value is ulong ||
						value is decimal;
				case VariableType.FloatNumber:
					return CheckCorrectType(value, VariableType.IntNumber) || value is float || value is double;
				case VariableType.Text:
					return value is string;
			}
		}
		public Variable() {
			type = VariableType.IntNumber;
			value = 0;
		}

	}
}
