using GKit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story {
	public class StoryBlockClipItem : StoryBlockItemBase {
		public event NodeItemInsertedDelegate<StoryBlockItemBase> ChildInserted;

		public event NodeItemDelegate<StoryBlockItemBase, StoryBlockClipItem> ChildRemoved;

		public List<StoryBlockItemBase> ChildItemList {
			get; private set;
		}

		public StoryBlockClipItem() : base() {
			ChildItemList = new List<StoryBlockItemBase>();
		}

		public void AddChildItem(StoryBlockItemBase item) {
			InsertChildItem(ChildItemList.Count, item);
		}
		public void InsertChildItem(int index, StoryBlockItemBase item) {
			ChildItemList.Insert(index, item);

			ChildInserted?.Invoke(index, item);
		}
		public void RemoveChildItem(StoryBlockItemBase item) {
			ChildItemList.Remove(item);

			ChildRemoved?.Invoke(item, this);
		}
	}
}
