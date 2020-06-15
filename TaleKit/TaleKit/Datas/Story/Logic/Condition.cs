﻿using System;

namespace TaleKit.Datas.Story.Logic {
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
					throw new NotImplementedException("알 수 없는 비교자입니다.");
				case CompareOperator.None:
					return true;
					//case CompareOperator.Equal:
					//	return VariableStorage value
					//case CompareOperator.Greater:
					//	break;
					//case CompareOperator.GEqual:
					//	break;
					//case CompareOperator.Less:
					//	break;
					//case CompareOperator.LEqual:
					//	break;
			}
		}
	}
}
