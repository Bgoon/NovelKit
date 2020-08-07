using GKit.Json;
using GKitForUnity;
using GKitForUnity.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Resources;
using TaleKit.Datas.Asset;
using TaleKit.Datas.Editor;
using TaleKit.Datas.Resource;
using UnityEngine;
using UAnchorPreset = GKitForUnity.AnchorPreset;
using UAxisAnchor = GKitForUnity.AxisAnchor;
using UColor = UnityEngine.Color;
using UVector2 = UnityEngine.Vector2;

namespace TaleKit.Datas.UI {
	public class UiItem : IEditableModel {
		public event Action ModelUpdated;

		public event NodeItemInsertedDelegate<UiItem> ChildInserted;
		public event NodeItemDelegate<UiItem, UiItem> ChildRemoved;

		private TaleData OwnerTaleData => OwnerFile.OwnerTaleData;
		private AssetManager AssetManager => OwnerTaleData.AssetManager;

		public readonly UiFile OwnerFile;
		public UiItem ParentItem {
			get; private set;
		}
		public List<UiItem> ChildItemList {
			get; private set;
		}

		// External data
		public object View {
			get; set;
		}

		// Helper datas
		public UAxisAnchor AnchorX {
			get {
				switch (anchorPreset) {
					case UAnchorPreset.TopLeft:
					case UAnchorPreset.MidLeft:
					case UAnchorPreset.BotLeft:
					case UAnchorPreset.StretchLeft:
						return UAxisAnchor.Min;
					case UAnchorPreset.TopMid:
					case UAnchorPreset.MidMid:
					case UAnchorPreset.BotMid:
					case UAnchorPreset.StretchMid:
						return UAxisAnchor.Mid;
					case UAnchorPreset.TopRight:
					case UAnchorPreset.MidRight:
					case UAnchorPreset.BotRight:
					case UAnchorPreset.StretchRight:
						return UAxisAnchor.Max;
					default:
						return UAxisAnchor.Stretch;
				}
			}
		}
		public UAxisAnchor AnchorY {
			get {
				switch (anchorPreset) {
					case UAnchorPreset.BotLeft:
					case UAnchorPreset.BotMid:
					case UAnchorPreset.BotRight:
					case UAnchorPreset.BotStretch:
						return UAxisAnchor.Min;
					case UAnchorPreset.MidLeft:
					case UAnchorPreset.MidMid:
					case UAnchorPreset.MidRight:
					case UAnchorPreset.MidStretch:
						return UAxisAnchor.Mid;
					case UAnchorPreset.TopLeft:
					case UAnchorPreset.TopMid:
					case UAnchorPreset.TopRight:
					case UAnchorPreset.TopStretch:
						return UAxisAnchor.Max;
					default:
						return UAxisAnchor.Stretch;
				}
			}
		}

		// Datas
		public UiItemType itemType;

		[ValueEditorComponent_Header("Transform")]
		[ValueEditor_AnchorPreset("Anchor")]
		public UAnchorPreset anchorPreset = UAnchorPreset.StretchAll;

		[ValueEditor_Margin("Margin")]
		public GRect margin;

		[ValueEditor_Vector2("Size")]
		public UVector2 size = new UVector2(1f, 1f);
		[ValueEditor_NumberBox("Rotation")]
		public float rotation;
		
		[ValueEditorComponent_ItemSeparator]
		[ValueEditorComponent_Header("Render")]
		[ValueEditor_ColorBox("Color")]
		public UColor color = Color.white;

		[ValueEditor_AssetSelector("Image", Asset.AssetType.Image)]
		public string imageAssetKey;

		[ValueEditor_Slider("Alpha", NumberType.Float, 0f, 1f)]
		public float alpha = 1f;

		//public readonly GameObject GameObject;
		//public readonly RectTransform RectTransform;
		//public readonly UiTransform UiTransform;
		//public readonly CanvasRenderer Renderer;

		public UiItem(UiFile ownerFile, UiItemType itemType) {
			this.OwnerFile = ownerFile;
			this.itemType = itemType;
			System.Diagnostics.Debug.WriteLine($"Created {itemType}");
			ChildItemList = new List<UiItem>();

			//For unity only (제거할것)
			//GameObject = new GameObject();
			//RectTransform = GameObject.AddComponent<RectTransform>();
			//UiTransform = GameObject.AddComponent<UiTransform>();
			//Renderer = GameObject.AddComponent<CanvasRenderer>();
		}

		public void UpdateModel() {
			ModelUpdated?.Invoke();
		}

		public AssetItem GetImageAsset() {
			return AssetManager.GetAsset(imageAssetKey);
		}

		public void AddChildItem(UiItem item) {
			InsertChildItem(ChildItemList.Count, item);
		}
		public void InsertChildItem(int index, UiItem item) {
			ChildItemList.Insert(index, item);
			item.ParentItem = this;

			ChildInserted?.Invoke(index, item);
		}

		public void RemoveChildItem(UiItem item) {
			ChildItemList.Remove(item);
			item.ParentItem = null;

			ChildRemoved?.Invoke(item, this);
		}

		public JObject ToJObject() {
			JObject jUiItem = new JObject();

			JObject jAttributes = new JObject();
			jUiItem.Add("Attributes", jAttributes);
			jAttributes.AddAttrFields<ValueEditorAttribute>(this);

			JArray jChilds = new JArray();
			jUiItem.Add("Childs", jChilds);

			foreach (UiItem childItem in ChildItemList) {
				jChilds.Add(childItem.ToJObject());
			}

			return jUiItem;
		}

	}
}
