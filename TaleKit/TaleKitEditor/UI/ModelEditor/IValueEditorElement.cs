using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKitEditor.UI.ModelEditor {
	public delegate void EditableValueChangedDelegate(object value);
	public interface IValueEditorElement {
		event EditableValueChangedDelegate EditableValueChanged;

		object EditableValue {
			get; set;
		}
	}
}
