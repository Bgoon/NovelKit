using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_MarginAttribute : ValueEditorAttribute {
		
		public ValueEditor_MarginAttribute(string valueName) : base(valueName, ValueEditorLayout.Wide) {

		}
	}
}
