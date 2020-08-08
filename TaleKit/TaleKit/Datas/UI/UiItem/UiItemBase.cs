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
	public class UiItemBase : IEditableModel {
		public event Action ModelUpdated;

		public event NodeItemInsertedDelegate<UiItemBase> ChildInserted;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ChildRemoved;

		protected TaleData OwnerTaleData => OwnerFile.OwnerTaleData;
		protected AssetManager AssetManager => OwnerTaleData.AssetManager;

		public readonly UiFile OwnerFile;
		public UiItemBase ParentItem {
			get; private set;
		}
		public List<UiItemBase> ChildItemList {
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
		public int guid;

		[ValueEditorComponent_Header("Common")]
		[ValueEditor_TextBox("Name")]
		public string name = "UI Item";
		[ValueEditor_TextBlockViewer("Type")]
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
		
		//public readonly GameObject GameObject;
		//public readonly RectTransform RectTransform;
		//public readonly UiTransform UiTransform;
		//public readonly CanvasRenderer Renderer;

		public UiItemBase(UiFile ownerFile, UiItemType itemType) {
			this.OwnerFile = ownerFile;
			this.itemType = itemType;
			System.Diagnostics.Debug.WriteLine($"Created {itemType}");
			ChildItemList = new List<UiItemBase>();

			//For unity only (제거할것)
			//GameObject = new GameObject();
			//RectTransform = GameObject.AddComponent<RectTransform>();
			//UiTransform = GameObject.AddComponent<UiTransform>();
			//Renderer = GameObject.AddComponent<CanvasRenderer>();
		}

		public void UpdateModel() {
			ModelUpdated?.Invoke();
		}

		public void AddChildItem(UiItemBase item) {
			InsertChildItem(ChildItemList.Count, item);
		}
		public void InsertChildItem(int index, UiItemBase item) {
			ChildItemList.Insert(index, item);
			item.ParentItem = this;

			ChildInserted?.Invoke(index, item);
		}

		public void RemoveChildItem(UiItemBase item) {
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

			foreach (UiItemBase childItem in ChildItemList) {
				jChilds.Add(childItem.ToJObject());
			}

			return jUiItem;
		}

	}
}
