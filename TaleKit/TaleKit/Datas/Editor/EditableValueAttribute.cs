using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Editor {
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableValueAttribute : Attribute {

		public string header;
		public ValueEditorType valueEditorType;

		public EditableValueAttribute(string header, ValueEditorType valueEditorType) {
			this.header = header;
			this.valueEditorType = valueEditorType;
		}
	}
}
