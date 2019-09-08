using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story {
	public class StoryBlockItemBase {
		public readonly StoryFile OwnerFile;

		public bool IsRoot => ParentItem == null;
		public StoryBlockClipItem ParentItem {
			get; internal set;
		}
		


	}
}
