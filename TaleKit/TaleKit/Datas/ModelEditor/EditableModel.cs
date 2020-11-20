using System;
using System.Reflection;

namespace TaleKit.Datas.ModelEditor {
	public delegate void ModelUpdatedDelegate(EditableModel model, FieldInfo fieldInfo, object editorView);

	public class EditableModel {
		/// <summary>
		/// 참고 : EditorContext가 비활성화 될 때 이벤트가 초기화됩니다.
		/// </summary>
		public event ModelUpdatedDelegate ModelUpdated;

		public virtual void NotifyModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView) {
			ModelUpdated?.Invoke(model, fieldInfo, editorView);
		}
		public void ClearEvents() {
			ModelUpdated = null;
		}
	}
}
