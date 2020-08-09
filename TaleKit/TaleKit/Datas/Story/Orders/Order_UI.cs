using GKitForUnity;
using System.Collections.Generic;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using UnityEngine;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// UI를 조작할 수 있는 명령
	/// </summary>
	public class Order_UI : OrderBase, IKeyFrameModel {
		public override OrderType OrderType => OrderType.UI;
		
		public string targetUiName;
		public UiItemBase targetUI;

		[ValueEditor_ModelKeyFrame()]
		public UiItemBase KeyFrameUI;

		public Dictionary<string, bool> FieldName_To_UseKeyDict {
			get; private set;
		}

		public float BlendProgress => blendElapsedSeconds / BlendTotalSeconds;
		public float BlendTotalSeconds;
		private float blendElapsedSeconds;

		// [ Constructor ]
		public Order_UI(StoryBlock ownerBlock) : base(ownerBlock) {
			FieldName_To_UseKeyDict = new Dictionary<string, bool>();

			// TODO : 원래 targetUI가 선택될 때 생성되야 하지만, 테스트를 위해 전역 멤버 찾아서 전달
			KeyFrameUI = new UiItemBase(ownerBlock.OwnerFile.OwnerTaleData.UiFile, UiItemType.Panel);
		}

		// [ Event ]
		public override void OnStart() {
			base.OnStart();

			targetUI = UiManager.UiDict[targetUiName];
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
