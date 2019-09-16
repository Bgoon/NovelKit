using Newtonsoft.Json.Linq;
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

		public int CurrentBlockIndex {
			get; private set;
		}
		public StoryBlock CurrentBlock {
			get {
				if (CurrentBlockIndex < 0 || CurrentBlockIndex >= blockList.Count)
					return null;

				StoryBlock block = blockList[CurrentBlockIndex];
				return block;
			}
		}
		private List<StoryBlock> blockList;

		public Scene() {
			connectionList = new List<SceneConnection>();
			blockList = new List<StoryBlock>();
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
				StoryBlock prevBlock = CurrentBlock;
				prevBlock.OnEnd();

				++CurrentBlockIndex;
				if(CurrentBlockIndex < blockList.Count) {
					StoryBlock currentBlock = CurrentBlock;
					currentBlock.OnEnter();

					return true;
				} else {
					return false;
				}
			} else {
				StoryBlock currentBlock = CurrentBlock;
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

		public JObject ToJObject() {
			JObject jScene = new JObject();

			return jScene;
		}
	}
}
