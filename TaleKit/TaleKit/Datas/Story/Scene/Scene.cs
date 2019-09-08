using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story.Scenes {
	public class Scene {
		public string name;

		public bool HasConnection => connectionList.Count > 0;
		private List<SceneConnection> connectionList;

		private List<JobBlock> blockList;
		public JobBlock CurrentBlock {
			get {
				if (CurrentBlockIndex < 0 || CurrentBlockIndex >= blockList.Count)
					return null;

				JobBlock block = blockList[CurrentBlockIndex];
				return block;
			}
		}
		public int CurrentBlockIndex {
			get; private set;
		}

		public Scene() {
			connectionList = new List<SceneConnection>();
			blockList = new List<JobBlock>();
		}

		public void OnTick() {
			if(PlayerInput.NextScript.Down) {
				NextBlock();
			}
			CurrentBlock.OnTick();
		}

		public void Start() {
			CurrentBlockIndex = 0;
		}
		private void End() {
			
		}

		public bool NextBlock() {
			if(CurrentBlock.IsComplete) {
				JobBlock prevBlock = CurrentBlock;
				prevBlock.OnEnd();

				++CurrentBlockIndex;
				if(CurrentBlockIndex < blockList.Count) {
					JobBlock currentBlock = CurrentBlock;
					currentBlock.OnEnter();

					return true;
				} else {
					return false;
				}
			} else {
				JobBlock currentBlock = CurrentBlock;
				currentBlock.Skip();

				return true;
			}
		}

		public Scene GetNextScene() {
			foreach(SceneConnection conn in connectionList) {
				if(conn.CheckSatisfied()) {
					return conn.scene;
				}
			}
			return null;
		}
	}
}
