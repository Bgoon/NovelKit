using Newtonsoft.Json.Linq;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story {
	public abstract class StoryBlockBase : EditableModel {
		public readonly StoryFile OwnerFile;

		public bool IsRoot => ParentItem == null;
		public StoryClip ParentItem {
			get; internal set;
		}
		public readonly StoryBlockType Type;
		
		// Ui Cache
		public bool HasUiCache => UiCacheSnapshot != null;
		public UiSnapshot UiCacheSnapshot {
			get; private set;
		}

		// Constructor
		public StoryBlockBase(StoryFile ownerFile, StoryBlockType type) {
			this.OwnerFile = ownerFile;
			this.Type = type;
		}

		// Cache
		public void SaveCache(UiSnapshot srcSnapshot) {
			UiCacheSnapshot = srcSnapshot.Clone();
		}
		public void DeleteCache() {
			UiCacheSnapshot = null;
		}

		// Serialize
		public abstract JObject ToJObject();

	}
}
