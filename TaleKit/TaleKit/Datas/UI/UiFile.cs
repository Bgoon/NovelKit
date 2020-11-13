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

		private bool createRootUiItem;

		// Event
		public event NodeItemDelegate<UIItemBase, UIItemBase> ItemCreatedPreview;
		public event NodeItemDelegate<UIItemBase, UIItemBase> ItemCreated;
		public event NodeItemDelegate<UIItemBase, UIItemBase> ItemRemoved;

		// UI Collection
		public readonly List<UIItemBase> UiItemList;
		public readonly List<UIText> TextList;
		public readonly UISnapshot UiSnapshot;
		public readonly Dictionary<string, object> Guid_To_RendererDict;

		// [ Constructor ]
		public UIFile(TaleData ownerTaleData, bool createRootUiItem = true) {
			this.OwnerTaleData = ownerTaleData;
			this.createRootUiItem = createRootUiItem;

			UiItemList = new List<UIItemBase>();
			TextList = new List<UIText>();
			UiSnapshot = new UISnapshot();
			Guid_To_RendererDict = new Dictionary<string, object>();

		}
		public void Init() {
			if(createRootUiItem) {
				UiSnapshot.rootUiItem = CreateUiItem(null, UIItemType.Panel);
			}
		}
		public void Clear() {
			RemoveUiItem(UiSnapshot.rootUiItem);

			UiItemList.Clear();
			TextList.Clear();
			UiSnapshot.Clear();
			Guid_To_RendererDict.Clear();
		}

		public JObject ToJObject() {
			JObject jFile = new JObject();
			//Add rootClip
			jFile.Add("RootUI", UiSnapshot.ToJObject());

			return jFile;
		}
		public bool LoadFromJson(JToken jUIFile) {
			LoadUIItem(jUIFile["RootUI"], null);
			
			void LoadUIItem(JToken jUIItem, UIItemBase parentUIItem) {
				JToken jUIItemAttr = jUIItem["Attributes"];
				UIItemType itemType = jUIItemAttr["itemType"].ToObject<UIItemType>();
				string guid = jUIItemAttr["guid"].ToObject<string>();

				UIItemBase UIItem = CreateUiItem(parentUIItem, itemType, guid);
				UIItem.LoadAttrFields<ValueEditorAttribute>(jUIItemAttr.ToObject<JObject>());

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
					item = new UIPanel(this);
					break;
				case UIItemType.Text:
					item = new UIText(this);
					TextList.Add(item as UIText);
					break;
			}
			if(!string.IsNullOrEmpty(guid)) {
				item.guid = guid;
			}

			UiSnapshot.RegisterUiItem(item.guid, item);

			// Add to collection
			ItemCreatedPreview?.Invoke(item, parentUIItem);

			if (parentUIItem == null) {
				UiSnapshot.rootUiItem = item;
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
					TextList.Remove(item as UIText);
					break;
			}
			UiItemList.Remove(item);
			UiSnapshot.RemoveUiItem(item.guid);

			ItemRemoved?.Invoke(item, parentItem);
		}

	}
}
