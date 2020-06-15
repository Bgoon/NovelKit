using System.Collections.Generic;
using TaleKit.Datas.Story.Logic;

namespace TaleKit.Datas.Story.Scenes {
	public class SceneConnection {
		public List<Condition> conditionList;
		public Scene scene;

		public SceneConnection() {

		}

		public bool CheckSatisfied() {
			foreach (Condition condition in conditionList) {
				if (!condition.CheckSatisfied()) {
					return false;
				}
			}
			return true;
		}
	}
}
