using GKitForUnity;
using GKitForUnity.Graphics;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace TaleKit.Datas.UI {
	public abstract class MotionCharBase : IDisposable {
		//Unity object
		//public GameObject GameObject {
		//	get; private set;
		//}
		//public RectTransform RectTransform {
		//	get; private set;
		//}
		//public Text Text {
		//	get; private set;
		//}
		//public Font Font {
		//	get {
		//		return Text.font;
		//	}
		//	set {
		//		Text.font = value;
		//	}
		//}

		//Text setting
		private char character;
		public char Character {
			get {
				return character;
			}
			set {
				character = value;
				UpdateCharacter();
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
		public float Alpha {
			get {
				return FontColor.a;
			}
			set {
				Color color = FontColor;
				color.a = value;
				FontColor = color;
			}
		}

		//Layout
		private Vector2 position;
		public Vector2 Position {
			get {
				return position;
			}
			set {
				position = value;
				UpdatePosition();
			}
		}
		private Vector2 offset;
		public Vector2 Offset {
			get {
				return offset;
			}
			set {
				offset = value;
				UpdatePosition();
			}
		}
		//public Vector2 Bounds => RectTransform.GetPreferredSize();


		public event Action<Vector2> PositionChanged;
		public event Action<string> CharChanged;

		public MotionCharBase(char character, string fontName, int fontSize, Transform parent) {
			//GameObject = new GameObject();
			//RectTransform = GameObject.AddComponent<RectTransform>();
			//RectTransform.parent = parent;
			//RectTransform.localScale = Vector3.one;
			//Text = GameObject.AddComponent<Text>();
			//Text.text = character.ToString();
			//Text.color = "#C6C6C6".ToColor();
			//Text.font = Font.CreateDynamicFontFromOSFont(fontName, fontSize);
			//Text.fontSize = 25;
			//Text.horizontalOverflow = HorizontalWrapMode.Overflow;
			//Text.verticalOverflow = VerticalWrapMode.Overflow;

			//RectTransform.sizeDelta = RectTransform.GetPreferredSize();
			////왼쪽 위
			//RectTransform.pivot = new Vector2(0f, 1f);
			//RectTransform.anchorMin = new Vector2(0f, 1f);
			//RectTransform.anchorMax = new Vector2(0f, 1f);
		}
		public void Dispose() {
			ClearEvents();
			
		}

		public void ClearEvents() {
			PositionChanged = null;
			CharChanged = null;
		}

		public abstract void SetActive(bool active);

		public void UpdatePosition() {
			PositionChanged?.Invoke(position + offset);
		}
		public void UpdateCharacter() {
			CharChanged?.Invoke(character.ToString());
		}

		public abstract Vector2 GetBounds();
	}
}
