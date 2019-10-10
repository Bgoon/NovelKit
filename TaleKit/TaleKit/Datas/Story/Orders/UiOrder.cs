using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TaleKit.Datas.UI;
using Newtonsoft.Json.Linq;
using TaleKit.Datas.Editor;

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

		[EditableValue("위치 변경", ValueEditorType.CheckBox)]
		public bool UsePosition;
		[EditableValue("회전 변경", ValueEditorType.CheckBox)]
		public bool UseRotation;
		[EditableValue("크기 변경", ValueEditorType.CheckBox)]
		public bool UseScale;
		[EditableValue("투명도 변경", ValueEditorType.CheckBox)]
		public bool UseAlpha;
		[EditableValue("보이기 변경", ValueEditorType.CheckBox)]
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

		public UiOrder(StoryBlock ownerBlock) : base(ownerBlock) {

		}

		public override void OnStart() {
			base.OnStart();

			targetUi = UiManager.UiDict[targetUiName];
			srcPosition = targetUi.RectTransform.anchoredPosition;
			srcRotation = targetUi.RectTransform.localRotation.z;
			srcSizeDelta = targetUi.RectTransform.sizeDelta;
			srcAlpha = targetUi.Alpha;
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

			targetUi.RectTransform.anchoredPosition = srcPosition + (DstPosition - srcPosition) * time;

			Quaternion rotation = targetUi.RectTransform.localRotation;
			rotation.z = srcRotation + (DstRotation - srcRotation) * time;
			targetUi.RectTransform.localRotation = rotation;

			targetUi.RectTransform.sizeDelta = srcSizeDelta + (DstSizeDelta - srcSizeDelta) * time;
			targetUi.Alpha = srcAlpha + (DstAlpha - srcAlpha) * time;
			targetUi.GameObject.SetActive(DstVisible);
		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
