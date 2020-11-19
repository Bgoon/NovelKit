using GKit.Json;
using GKitForUnity;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UIItem;
using UnityEngine;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// UI를 조작할 수 있는 명령
	/// </summary>
	public class Order_UI : OrderBase {
		private UIFile UiFile => OwnerBlock.OwnerFile.OwnerTaleData.UiFile;
		
		// HelperData
		public float BlendProgress => blendElapsedSeconds / BlendTotalSeconds;
		public float BlendTotalSeconds;
		private float blendElapsedSeconds;

		public float TotalSec => durationSec + delaySec;

		// Data
		[ValueEditor_UiItemSelector("Target UI")]
		public string targetUIGuid;

		[ValueEditor_ModelKeyFrame(nameof(targetUIGuid), nameof(CreateUIKeyData))]
		public UIItemBase UIKeyData;

		[ValueEditor_NumberBox("Duration Sec", minValue = 0)]
		public float durationSec = 1f;
		[ValueEditor_NumberBox("Delay Sec", minValue = 0)]
		public float delaySec = 0f;
		[ValueEditor_EasingSelector("Easing")]
		public string easingKey;

		// [ Constructor ]
		public Order_UI(StoryBlock_Item ownerBlock) : base(ownerBlock) {
			orderType = OrderType.UI;
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

		public UIItemBase CreateUIKeyData() {
			if (string.IsNullOrEmpty(targetUIGuid))
				return null;

			UIItemBase targetUi = UiFile.UISnapshot.GetUiItem(targetUIGuid);

			if(targetUi == null) {
				UIKeyData = null;
				return null;
			}
			switch(targetUi.itemType) {
				case UIItemType.Panel:
					UIKeyData = new UIItem_Panel(UiFile);
					break;
				case UIItemType.Text:
					UIKeyData = new UIItem_Text(UiFile);
					break;
			}
			UIKeyData.IsKeyFrameModel = true;

			return UIKeyData;
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

		public override JObject ToJObject() {
			JObject jOrder = new JObject();
			jOrder.Add("OrderType", orderType.ToString());

			JObject jFields = new JObject();
			jOrder.Add("Fields", jFields);

			SerializeUtility.FieldToJTokenDelegate classHandler = (object model, FieldInfo fieldInfo, out JObject jField) => {
				jField = null;
				 
				if(fieldInfo.GetValue(model) is IKeyFrameModel) {
					jField = new JObject();

					IKeyFrameModel keyFrameModel = fieldInfo.GetValue(model) as IKeyFrameModel;

					SerializeUtility.FieldHandlerDelegate keyPreHandler = (object keyModel, FieldInfo keyFieldInfo, ref bool skip) => {
						skip = !keyFrameModel.KeyFieldNameHashSet.Contains(keyFieldInfo.Name);
					};

					jField.AddFields(keyFrameModel, keyPreHandler, null, null);
				}
			};

			jFields.AddAttrFields<SavableFieldAttribute>(this, null, classHandler);


			return jOrder;
		}
	}
}
