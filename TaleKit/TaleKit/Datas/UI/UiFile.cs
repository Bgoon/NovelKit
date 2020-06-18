using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System.Text;

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

		public UiItem RootUiItem {
			get; private set;
		}

		public event NodeItemDelegate<UiItem, UiItem> ItemCreatedPreview;
		public event NodeItemDelegate<UiItem, UiItem> ItemCreated;
		public event NodeItemDelegate<UiItem, UiItem> ItemRemoved;

		public UiFile(TaleData ownerTaleData, bool createRootItem = true) {
			this.OwnerTaleData = ownerTaleData;

			if (createRootItem) {
				RootUiItem = new UiItem(this);
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

		public UiItem CreateUiItem(UiItem parentUiItem) {
			if (parentUiItem == null)
				parentUiItem = RootUiItem;

			UiItem item = new UiItem(this);
			ItemCreatedPreview?.Invoke(item, parentUiItem);

			parentUiItem.AddChildItem(item);

			ItemCreated?.Invoke(item, parentUiItem);

			return item;
		}
		public void RemoveUiItem(UiItem item) {
			UiItem[] childs = item.ChildItemList.ToArray();
			foreach (UiItem childItem in childs) {
				RemoveUiItem(childItem);
			}

			UiItem parentItem = item.ParentItem;
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
