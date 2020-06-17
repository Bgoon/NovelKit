using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaleKit.Datas.Editor;

namespace TaleKitEditor.UI.ValueEditors {
	public static class ValueEditorUtility {
		public static void CreateValueEditorViews(IEditableModel model, StackPanel editorViewContext, ModelValueChangedDelegate modelValueChanged = null) {
			Type modelType = model.GetType();
			FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields) {
				if (field.GetCustomAttributes<ValueEditorAttribute>().Count() == 0 &&
					field.GetCustomAttributes<ValueEditorComponentAttribute>().Count() == 0)
					continue;

				ValueEditorView valueEditorView = new ValueEditorView(model, field, modelValueChanged);
				editorViewContext.Children.Add(valueEditorView);
			}
		}
	}
}
