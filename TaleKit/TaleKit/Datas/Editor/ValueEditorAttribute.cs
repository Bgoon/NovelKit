using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {

	[AttributeUsage(AttributeTargets.Field)]
	public class ValueEditorAttribute : Attribute {

		public string header;

		public ValueEditorAttribute(string header) {
			this.header = header;
		}
	}
}
