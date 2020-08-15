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
	public class UiFile : ITaleDataFile, INeedInitialization {
		public delegate void NodeChangedDelegate();

		public readonly TaleData OwnerTaleData;

		public UiItemBase RootUiItem {
			get; private set;
		}

		private bool createRootUiItem;

		// Event
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreatedPreview;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreated;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemRemoved;

		// UI Collection
		public readonly List<UiItemBase> UiItemList;
		public readonly List<UiText> TextList;
		public readonly Dictionary<UiItemBase, object> Item_To_ViewDict;

		// [ Constructor ]
		public UiFile(TaleData ownerTaleData, bool createRootUiItem = true) {
			this.OwnerTaleData = ownerTaleData;
			this.createRootUiItem = createRootUiItem;

			UiItemList = new List<UiItemBase>();
			TextList = new List<UiText>();
			Item_To_ViewDict = new Dictionary<UiItemBase, object>();

		}
		public void Init() {
			if(createRootUiItem) {
				RootUiItem = CreateUiItem(null, UiItemType.Panel);
			}
		}
		public void Clear() {
			RemoveUiItem(RootUiItem);
			RootUiItem = null;

			UiItemList.Clear();
			TextList.Clear();
			Item_To_ViewDict.Clear();
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
			ItemCreatedPreview?.Invoke(item, parentUiItem);

			if (parentUiItem == null) {
				RootUiItem = item;
			} else {
				UiItemList.Add(item);

				parentUiItem.AddChildItem(item);
			}

			ItemCreated?.Invoke(item, parentUiItem);

			return item;
		}
		public void RemoveUiItem(UiItemBase item) {
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
