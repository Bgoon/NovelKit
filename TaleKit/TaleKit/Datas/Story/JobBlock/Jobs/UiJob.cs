using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TaleKit.UI;

namespace TaleKit.Datas.Story.Jobs {
	/// <summary>
	/// UI를 조작할 수 있는 Job
	/// </summary>
	public class UiJob : BlockJobBase {
		public string targetUiName;
		private UiItem targetUi;

		public float BlendProgress => blendElapsedSeconds / BlendTotalSeconds;
		public float BlendTotalSeconds;
		private float blendElapsedSeconds;

		public bool UsePosition;
		public bool UseRotation;
		public bool UseScale;
		public bool UseAlpha;
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

		public UiJob(JobBlock ownerBlock) : base(ownerBlock) {

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
	}
}
