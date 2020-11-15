using GKit.Json;
using GKitForUnity;
using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI.UIItem;

namespace TaleKit.Datas.UI {
	//UiData {
	//	UiItem {
	//		Components {} []
	//		Childs {} []
	//	}
	//}
	public class UIFile : ITaleDataFile, INeedInitialization {
		public delegate void NodeChangedDelegate();

		public readonly TaleData OwnerTaleData;

		// Event
		public event NodeItemDelegate<UIItemBase, UIItemBase> ItemCreatedPreview;
		public event NodeItemDelegate<UIItemBase, UIItemBase> ItemCreated;
		public event NodeItemDelegate<UIItemBase, UIItemBase> ItemRemoved;

		// UI Collection
		public readonly List<UIItemBase> UiItemList;
		public readonly List<UIItem_Text> TextList;
		public readonly UISnapshot UISnapshot;
		public readonly Dictionary<string, object> Guid_To_RendererDict;

		// [ Constructor ]
		public UIFile(TaleData ownerTaleData) {
			this.OwnerTaleData = ownerTaleData;

			UiItemList = new List<UIItemBase>();
			TextList = new List<UIItem_Text>();
			UISnapshot = new UISnapshot();
			Guid_To_RendererDict = new Dictionary<string, object>();

		}
		public void Init() {
			if(!OwnerTaleData.InitArgs.onDataLoad) {
				UISnapshot.rootUiItem = CreateUiItem(null, UIItemType.Panel);
			}
		}
		public void Clear() {
			RemoveUiItem(UISnapshot.rootUiItem);

			UiItemList.Clear();
			TextList.Clear();
			UISnapshot.Clear();
			Guid_To_RendererDict.Clear();
		}

		public JObject ToJObject() {
			JObject jFile = new JObject();
			//Add rootClip
			jFile.Add("RootUI", UISnapshot.ToJObject());

			return jFile;
		}
		public bool LoadFromJson(JToken jUIFile) {
			LoadUIItem(jUIFile["RootUI"], null);
			
			void LoadUIItem(JToken jUIItem, UIItemBase parentUIItem) {
				JToken jUIItemFields = jUIItem["Fields"];
				UIItemType itemType = jUIItemFields["itemType"].ToObject<UIItemType>();
				string guid = jUIItemFields["guid"].ToObject<string>();

				UIItemBase UIItem = CreateUiItem(parentUIItem, itemType, guid);
				UIItem.LoadAttrFields<ValueEditorAttribute>(jUIItemFields.ToObject<JObject>());

				JArray jChildUIItems = jUIItem["Childs"] as JArray;
				foreach (JObject jChildUIItem in jChildUIItems) {
					LoadUIItem(jChildUIItem, UIItem);
				}
			}

			return true;
		}

		/// <summary>
		/// If parentUiItem is null, create root
		/// </summary>
		public UIItemBase CreateUiItem(UIItemBase parentUIItem, UIItemType itemType, string guid = null) {

			UIItemBase item;
			switch (itemType) {
				default:
					throw new Exception($"Failed to create UiItem because itemType '{itemType}' is unknown.");
				case UIItemType.Panel:
					item = new UIItem_Panel(this);
					break;
				case UIItemType.Text:
					item = new UIItem_Text(this);
					TextList.Add(item as UIItem_Text);
					break;
			}
			if(!string.IsNullOrEmpty(guid)) {
				item.guid = guid;
			}

			UISnapshot.RegisterUiItem(item.guid, item);

			// Add to collection
			ItemCreatedPreview?.Invoke(item, parentUIItem);

			if (parentUIItem == null) {
				UISnapshot.rootUiItem = item;
			} else {
				UiItemList.Add(item);

				parentUIItem.AddChildItem(item);
			}

			ItemCreated?.Invoke(item, parentUIItem);

			return item;
		}
		public void RemoveUiItem(UIItemBase item) {
			// Recursive
			UIItemBase parentItem = item.ParentItem;
			UIItemBase[] childs = item.ChildItemList.ToArray();
			foreach (UIItemBase childItem in childs) {
				RemoveUiItem(childItem);
			}

			if(parentItem != null) {
				parentItem.RemoveChildItem(item);
			}

			// Remove from collection
			switch(item.itemType) {
				case UIItemType.Text:
					TextList.Remove(item as UIItem_Text);
					break;
			}
			UiItemList.Remove(item);
			UISnapshot.RemoveUiItem(item.guid);

			ItemRemoved?.Invoke(item, parentItem);
		}

	}
}
