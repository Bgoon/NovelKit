using GKit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit.Data;

namespace TaleKit.Datas.UI {
	public class UiFile : ITaleDataFile {
		public DateTime exportedTime;

		public UiItem RootUiItem {
			get; private set;
		}

		public event NodeItemDelegate<UiItem, UiItem> ItemCreated;
		public event NodeItemDelegate<UiItem, UiItem> ItemRemoved;

		public UiFile() {

		}

		public bool Save(string filename) {
			JObject jMotionFile = new JObject();

			//File info
			jMotionFile.Add(nameof(exportedTime), exportedTime);

			IOUtility.SaveText(jMotionFile.ToString(), filename);
			return true;
		}
		public bool Load(string filename) {
			string jMotionFileString = IOUtility.LoadText(filename, Encoding.UTF8);
			JObject jMotionFile = JObject.Parse(jMotionFileString);

			return true;
		}

		public UiItem CreateUiItem(UiItem parentUiItem) {
			if(parentUiItem == null)
				parentUiItem = RootUiItem;

			UiItem item = new UiItem();

			ItemCreated?.Invoke(item, parentUiItem);

			parentUiItem.AddChildItem(item);

			return item;
		}
		public void RemoveUiItem(UiItem item) {
			foreach(UiItem childItem in item.ChildItemList) {
				RemoveUiItem(childItem);
			}

			UiItem parentItem = item.ParentItem;
			parentItem.ChildItemList.Remove(item);

			ItemRemoved?.Invoke(item, parentItem);
		}
	}
}
