using Newtonsoft.Json.Linq;

namespace TaleKit.Datas.Story {
	public abstract class StoryBlockBase {
		public readonly StoryFile OwnerFile;

		public bool IsRoot => ParentItem == null;
		public StoryClip ParentItem {
			get; internal set;
		}
		public readonly StoryBlockType Type;

		public StoryBlockBase(StoryFile ownerFile, StoryBlockType type) {
			this.OwnerFile = ownerFile;
			this.Type = type;
		}

		public abstract JObject ToJObject();

	}
}
