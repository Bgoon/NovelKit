﻿using GKitForUnity;
using System.Collections.Generic;
using TaleKit.Datas.UI.UIItem;
using UnityEngine;

namespace TaleKit.Datas.UI {
	public class UIManager {
		protected static TaleKitClient Client => TaleKitClient.Instance;
		protected static GLoopEngine LoopEngine => Client.LoopEngine;

		public GameObject Obj_Scenes => Client.GameObjects.Scenes;

		public Dictionary<string, UIItemBase> UiDict {
			get; private set;
		}
		public MotionTextBase ScriptText {
			get; private set;
		}


		public UIManager() {
			InitMembers();
			RegisterEvents();
			//CreateTestScene();
		}
		private void InitMembers() {
			UiDict = new Dictionary<string, UIItemBase>();
		}
		private void RegisterEvents() {

		}

		//private void CreateTestScene() {
		//	ScriptText = new MotionTextBase();
		//	ScriptText.RectTransform.SetVerticalAlignment(VerticalAlignment.Bottom);
		//	ScriptText.RectTransform.SetVerticalPivot(VerticalAlignment.Bottom);
		//	ScriptText.RectTransform.SetHorizontalAlignment(HorizontalAlignment.Stretch);
		//	ScriptText.RectTransform.SetHorizontalPivot(HorizontalAlignment.Center);
		//	ScriptText.RectTransform.anchoredPosition = new Vector2(0f, 40f);
		//	ScriptText.RectTransform.sizeDelta = new Vector2(-440, 220f);
		//	ScriptText.FontSize = 24;

		//	ScriptText.FontFamily = "KoPubDotum_Pro Bold";
		//	Obj_Scenes.AddChild(ScriptText.GameObject);
		//}
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
