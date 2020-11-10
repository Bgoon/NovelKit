using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
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

		public Dictionary<string, StoryClip> GUID_To_ClipDict {
			get; private set;
		}

		public readonly UiCacheManager UiCacheManager;

		public event NodeItemDelegate<StoryBlockBase, StoryClip> ItemCreated;
		public event NodeItemDelegate<StoryBlockBase, StoryClip> ItemRemoved;

		public event Action<StoryClip> ClipCreated;
		public event Action<StoryClip> ClipRemoved;

		public StoryFile(TaleData ownerTaleData) {
			// Init members
			this.OwnerTaleData = ownerTaleData;

			GUID_To_ClipDict = new Dictionary<string, StoryClip>();
			UiCacheManager = new UiCacheManager(ownerTaleData);

			RootClip = CreateStoryClip();

			// Register events
			ownerTaleData.Tick += UiCacheManager.OnTick;
		}

		public JObject ToJObject() {
			JObject jFile = new JObject();

			//Add clips
			JObject jClips = new JObject();
			jFile.Add("Clips", jClips);

			foreach (KeyValuePair<string, StoryClip> clipPair in GUID_To_ClipDict) {
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
			
			//RootClip.LoadFromJson()


			return true;
		}

		public StoryBlock_Item CreateStoryBlock_Item(StoryClip parentClip) {
			if (parentClip == null)
				parentClip = RootClip;

			StoryBlock_Item item = new StoryBlock_Item(this);

			ItemCreated?.Invoke(item, parentClip);

			parentClip.AddChildItem(item);

			return item;
		}
		public StoryBlock_Clip CreateStoryBlock_Clip(StoryClip parentClip) {
			if (parentClip == null)
				parentClip = RootClip;

			StoryBlock_Clip item = new StoryBlock_Clip(this);

			ItemCreated?.Invoke(item, parentClip);

			parentClip.AddChildItem(item);

			return item;
		}
		public void RemoveStoryBlock(StoryBlockBase item) {
			StoryClip clip = item.ParentClip;
			clip.BlockItemList.Remove(item);

			ItemRemoved?.Invoke(item, clip);
		}

		public StoryClip CreateStoryClip() {
			StoryClip clip = new StoryClip(this);
			GUID_To_ClipDict.Add(clip.guid, clip);

			ClipCreated?.Invoke(clip);

			return clip;
		}
		public void RemoveStoryClip(StoryClip clip) {
			foreach (StoryBlockBase childItem in clip.BlockItemList) {
				RemoveStoryBlock(childItem);
			}
			GUID_To_ClipDict.Remove(clip.guid);

			ClipRemoved?.Invoke(clip);
		}

		private string GetNewItemName() {
			return "New object";
		}

	}
}
