using GKit.Json;
using GKitForUnity;
using GKitForUnity.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using TaleKit.Datas.Asset;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Resource;
using UnityEngine;
using UAnchorPreset = GKitForUnity.AnchorPreset;
using UAxisAnchor = GKitForUnity.AxisAnchor;
using UColor = UnityEngine.Color;
using UVector2 = UnityEngine.Vector2;

namespace TaleKit.Datas.UI.UIItem {
	[Serializable]
	public class UIItemBase : EditableModel, IKeyFrameModel, ICloneable {
		public event NodeItemInsertedDelegate<UIItemBase> ChildInserted;
		public event NodeItemDelegate<UIItemBase, UIItemBase> ChildRemoved;

		protected TaleData OwnerTaleData => OwnerFile.OwnerTaleData;
		protected AssetManager AssetManager => OwnerTaleData.AssetManager;

		// Inherit
		public readonly UIFile OwnerFile;
		public UIItemBase ParentItem {
			get; set;
		}
		public List<UIItemBase> ChildItemList {
			get; private set;
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

		// External data
		public object View {
			get; set;
		}
		public HashSet<string> KeyFieldNameHashSet {
			get; private set;
		}
		public bool IsKeyFrameModel {
			get; set;
		}

		// Datas
		[ValueEditorComponent_Header("Common")]
		[ValueEditor_TextBlockViewer("GUID", noKey = true)]
		public string guid;

		[ValueEditor_TextBlockViewer("Name", noKey = true)]
		public string name = "UI Item";
		[ValueEditor_TextBlockViewer("Type")]
		public UIItemType itemType;

		[ValueEditorComponent_Header("Transform")]
		[ValueEditor_AnchorPreset("Anchor")]
		public UAnchorPreset anchorPreset = UAnchorPreset.StretchAll;

		[ValueEditor_Margin("Margin")]
		public GRect margin;

		[ValueEditor_Vector2("Size", minValue = 0f)]
		public UVector2 size = new UVector2(100f, 100f);
		[ValueEditor_NumberBox("Rotation")]
		public float rotation;
		
		//public readonly GameObject GameObject;
		//public readonly RectTransform RectTransform;
		//public readonly UiTransform UiTransform;
		//public readonly CanvasRenderer Renderer;

		public UIItemBase(UIFile ownerFile, UIItemType itemType) {
			this.guid = Guid.NewGuid().ToString();
			this.OwnerFile = ownerFile;
			this.itemType = itemType;

			ChildItemList = new List<UIItemBase>();

			KeyFieldNameHashSet = new HashSet<string>();

			name = $"New {itemType}";

			//For unity only (제거할것)
			//GameObject = new GameObject();
			//RectTransform = GameObject.AddComponent<RectTransform>();
			//UiTransform = GameObject.AddComponent<UiTransform>();
			//Renderer = GameObject.AddComponent<CanvasRenderer>();
		}
		public void InitializeClone() {
			ChildItemList = new List<UIItemBase>();
			ChildInserted = null;
			ChildRemoved = null;
		}

		public void AddChildItem(UIItemBase item) {
			InsertChildItem(ChildItemList.Count, item);
		}
		public void InsertChildItem(int index, UIItemBase item) {
			ChildItemList.Insert(index, item);
			item.ParentItem = this;

			ChildInserted?.Invoke(index, item);
		}
		public void RemoveChildItem(UIItemBase item) {
			ChildItemList.Remove(item);
			item.ParentItem = null;

			ChildRemoved?.Invoke(item, this);
		}
		public void ClearChildItems() {
			foreach(var childItem in ChildItemList) {
				RemoveChildItem(childItem);
			}
		}

		public void CopyDataFrom(UIItemBase srcUIItem) {
			if (srcUIItem.itemType != itemType)
				return;

			foreach(FieldInfo fieldInfo in srcUIItem.GetType().GetFields()) {
				if (fieldInfo.GetCustomAttribute(typeof(ValueEditorAttribute)) == null)
					continue;

				fieldInfo.SetValue(this, fieldInfo.GetValue(srcUIItem));
			}
		}

		public JObject ToJObject() {
			JObject jUiItem = new JObject();

			JObject jFields = new JObject();
			jUiItem.Add("Fields", jFields);
			jFields.AddAttrFields<SavableFieldAttribute>(this);

			JArray jChilds = new JArray();
			jUiItem.Add("Childs", jChilds);

			foreach (UIItemBase childItem in ChildItemList) {
				jChilds.Add(childItem.ToJObject());
			}

			return jUiItem;
		}
		public object Clone() {
			UIItemBase cloneItem = this.MemberwiseClone() as UIItemBase;
			cloneItem.KeyFieldNameHashSet = new HashSet<string>();
			return cloneItem;
		}

	}
}
