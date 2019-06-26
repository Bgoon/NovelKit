using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Story {
	public class JobBlockClip {

		public string DisplayName {
			get; set;
		}
		public List<JobBlock> BlockList {
			get; private set;
		}

		public JobBlockClip() {

		}
	}
}
