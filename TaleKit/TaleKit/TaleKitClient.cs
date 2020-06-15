﻿using GKitForUnity;
using TaleKit.Datas.Story.Logic;
using TaleKit.Datas.UI;
using UnityEngine;

namespace TaleKit {
	public class TaleKitClient : MonoBehaviour {
		public static TaleKitClient Instance {
			get; private set;
		}

		//Refs
		[SerializeField] private Canvas canvas;
		[SerializeField] private GLoopEngine loopEngine;
		public Canvas Canvas => canvas;
		public GLoopEngine LoopEngine => loopEngine;

		//Modules
		public Option Option {
			get; private set;
		}
		public UiManager UiManager {
			get; private set;
		}
		public GameObjects GameObjects {
			get; private set;
		}

		public VariableStorage VariableStorage {
			get; private set;
		}

		private void Awake() {
			Instance = this;

			CreateTKModules();
			RegisterEvents();
		}
		private void CreateTKModules() {
			Option = new Option();
			UiManager = new UiManager();
			GameObjects = new GameObjects();
		}
		private void RegisterEvents() {
			LoopEngine.AddLoopAction(OnTick);
		}

		private void OnTick() {
			PlayerInput.UpdateStates();
		}
	}
}
