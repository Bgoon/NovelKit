using System;
using System.Reflection;

namespace TaleKit.Datas.ModelEditor {
	public delegate void ModelUpdatedDelegate(EditableModel model, FieldInfo fieldInfo, object editorView);

	public class EditableModel {
		public event ModelUpdatedDelegate ModelUpdated;

		public virtual void NotifyModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView) {
			ModelUpdated?.Invoke(model, fieldInfo, editorView);
		}
		public void ClearEvents() {
			ModelUpdated = null;
		}
	}
}
