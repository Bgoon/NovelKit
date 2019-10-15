using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	public class ValueEditor_TextAttribute : ValueEditorAttribute {

		public int maxLength;
		public bool allowMultiline;
		public string defaultValue;

		public ValueEditor_TextAttribute(string header, int maxLength = 100, bool allowMultiline = false, string defaultValue = null) : base(header) {
			this.maxLength = maxLength;
			this.allowMultiline = allowMultiline;
			this.defaultValue = defaultValue;
		}
	}
}
