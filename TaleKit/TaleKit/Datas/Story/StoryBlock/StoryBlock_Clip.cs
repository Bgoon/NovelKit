using GKitForUnity.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TaleKit.Datas.Story.StoryBlock {
	public class StoryBlock_Clip : StoryBlockBase {
		

		public StoryBlock_Clip(StoryFile ownerFile) : base(ownerFile, StoryBlockType.Clip) {
		}

		public override JObject ToJObject() {
			return null;
		}
	}
}
