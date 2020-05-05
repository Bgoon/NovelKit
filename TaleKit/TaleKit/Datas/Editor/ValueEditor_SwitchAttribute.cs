using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_SwitchAttribute : ValueEditorAttribute {

		public bool defaultValue;

		public ValueEditor_SwitchAttribute(string valueName, bool defaultValue = false) : base(valueName) {
			this.defaultValue = defaultValue;
		}
	}
}
