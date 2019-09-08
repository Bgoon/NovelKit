using GKit;
using GKit.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story {
	public class StoryFile : ITaleDataFile {
		public DateTime exportedTime;

		public StoryBlockClipItem RootClipItem {
			get; private set;
		}

		public event NodeItemDelegate<StoryBlockItemBase, StoryBlockClipItem> ItemCreated;
		public event NodeItemDelegate<StoryBlockItemBase, StoryBlockClipItem> ItemRemoved;

		public StoryFile() {

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

		public StoryBlockItem CreateStoryBlockItem(StoryBlockClipItem parentUiItem) {
			if (parentUiItem == null)
				parentUiItem = RootClipItem;

			StoryBlockItem item = new StoryBlockItem();

			ItemCreated?.Invoke(item, parentUiItem);

			parentUiItem.AddChildItem(item);

			return item;
		}
		public void RemoveUiItem(StoryBlockItemBase item) {
			if (item is StoryBlockClipItem) {
				foreach (StoryBlockItemBase childItem in ((StoryBlockClipItem)item).ChildItemList) {
					RemoveUiItem(childItem);
				}
			}

			StoryBlockClipItem parentItem = item.ParentItem;
			parentItem.ChildItemList.Remove(item);

			ItemRemoved?.Invoke(item, parentItem);
		}

		private string GetNewItemName() {
			return "New object";
		}
	}
}
