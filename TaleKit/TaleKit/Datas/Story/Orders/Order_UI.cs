using GKitForUnity;
using System.Collections.Generic;
using System.Linq;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UiItem;
using UnityEngine;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// UI를 조작할 수 있는 명령
	/// </summary>
	public class Order_UI : OrderBase {
		private UiFile UiFile => OwnerBlock.OwnerFile.OwnerTaleData.UiFile;

		public override OrderType OrderType => OrderType.UI;
		
		[ValueEditor_UiItemSelector("Target UI")]
		public string targetUiName;

		[ValueEditor_ModelKeyFrame(nameof(targetUiName), nameof(OnTargetUiUpdated))]
		public UiItemBase UiKeyData;

		public float BlendProgress => blendElapsedSeconds / BlendTotalSeconds;
		public float BlendTotalSeconds;
		private float blendElapsedSeconds;

		// [ Constructor ]
		public Order_UI(StoryBlock ownerBlock) : base(ownerBlock) {
		}

		// [ Event ]
		public override void OnStart() {
			base.OnStart();
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

		private void OnTargetUiUpdated() {
			UiItemBase targetUi = UiFile.UiItemList.Where(x => x.name == targetUiName).FirstOrDefault(); // TODO : Unique Key로 찾도록 수정해야 함

			if(targetUi == null) {
				UiKeyData = null;
				return;
			}
			switch(targetUi.itemType) {
				case UiItemType.Panel:
					UiKeyData = new UiPanel(UiFile);
					break;
				case UiItemType.Text:
					UiKeyData = new UiText(UiFile);
					break;
			}
			UiKeyData.IsKeyFrameModel = true;
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
