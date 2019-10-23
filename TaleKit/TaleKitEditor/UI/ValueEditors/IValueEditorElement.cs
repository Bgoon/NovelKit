using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKitEditor.UI.ValueEditors {
	public interface IValueEditorElement {
		event Action<object> EditableValueChanged;

		object EditableValue {
			get; set;
		}
	}
}
