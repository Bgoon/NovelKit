using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditorComponent_HeaderAttribute : ValueEditorComponentAttribute {
		public string headerText;

		public ValueEditorComponent_HeaderAttribute(string headerText) {
			this.headerText = headerText;
		}
	}
}
