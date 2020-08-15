using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.ModelEditor {
	public interface IKeyFrameModel {
		HashSet<string> KeyFieldNameHashSet {
			get;
		}
		bool IsKeyFrameModel {
			get;
		}
	}
}
