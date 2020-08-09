using GKitForUnity.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TaleKit.Datas.Story {
	public class StoryClip : StoryBlockBase {
		public event NodeItemInsertedDelegate<StoryBlockBase> ChildInserted;

		public event NodeItemDelegate<StoryBlockBase, StoryClip> ChildRemoved;

		public List<StoryBlockBase> ChildItemList {
			get; private set;
		}

		public StoryClip(StoryFile ownerFile) : base(ownerFile, StoryBlockType.StoryClip) {
			ChildItemList = new List<StoryBlockBase>();
		}

		public void AddChildItem(StoryBlockBase item) {
			InsertChildItem(ChildItemList.Count, item);

		}
		public void InsertChildItem(int index, StoryBlockBase item) {
			ChildItemList.Insert(index, item);
			item.ParentItem = this;

			ChildInserted?.Invoke(index, item);
		}
		public void RemoveChildItem(StoryBlockBase item) {
			ChildItemList.Remove(item);
			item.ParentItem = null;

			ChildRemoved?.Invoke(item, this);
		}

		public override JObject ToJObject() {
			JObject jClip = new JObject();

			//Add items
			JArray jItems = new JArray();
			jClip.Add("Items", jItems);

			foreach (StoryBlockBase block in ChildItemList) {
				jItems.Add(block.ToJObject());
			}

			return jClip;
		}
	}
}
