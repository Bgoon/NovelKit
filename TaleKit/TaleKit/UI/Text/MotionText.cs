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
	public class MotionText : IDisposable {
		protected TaleKitClient Client => TaleKitClient.Instance;
		protected GLoopEngine LoopEngine => Client.LoopEngine;

		//Text setting
		private string text;
		public string Text {
			get {
				return text;
			} set {
				text = value;
				UpdateChars();
			}
		}
		public Font Font {
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
		public GameObject GameObject {
			get; private set;
		}
		public RectTransform RectTransform {
			get; private set;
		}

		//TKChar
		private ObjectPool<MotionChar> charPool;
		private List<MotionChar> charList;

		//Time state
		public float TotalMotionSeconds {
			get {
				if (text == null)
					return 0f;
				return text.Length * CharMotionDelaySeconds + CharMotionSeconds;
			}
		}
		private float elapsedMotionSeconds;

		//LoopAction
		private GLoopAction onTickLoop;

		public MotionText() {
			SetDefaultValues();

			//TKChar
			charList = new List<MotionChar>();

			charPool = new ObjectPool<MotionChar>(CreateCharTask);
			charPool.DisposeTask += DisposeCharTask;
			charPool.GetTask += GetCharTask;
			charPool.ReleaseTask += ReleaseCharTask;

			//Unity object
			GameObject = new GameObject(nameof(MotionText));
			RectTransform = GameObject.AddComponent<RectTransform>();

			onTickLoop = LoopEngine.AddLoopAction(OnTick);
		}
		public void Dispose() {
			onTickLoop.Stop();
			GameObject.Destroy(GameObject);
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
			for (int i=0; i<charList.Count; ++i) {
				MotionChar character = charList[i];

				float indexTime = (elapsedMotionSeconds - i * CharMotionDelaySeconds) / CharMotionSeconds;
				float motionTime = Mathf.Pow(Mathf.Clamp01(indexTime), 0.5f);
				float motionTimeInvert = 1f - motionTime;

				character.Offset = MotionStartOffset * motionTimeInvert;
				character.Alpha = MotionStartAlpha + (1f - MotionStartAlpha) * motionTime;
			}
		}

		//CharPool Task
		private MotionChar CreateCharTask() {
			return new MotionChar(' ', null, 24, RectTransform);
		}
		private void DisposeCharTask(MotionChar value) {
			value.Dispose();
		}
		private void GetCharTask(MotionChar value) {
			value.GameObject.SetActive(true);
		}
		private void ReleaseCharTask(MotionChar value) {
			value.GameObject.SetActive(false);
		}

		private void ClearChars() {
			foreach(MotionChar character in charList) {
				charPool.ReleaseObject(character);
			}
			charList.Clear();
			elapsedMotionSeconds = 0f;
		}
		private void UpdateChars() {
			ClearChars();

			MotionChar sampleChar = charPool.GetObject();
			SetCharStyle(sampleChar);
			sampleChar.Character = 'A';

			Vector2 charPosition = Vector2.zero;
			for (int i=0; i<text.Length; ++i) {
				char characterSrc = text[i];
				switch(characterSrc) {
					case '\r':
						break;
					case '\n':
						charPosition.x = 0f;
						charPosition.y -= sampleChar.Bounds.y + FontSize * 0.2f;
						break;
					default:
						MotionChar character = charPool.GetObject();
						charList.Add(character);

						SetCharStyle(character);
						character.Character = text[i];
						character.Position = charPosition;

						charPosition.x += character.Bounds.x;
						break;
				}
				
			}
			charPool.ReleaseObject(sampleChar);
		}
		private void SetCharStyle(MotionChar character) {
			character.Font = Font;
			character.FontSize = FontSize;
			character.FontColor = FontColor;
		}

	}
}
