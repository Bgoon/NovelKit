using GKit.Json;
using GKitForUnity.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.ModelEditor;

namespace TaleKit.Datas.Story {
	public class StoryClip {
		public event NodeItemInsertedDelegate<StoryBlockBase> ChildInserted;
		public event NodeItemDelegate<StoryBlockBase, StoryClip> ChildRemoved;

		public readonly StoryFile OwnerFile;

		public readonly List<StoryBlockBase> BlockItemList;
		[SavableField]
		public string guid;
		[SavableField]
		public string name = "New clip";

		// [ Constructor ]
		public StoryClip(StoryFile ownerFile) {
			this.OwnerFile = ownerFile;
			this.guid = Guid.NewGuid().ToString();

			BlockItemList = new List<StoryBlockBase>();

		}

		// [ Tree ]
		public void AddChildItem(StoryBlockBase item) {
			InsertChildItem(BlockItemList.Count, item);

		}
		public void InsertChildItem(int index, StoryBlockBase item) {
			BlockItemList.Insert(index, item);
			item.ParentClip = this;

			ChildInserted?.Invoke(index, item);
		}
		public void RemoveChildItem(StoryBlockBase item) {
			BlockItemList.Remove(item);
			item.ParentClip = null;

			ChildRemoved?.Invoke(item, this);
		}

		// [ Data ]
		public JObject ToJObject() {
			JObject jClip = new JObject();

			// Add attributes
			JObject jFields = new JObject();
			jClip.Add("Fields", jFields);

			jFields.AddAttrFields<SavableFieldAttribute>(this);


			// Add items
			JArray jBlocks = new JArray();
			jClip.Add("Blocks", jBlocks);

			foreach (StoryBlockBase block in BlockItemList) {
				jBlocks.Add(block.ToJObject());
			}

			return jClip;
		}
	}
}
