using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GKit;
using GKit.Unity;

namespace TaleKit.UI {
	public class UiManager {
		protected static TaleKitClient Client => TaleKitClient.Instance;
		protected static GLoopEngine LoopEngine => Client.LoopEngine;

		public GameObject Obj_Scenes => Client.Obj_TKScenes;

		public Dictionary<string, UiElement> UiDict {
			get; private set;
		}
		public MotionText ScriptText {
			get; private set;
		}


		public UiManager() {
			InitMembers();
			RegisterEvents();
			CreateTestScene();
		}
		private void InitMembers() {
			UiDict = new Dictionary<string, UiElement>();
		}
		private void RegisterEvents() {
			
		}

		private void CreateTestScene() {
			ScriptText = new MotionText();
			ScriptText.RectTransform.SetVerticalAlignment(VerticalAlignment.Bottom);
			ScriptText.RectTransform.SetVerticalPivot(VerticalAlignment.Bottom);
			ScriptText.RectTransform.SetHorizontalAlignment(HorizontalAlignment.Stretch);
			ScriptText.RectTransform.SetHorizontalPivot(HorizontalAlignment.Center);
			ScriptText.RectTransform.anchoredPosition = new Vector2(0f, 40f);
			ScriptText.RectTransform.sizeDelta = new Vector2(-440, 220f);
			ScriptText.FontSize = 24;

			ScriptText.Font = Font.CreateDynamicFontFromOSFont("KoPubDotum_Pro Bold", 1);
			Client.Obj_TKScenes.AddChild(ScriptText.GameObject);
		}
		//private void OnTick() {
		//	if(PlayerInput.NextScript.Down) {
		//		if (scriptIndex < TestScripts.Length) {
		//			text.Text = TestScripts[scriptIndex];
		//			++scriptIndex;
		//		}
		//	}
		//}


	}
}
