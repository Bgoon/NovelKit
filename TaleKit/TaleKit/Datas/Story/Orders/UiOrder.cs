using GKitForUnity;
using TaleKit.Datas.Editor;
using TaleKit.Datas.UI;
using UnityEngine;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// UI를 조작할 수 있는 명령
	/// </summary>
	public class UiOrder : OrderBase {
		public string targetUiName;
		private UiItem targetUi;

		public float BlendProgress => blendElapsedSeconds / BlendTotalSeconds;
		public float BlendTotalSeconds;
		private float blendElapsedSeconds;

		[ValueEditorComponent_Header("UI테스트 변수")]
		[ValueEditor_Slider("슬라이더", NumberType.Float)]
		public float Float;
		[ValueEditor_NumberBox("숫자박스", NumberType.Float)]
		public float FloatTextBox;
		[ValueEditor_TextBox("텍스트", allowMultiline: true)]
		public string Text;
		[ValueEditor_AnchorPreset("앵커")]
		public AnchorPreset anchorPreset;
		[ValueEditor_Vector3("벡터3")]
		public Vector3 vector3;

		[ValueEditorComponent_Header("실제 모델변수")]
		[ValueEditor_Switch("위치 변경")]
		public bool UsePosition;
		[ValueEditor_Switch("회전 변경")]
		public bool UseRotation;
		[ValueEditor_Switch("크기 변경")]
		public bool UseScale;
		[ValueEditor_Switch("투명도 변경")]
		public bool UseAlpha;
		[ValueEditor_Switch("보이기 변경")]
		public bool UseVisible;

		private Vector2 srcPosition;
		private float srcRotation;
		private Vector2 srcSizeDelta;
		private float srcAlpha;

		public Vector2 DstPosition;
		public float DstRotation;
		public Vector2 DstSizeDelta;
		public float DstAlpha;
		public bool DstVisible;

		public override OrderType OrderType => OrderType.Ui;

		public UiOrder(StoryBlock ownerBlock) : base(ownerBlock) {

		}

		public override void OnStart() {
			base.OnStart();

			targetUi = UiManager.UiDict[targetUiName];
			//srcPosition = targetUi.RectTransform.anchoredPosition;
			//srcRotation = targetUi.RectTransform.localRotation.z;
			//srcSizeDelta = targetUi.RectTransform.sizeDelta;
			//srcAlpha = targetUi.Alpha;
		}
		public override void OnTick() {
			base.OnTick();

			blendElapsedSeconds = Mathf.Clamp(blendElapsedSeconds + LoopEngine.DeltaSeconds, 0f, BlendTotalSeconds);
			SetBlendProgress(BlendProgress);
		}
		public override void OnComplete() {
			base.OnComplete();

			SetBlendProgress(1f);
		}

		public override void Skip() {
			base.Skip();

			SetBlendProgress(1f);
		}

		private void SetBlendProgress(float time) {
			time = Mathf.Clamp01(time);
			IsComplete = time > 0.999f;

			//targetUi.RectTransform.anchoredPosition = srcPosition + (DstPosition - srcPosition) * time;

			//Quaternion rotation = targetUi.RectTransform.localRotation;
			//rotation.z = srcRotation + (DstRotation - srcRotation) * time;
			//targetUi.RectTransform.localRotation = rotation;

			//targetUi.RectTransform.sizeDelta = srcSizeDelta + (DstSizeDelta - srcSizeDelta) * time;
			//targetUi.Alpha = srcAlpha + (DstAlpha - srcAlpha) * time;
			//targetUi.GameObject.SetActive(DstVisible);
		}
	}
}
