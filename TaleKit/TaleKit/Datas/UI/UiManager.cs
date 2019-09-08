using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GKit;
using GKit.Unity;

namespace TaleKit.Datas.UI {
	public class UiManager {
		protected static TaleKitClient Client => TaleKitClient.Instance;
		protected static GLoopEngine LoopEngine => Client.LoopEngine;

		public GameObject Obj_Scenes => Client.GameObjects.Scenes;

		public Dictionary<string, UiItem> UiDict {
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
			UiDict = new Dictionary<string, UiItem>();
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
			Obj_Scenes.AddChild(ScriptText.GameObject);
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
