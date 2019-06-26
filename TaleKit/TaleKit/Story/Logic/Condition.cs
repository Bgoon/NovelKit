using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Story.Logic {
	public class Condition {
		protected static TaleKitClient Client => TaleKitClient.Instance;
		protected static VariableStorage VariableStorage => Client.VariableStorage;

		public CompareOperator compareOperator;
		public string variableName;
		public dynamic value;

		public Condition() {
			compareOperator = CompareOperator.None;
		}
		public bool CheckSatisfied() {
			switch (compareOperator) {
				default:
					throw new Exception("알 수 없는 비교자입니다.");
				case CompareOperator.None:
					return true;
				case CompareOperator.Equal:
					return value 
				case CompareOperator.Greater:
					break;
				case CompareOperator.GEqual:
					break;
				case CompareOperator.Less:
					break;
				case CompareOperator.LEqual:
					break;
			}
		}
	}
}
