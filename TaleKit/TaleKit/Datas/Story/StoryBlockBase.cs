using GKit.Json;
using Newtonsoft.Json.Linq;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story {
	public abstract class StoryBlockBase : EditableModel {
		public readonly StoryFile OwnerFile;

		public bool IsRoot => ParentClip == null;
		public StoryClip ParentClip {
			get; internal set;
		}

		// Data
		[SavableField]
		public readonly StoryBlockType Type;
		[SavableField]
		public bool isVisible;

		// Ui Cache
		public bool HasUiCache => UiCacheSnapshot != null;
		public UISnapshot UiCacheSnapshot {
			get; private set;
		}

		// Constructor
		public StoryBlockBase(StoryFile ownerFile, StoryBlockType type) {
			this.OwnerFile = ownerFile;
			this.Type = type;
			this.isVisible = true;
		}

		// Cache
		public void SaveCache(UISnapshot srcSnapshot) {
			UiCacheSnapshot = srcSnapshot.Clone();
		}
		public void DeleteCache() {
			UiCacheSnapshot = null;
		}

		// Serialize
		public virtual JObject ToJObject() {
			JObject jBlock = new JObject();

			jBlock.AddAttrFields<SavableFieldAttribute>(this);

			return jBlock;
		}
	}
}
