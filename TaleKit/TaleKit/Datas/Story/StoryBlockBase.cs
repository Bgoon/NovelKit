using Newtonsoft.Json.Linq;

namespace TaleKit.Datas.Story {
	public abstract class StoryBlockBase {
		public readonly StoryFile OwnerFile;

		public bool IsRoot => ParentItem == null;
		public StoryClip ParentItem {
			get; internal set;
		}

		public StoryBlockBase(StoryFile ownerFile) {
			this.OwnerFile = ownerFile;
		}

		public abstract JObject ToJObject();

	}
}
