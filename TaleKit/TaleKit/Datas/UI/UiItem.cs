using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GKit;
using GKit.Unity;
using GKit.Data;
using TaleKit.Datas.Editor;
using UAnchorPreset = GKit.AnchorPreset;
using UAxisAnchor = GKit.AxisAnchor;
using UVector2 = UnityEngine.Vector2;
using UColor = UnityEngine.Color;
using Newtonsoft.Json.Linq;
using JetBrains.Annotations;
using GKit.Json;

namespace TaleKit.Datas.UI {
	public class UiItem : IEditableModel {

		public event NodeItemInsertedDelegate<UiItem> ChildInserted;

		public event NodeItemDelegate<UiItem, UiItem> ChildRemoved;

		public readonly UiFile OwnerFile;
		public UiItem ParentItem {
			get; private set;
		}
		public List<UiItem> ChildItemList {
			get; private set;
		}

		//HelperDatas
		public UAxisAnchor AnchorX {
			get {
				switch(anchorPreset) {
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

		//Datas
		[ValueEditorComponent_Header("Transform")]
		[ValueEditor_AnchorPreset("Anchor")]
		public UAnchorPreset anchorPreset = UAnchorPreset.StretchAll;

		[ValueEditor_Margin("Margin")]
		public BRect margin;

		[ValueEditor_Vector2("Size")]
		public UVector2 size;
		[ValueEditor_NumberBox("Rotation")]
		public float rotation;

		[ValueEditorComponent_ItemSeparator]
		[ValueEditorComponent_Header("Render")]
		[ValueEditor_ColorBox("Color")]
		public UColor color = Color.white;

		public readonly GameObject GameObject;
		public readonly RectTransform RectTransform;
		public readonly UiTransform UiTransform;
		public readonly CanvasRenderer Renderer;
		public float Alpha {
			get {
				return Renderer.GetAlpha();
			} set {
				Renderer.SetAlpha(value);
			}
		}

		public UiItem(UiFile ownerFile) {
			this.OwnerFile = ownerFile;

			ChildItemList = new List<UiItem>();

			//For unity only (제거할것)
			//GameObject = new GameObject();
			//RectTransform = GameObject.AddComponent<RectTransform>();
			//UiTransform = GameObject.AddComponent<UiTransform>();
			//Renderer = GameObject.AddComponent<CanvasRenderer>();
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

			foreach(UiItem childItem in ChildItemList) {
				jChilds.Add(childItem.ToJObject());
			}

			return jUiItem;
		}
		
	}
}
