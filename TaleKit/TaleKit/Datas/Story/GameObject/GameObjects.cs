using GKitForUnity;
using UnityEngine;

namespace TaleKit {
	public class GameObjects {
		protected static TaleKitClient Client => TaleKitClient.Instance;

		public GameObject Root {
			get; private set;
		}
		public GameObject Scenes {
			get; private set;
		}
		public GameObject Transitions {
			get; private set;
		}
		public GameObject Sounds {
			get; private set;
		}

		public GameObjects() {
			CreateUiObjects();
		}
		private void CreateUiObjects() {
			Root = CreateUiObject("Root");
			Root.SetParent(Client.Canvas.transform);

			Scenes = CreateUiObject("Scenes");
			Scenes.SetParent(Root);

			Transitions = CreateUiObject("Transitions");
			Transitions.SetParent(Root);

			Sounds = CreateUiObject("Sounds");
			Sounds.SetParent(Root);
		}

		private GameObject CreateUiObject(string name) {
			GameObject gameObject = new GameObject(name);

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			//rectTransform.SetHorizontalAlignment(HorizontalAlignment.Stretch);
			//rectTransform.SetVerticalAlignment(VerticalAlignment.Stretch);
			rectTransform.sizeDelta = Vector2.zero;

			return gameObject;
		}
	}
}
