using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story {
	public class StoryClipState {
		public StoryClip storyClip;
		public int selectedIndex;

		public StoryClipState(StoryClip storyClip, int selectedIdnex) {
			this.storyClip = storyClip;
			this.selectedIndex = selectedIdnex;
		}
	}
}
