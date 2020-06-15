using UnityEngine;

namespace TaleKit.Datas.UI {
	[RequireComponent(typeof(RectTransform))]
	public class DepthScroller : MonoBehaviour {
		[Range(0f, 1f)]
		public float scrollSensitivity = 0.05f;

		private new RectTransform transform;

		private void Start() {
			transform = GetComponent<RectTransform>();
		}
		private void Update() {
			Vector2 cursorPos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
			cursorPos -= new Vector2(0.5f, 0.5f);
			cursorPos = new Vector2(Mathf.Clamp(cursorPos.x, -0.5f, 0.5f), Mathf.Clamp(cursorPos.y, -0.5f, 0.5f));


			float scale = 1f + scrollSensitivity;
			transform.localScale = new Vector3(scale, scale, 1f);
			transform.localPosition = new Vector3(
			-cursorPos.x * transform.rect.width * scrollSensitivity,
			-cursorPos.y * transform.rect.height * scrollSensitivity,
			0f);
		}
	}
}
