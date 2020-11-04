using GKitForUnity;
using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI.UiItem;

namespace TaleKit.Datas.UI {
	//UiData {
	//	UiItem {
	//		Components {} []
	//		Childs {} []
	//	}
	//}
	public class UiFile : ITaleDataFile, INeedInitialization {
		public delegate void NodeChangedDelegate();

		public readonly TaleData OwnerTaleData;

		private bool createRootUiItem;

		// Event
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreatedPreview;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreated;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemRemoved;

		// UI Collection
		public readonly List<UiItemBase> UiItemList;
		public readonly List<UiText> TextList;
		public readonly UiSnapshot UiSnapshot;
		public readonly Dictionary<string, object> Guid_To_RendererDict;

		// [ Constructor ]
		public UiFile(TaleData ownerTaleData, bool createRootUiItem = true) {
			this.OwnerTaleData = ownerTaleData;
			this.createRootUiItem = createRootUiItem;

			UiItemList = new List<UiItemBase>();
			TextList = new List<UiText>();
			UiSnapshot = new UiSnapshot();
			Guid_To_RendererDict = new Dictionary<string, object>();

		}
		public void Init() {
			if(createRootUiItem) {
				UiSnapshot.rootUiItem = CreateUiItem(null, UiItemType.Panel);
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
		public bool LoadFromJson(JObject jUiFile) {

			return true;
		}

		public UiItemBase CreateUiItem(UiItemBase parentUiItem, UiItemType itemType) {

			UiItemBase item;
			switch (itemType) {
				default:
					throw new Exception($"Failed to create UiItem because itemType '{itemType}' is unknown.");
				case UiItemType.Panel:
					item = new UiPanel(this);
					break;
				case UiItemType.Text:
					item = new UiText(this);
					TextList.Add(item as UiText);
					break;
			}
			UiSnapshot.RegisterUiItem(item.guid, item);

			// Add to collection
			ItemCreatedPreview?.Invoke(item, parentUiItem);

			if (parentUiItem == null) {
				UiSnapshot.rootUiItem = item;
			} else {
				UiItemList.Add(item);

				parentUiItem.AddChildItem(item);
			}

			ItemCreated?.Invoke(item, parentUiItem);

			return item;
		}
		public void RemoveUiItem(UiItemBase item) {
			// Recursive
			UiItemBase parentItem = item.ParentItem;
			UiItemBase[] childs = item.ChildItemList.ToArray();
			foreach (UiItemBase childItem in childs) {
				RemoveUiItem(childItem);
			}

			if(parentItem != null) {
				parentItem.RemoveChildItem(item);
			}

			// Remove from collection
			switch(item.itemType) {
				case UiItemType.Text:
					TextList.Remove(item as UiText);
					break;
			}
			UiItemList.Remove(item);
			UiSnapshot.RemoveUiItem(item.guid);

			ItemRemoved?.Invoke(item, parentItem);
		}

	}
}
