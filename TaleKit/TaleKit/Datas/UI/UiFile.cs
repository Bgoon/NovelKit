using GKitForUnity;
using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TaleKit.Datas.UI.UiItem;

namespace TaleKit.Datas.UI {
	//UiData {
	//	UiItem {
	//		Components {} []
	//		Childs {} []
	//	}
	//}
	public class UiFile : ITaleDataFile {
		public delegate void NodeChangedDelegate();

		public readonly TaleData OwnerTaleData;

		public UiItemBase RootUiItem {
			get; private set;
		}

		// Event
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreatedPreview;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreated;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemRemoved;

		// UI Collection
		public readonly List<UiItemBase> UiItemList;
		public readonly List<UiText> TextList;

		// [ Constructor ]
		public UiFile(TaleData ownerTaleData, bool createRootItem = true) {
			this.OwnerTaleData = ownerTaleData;

			UiItemList = new List<UiItemBase>();
			TextList = new List<UiText>();

			if (createRootItem) {
				RootUiItem = new UiPanel(this);
			}
		}
		public void Clear() {
			RootUiItem.ClearChildItem();

			UiItemList.Clear();
			TextList.Clear();
		}

		public bool Save(string filename) {
			JObject jFile = new JObject();

			IOUtility.SaveText(jFile.ToString(), filename);
			return true;
		}
		public bool Load(string filename) {
			string jMotionFileString = IOUtility.LoadText(filename, Encoding.UTF8);
			JObject jFile = JObject.Parse(jMotionFileString);

			return true;
		}

		public UiItemBase CreateUiItem(UiItemBase parentUiItem, UiItemType itemType) {
			if (parentUiItem == null)
				parentUiItem = RootUiItem;

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
			// Add to collection
			UiItemList.Add(item);

			ItemCreatedPreview?.Invoke(item, parentUiItem);

			parentUiItem.AddChildItem(item);

			ItemCreated?.Invoke(item, parentUiItem);

			return item;
		}
		public void RemoveUiItem(UiItemBase item) {
			UiItemBase[] childs = item.ChildItemList.ToArray();
			foreach (UiItemBase childItem in childs) {
				RemoveUiItem(childItem);
			}

			UiItemBase parentItem = item.ParentItem;
			parentItem.RemoveChildItem(item);

			// Remove from collection
			switch(item.itemType) {
				case UiItemType.Text:
					TextList.Remove(item as UiText);
					break;
			}
			UiItemList.Remove(item);


			ItemRemoved?.Invoke(item, parentItem);
		}

		public JObject ToJObject() {
			JObject jFile = new JObject();
			//Add rootClip
			jFile.Add("RootUI", RootUiItem.ToJObject());

			return jFile;
		}
	}
}
