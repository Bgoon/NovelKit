using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GKit;
using GKit.Unity;

namespace TaleKit.UI {
	public class UiElement {
		public readonly GameObject GameObject;
		public readonly RectTransform RectTransform;
		public readonly UiTransform UiTransform;
		public readonly CanvasRenderer Renderer;
		public float Alpha {
			get {
				return Renderer.GetAlpha();
			} set {
				Renderer.SetAlpha(value);
			}
		}

		public UiElement() {
			GameObject = new GameObject();

			RectTransform = GameObject.AddComponent<RectTransform>();
			UiTransform = GameObject.AddComponent<UiTransform>();
			Renderer = GameObject.AddComponent<CanvasRenderer>();
		}
	}
}
