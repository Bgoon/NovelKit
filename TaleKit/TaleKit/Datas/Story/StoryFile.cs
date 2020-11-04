using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace TaleKit.Datas.Story {

	//StoryData {
	//	Scenes {
	//		Clip {} []
	//	}
	//	Clips {
	//		Clip {} []
	//	}
	//}
	public class StoryFile : ITaleDataFile {
		public readonly TaleData OwnerTaleData;

		public StoryClip RootClip {
			get; private set;
		}

		public Dictionary<string, StoryClip> ClipDict {
			get; private set;
		}

		public readonly UiCacheManager UiCacheManager;

		public event NodeItemDelegate<StoryBlockBase, StoryClip> ItemCreated;
		public event NodeItemDelegate<StoryBlockBase, StoryClip> ItemRemoved;

		public StoryFile(TaleData ownerTaleData) {
			// Init members
			this.OwnerTaleData = ownerTaleData;

			ClipDict = new Dictionary<string, StoryClip>();
			UiCacheManager = new UiCacheManager(ownerTaleData);

			CreateRootItem();

			// Register events
			ownerTaleData.Tick += UiCacheManager.OnTick;
		}

		public JObject ToJObject() {
			JObject jFile = new JObject();

			//Add clips
			JObject jClips = new JObject();
			jFile.Add("Clips", jClips);

			foreach (KeyValuePair<string, StoryClip> clipPair in ClipDict) {
				JObject jClip = new JObject();
				jClips.Add(clipPair.Key, jClip);

				StoryClip clip = clipPair.Value;
				jClip.Add(clip.ToJObject());
			}

			//Add rootClip
			jFile.Add("RootClip", RootClip.ToJObject());

			return jFile;
		}
		public bool LoadFromJson(JObject jStoryFile) {

			return true;
		}

		private void CreateRootItem() {
			RootClip = new StoryClip(this);

			ItemCreated?.Invoke(RootClip, null);
		}
		public StoryBlock CreateStoryBlockItem(StoryClip parentUiItem) {
			if (parentUiItem == null)
				parentUiItem = RootClip;

			StoryBlock item = new StoryBlock(this);

			ItemCreated?.Invoke(item, parentUiItem);

			parentUiItem.AddChildItem(item);

			return item;
		}
		public StoryClip CreateStoryBlockClipItem(StoryClip parentUiItem) {
			if (parentUiItem == null)
				parentUiItem = RootClip;

			StoryClip item = new StoryClip(this);

			ItemCreated?.Invoke(item, parentUiItem);

			parentUiItem.AddChildItem(item);

			return item;
		}
		public void RemoveStoryBlockItem(StoryBlockBase item) {
			if (item is StoryClip) {
				foreach (StoryBlockBase childItem in ((StoryClip)item).ChildItemList) {
					RemoveStoryBlockItem(childItem);
				}
			}

			StoryClip parentItem = item.ParentItem;
			parentItem.ChildItemList.Remove(item);

			ItemRemoved?.Invoke(item, parentItem);
		}

		private string GetNewItemName() {
			return "New object";
		}

	}
}
