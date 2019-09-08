using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit;

namespace TaleKit.Datas.Story.Scenes {
	public class SceneManager {
		protected static TaleKitClient Client => TaleKitClient.Instance;
		protected static GLoopEngine LoopEngine => Client.LoopEngine;

		public Scene entryScene;
		public Dictionary<string, Scene> sceneDict;

		public Scene CurrentScene {
			get; private set;
		}

		public SceneManager() {
			InitMembers();
			RegisterEvents();
		}
		private void InitMembers() {
			sceneDict = new Dictionary<string, Scene>();
		}
		private void RegisterEvents() {
			LoopEngine.AddLoopAction(OnTick);
		}

		private void OnTick() {
			CurrentScene?.OnTick();
		}

		public void StartFirst() {
			CurrentScene = entryScene;
		}
		public void Start(string sceneName) {
			CurrentScene = sceneDict[sceneName];
		}
	}
}
