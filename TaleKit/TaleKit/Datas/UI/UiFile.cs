using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
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

		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreatedPreview;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemCreated;
		public event NodeItemDelegate<UiItemBase, UiItemBase> ItemRemoved;

		public UiFile(TaleData ownerTaleData, bool createRootItem = true) {
			this.OwnerTaleData = ownerTaleData;

			if (createRootItem) {
				RootUiItem = new UiPanel(this);
			}
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
					break;
			}
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
