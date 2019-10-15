using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_SwitchAttribute : ValueEditorAttribute {

		public bool defaultValue;

		public ValueEditor_SwitchAttribute(string header, bool defaultValue = false) : base(header) {
			this.defaultValue = defaultValue;
		}
	}
}
