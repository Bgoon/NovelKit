using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit {
	public class TKFile {

		public TKStoryBoard storyBoard;
		public TKResource resource;
		

		public static TKFile LoadFile(string filename) {
			return new TKFile();
		}
		public TKFile() {
			storyBoard = new TKStoryBoard();
			resource = new TKResource();
		}
	}
}
