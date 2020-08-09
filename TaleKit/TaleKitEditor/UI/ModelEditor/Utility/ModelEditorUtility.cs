using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaleKit.Datas.ModelEditor;

namespace TaleKitEditor.UI.ModelEditor {
	public enum ModelEditorType {
		EditModel,
		EditKeyFrameModel,
	}
	public static class ModelEditorUtility {
		public delegate FrameworkElement CreateSpecificEditorViewDelegate(FieldInfo field);

		public static void CreateOrderEditorView(EditableModel model, StackPanel editorViewContext, ModelValueChangedDelegate modelValueChanged = null) {
			CreateModelEditorView(model, editorViewContext, ModelEditorType.EditModel, modelValueChanged, (FieldInfo fieldInfo) => {
				// Handle specific attribute
				ValueEditor_ModelKeyFrameAttribute modelKeyFrameAttr = fieldInfo.GetCustomAttribute<ValueEditor_ModelKeyFrameAttribute>();
				if (modelKeyFrameAttr == null)
					return null;

				EditableModel keyModel = fieldInfo.GetValue(model) as EditableModel;
				Type keyModelType = model.GetType();
				FieldInfo[] keyModelFields = keyModelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				StackPanel keyFrameModelEditorContext = new StackPanel();
				keyFrameModelEditorContext.Orientation = Orientation.Vertical;

				CreateModelEditorView(keyModel, keyFrameModelEditorContext, ModelEditorType.EditKeyFrameModel);

				return keyFrameModelEditorContext;
			});
		}
		public static void CreateModelEditorView(EditableModel model, StackPanel editorViewContext, ModelEditorType editorType = ModelEditorType.EditModel, ModelValueChangedDelegate modelValueChanged = null, CreateSpecificEditorViewDelegate createSpecificEditorView = null) {
			Type modelType = model.GetType();
			FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields) {
				// Create ValueEditor
				if (field.GetCustomAttributes<ValueEditorAttribute>().Count() == 0 &&
					field.GetCustomAttributes<ValueEditorComponentAttribute>().Count() == 0)
					continue;

				FrameworkElement editorView = createSpecificEditorView?.Invoke(field);
				if (editorView == null) {
					editorView = new ValueEditorView(model, field, editorType, modelValueChanged);
				}
				
				editorViewContext.Children.Add(editorView);
			}
		}
	}
}
