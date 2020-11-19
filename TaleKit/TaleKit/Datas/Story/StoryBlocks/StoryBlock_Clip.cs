using GKit.Json;
using GKitForUnity.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TaleKit.Datas.ModelEditor;

namespace TaleKit.Datas.Story {
	public class StoryBlock_Clip : StoryBlockBase {

		// Data
		[ValueEditor_StoryClipSelector("StoryClip")]
		public string targetClipGuid;

		public StoryBlock_Clip(StoryFile ownerFile) : base(ownerFile, StoryBlockType.Clip) {
		}

		public override JObject ToJObject() {
			JObject jBlock = new JObject();

			JObject jFields = new JObject();
			jBlock.Add("Fields", jFields);
			jFields.AddAttrFields<SavableFieldAttribute>(this);

			return jBlock;
		}
	}
}
