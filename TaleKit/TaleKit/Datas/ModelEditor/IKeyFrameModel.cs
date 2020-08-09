using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.ModelEditor {
	public interface IKeyFrameModel {
		Dictionary<string, bool> FieldName_To_UseKeyDict {
			get;
		}
	}
}
