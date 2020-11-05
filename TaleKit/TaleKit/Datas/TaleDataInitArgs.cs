using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas {
	public class TaleDataInitArgs {
		public bool isEditMode = false;
		public bool createRootUIItem = true;
		public PenMotion.Datas.MotionFile targetMotionData;
		public Action<TaleData> projectLoadedTask;
	}
}
