using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit;

namespace TaleKit {
	public class TaleBlock {

		public List<TaleCommand> commandList;
		private GRoutine[] commandRoutines;

		public TaleBlock() {
			commandList = new List<TaleCommand>();


		}
		public IEnumerator ExecuteCommands() {

			commandRoutines = new GRoutine[commandList.Count];
			for(int i=0; i<commandList.Count; ++i) {
				//commandRoutines[i] = 
			}
			yield break;
		}
	}
}
