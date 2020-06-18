using GKitForUnity.Data;
using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using TaleKit.Datas.Story.Scenes;

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

		public Dictionary<string, Scene> SceneDict {
			get; private set;
		}
		public Dictionary<string, StoryClip> ClipDict {
			get; private set;
		}

		public event NodeItemDelegate<StoryBlockBase, StoryClip> ItemCreated;
		public event NodeItemDelegate<StoryBlockBase, StoryClip> ItemRemoved;

		public StoryFile(TaleData ownerTaleData) {
			this.OwnerTaleData = ownerTaleData;

			InitMembers();
			CreateRootItem();
		}
		private void InitMembers() {
			SceneDict = new Dictionary<string, Scene>();
			ClipDict = new Dictionary<string, StoryClip>();
		}

		public bool Save(string filename) {
			JObject jFile = ToJObject();

			//Save
			IOUtility.SaveText(jFile.ToString(), filename);
			return true;
		}
		public bool Load(string filename) {
			string jFileString = IOUtility.LoadText(filename, Encoding.UTF8);
			JObject jFile = JObject.Parse(jFileString);

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

		public JObject ToJObject() {
			JObject jFile = new JObject();

			//Add scenes
			JObject jScenes = new JObject();
			jFile.Add("Scenes", jScenes);

			foreach (KeyValuePair<string, Scene> scenePair in SceneDict) {
				JObject jScene = new JObject();
				jScenes.Add(scenePair.Key, jScene);

				Scene scene = scenePair.Value;
				jScene.Add(scene.ToJObject());
			}

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
	}
}
