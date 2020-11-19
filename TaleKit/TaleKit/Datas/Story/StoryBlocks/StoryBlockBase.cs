using GKit.Json;
using Newtonsoft.Json.Linq;
using System;
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
		public readonly StoryBlockType blockType;
		[SavableField]
		public bool isVisible;
		[SavableField]
		public string guid;

		// UI Cache
		public bool HasUICache => UICacheSnapshot != null;
		public UISnapshot UICacheSnapshot {
			get; private set;
		}

		// Constructor
		public StoryBlockBase(StoryFile ownerFile, StoryBlockType type) {
			this.OwnerFile = ownerFile;
			this.blockType = type;
			this.isVisible = true;
			this.guid = Guid.NewGuid().ToString();

		}

		// Cache
		public void SaveCache(UISnapshot srcSnapshot) {
			UICacheSnapshot = srcSnapshot.Clone();
		}
		public void ClearCache() {
			UICacheSnapshot = null;
		}

		// Serialize
		public abstract JObject ToJObject();
	}
}
