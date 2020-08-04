using GKitForUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaleKit.Datas.UI {
	public abstract class MotionTextBase : IDisposable {
		protected TaleKitClient Client => TaleKitClient.Instance;
		protected GLoopEngine LoopEngine => Client.LoopEngine;

		//Text setting
		private string text;
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				UpdateChars();
			}
		}
		public string FontFamily {
			get; set;
		}
		public int FontSize {
			get; set;
		}
		public Color FontColor {
			get; set;
		}
		public float LineSpaceFactor {
			get; set;
		}

		//Motion setting
		public bool IsMotionComplete => string.IsNullOrEmpty(text) || elapsedMotionSeconds >= TotalMotionSeconds;
		public float CharMotionSeconds {
			get; set;
		}
		public float CharMotionDelaySeconds {
			get; set;
		}
		public float MotionStartAlpha {
			get; set;
		}
		public Vector2 MotionStartOffset {
			get; set;
		}

		//Unity object
		//public GameObject GameObject {
		//	get; private set;
		//}
		//public RectTransform RectTransform {
		//	get; private set;
		//}

		//TKChar
		private ObjectPool<MotionCharBase> charPool;
		private List<MotionCharBase> charList;

		//Time state
		public float TotalMotionSeconds {
			get {
				//이렇게 구하지 말고 for문 돌아서 업데이트 시켜야 함 (Char마다 Delay를 줄 수 있으므로)
				if (text == null)
					return 0f;
				return text.Length * CharMotionDelaySeconds + CharMotionSeconds;
			}
		}
		private float elapsedMotionSeconds;

		//LoopAction
		private GLoopAction onTickLoop;

		public MotionTextBase() {
			SetDefaultValues();

			charList = new List<MotionCharBase>();

			charPool = new ObjectPool<MotionCharBase>(CreateCharTask);
			charPool.DisposeInstanceTask += DisposeCharTask;
			charPool.GetInstanceTask += GetCharTask;
			charPool.ReturnInstanceTask += ReleaseCharTask;

			//U nity object
			//GameObject = new GameObject(nameof(MotionText));
			//RectTransform = GameObject.AddComponent<RectTransform>();

			onTickLoop = LoopEngine.AddLoopAction(OnTick);
		}
		public virtual void Dispose() {
			onTickLoop.Stop();
			//GameObject.Destroy(GameObject);
		}
		private void SetDefaultValues() {
			//Font setting
			FontSize = 24;
			FontColor = Color.white;

			//Motion setting
			CharMotionSeconds = 0.5f;
			CharMotionDelaySeconds = 0.04f;
			MotionStartAlpha = 0f;
			MotionStartOffset = new Vector2(0f, -14f);
		}

		// Pool events
		protected abstract MotionCharBase CreateCharTask();
			//return new MotionCharBase(' ', null, 24, RectTransform);
	
		private void DisposeCharTask(MotionCharBase value) {
			value.Dispose();
		}
		private void GetCharTask(MotionCharBase value) {
			value.SetActive(true);
		}
		private void ReleaseCharTask(MotionCharBase value) {
			value.SetActive(false);
		}

		//Events
		private void OnTick() {
			UpdateMotion();
		}

		//Motion
		public void SkipMotion() {
			elapsedMotionSeconds = TotalMotionSeconds;
		}
		private void UpdateMotion() {
			if (elapsedMotionSeconds > TotalMotionSeconds)
				return;

			elapsedMotionSeconds += LoopEngine.DeltaSeconds;
			for (int i = 0; i < charList.Count; ++i) {
				MotionCharBase character = charList[i];

				float indexTime = (elapsedMotionSeconds - i * CharMotionDelaySeconds) / CharMotionSeconds;
				float motionTime = Mathf.Pow(Mathf.Clamp01(indexTime), 0.5f);
				float motionTimeInvert = 1f - motionTime;

				character.Offset = MotionStartOffset * motionTimeInvert;
				character.Alpha = MotionStartAlpha + (1f - MotionStartAlpha) * motionTime;
			}
		}

		//CharPool Task
		private void ClearChars() {
			foreach (MotionCharBase character in charList) {
				charPool.ReturnInstance(character);
			}
			charList.Clear();
			elapsedMotionSeconds = 0f;
		}
		private void UpdateChars() {
			ClearChars();

			MotionCharBase sampleChar = charPool.GetInstance();
			SetCharStyle(sampleChar);
			sampleChar.Character = 'A';

			Vector2 charPosition = Vector2.zero;
			for (int i = 0; i < text.Length; ++i) {
				char characterSrc = text[i];
				switch (characterSrc) {
					case '\r':
						break;
					case '\n':
						charPosition.x = 0f;
						charPosition.y -= sampleChar.GetBounds().y + FontSize * 0.2f;
						break;
					default:
						MotionCharBase character = charPool.GetInstance();
						charList.Add(character);

						SetCharStyle(character);
						character.Character = text[i];
						character.Position = charPosition;

						charPosition.x += character.GetBounds().x;
						break;
				}

			}
			charPool.ReturnInstance(sampleChar);
		}
		private void SetCharStyle(MotionCharBase character) {
			character.FontFamily = FontFamily;
			character.FontSize = FontSize;
			character.FontColor = FontColor;
		}

	}
}
