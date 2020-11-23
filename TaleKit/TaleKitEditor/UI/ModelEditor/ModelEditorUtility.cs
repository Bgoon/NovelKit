using GKitForWPF.Graphics;
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

		public static void CreateOrderEditorView(EditableModel model, StackPanel editorViewContext) {
			CreateSpecificEditorViewDelegate createKeyFrameEditorView = (FieldInfo fieldInfo) => {
				// Handle specific attribute
				ValueEditor_ModelKeyFrameAttribute modelKeyFrameAttr = fieldInfo.GetCustomAttribute<ValueEditor_ModelKeyFrameAttribute>();
				if (modelKeyFrameAttr == null)
					return null;

				EditableModel keyModel = fieldInfo.GetValue(model) as EditableModel;
				StackPanel keyFrameModelEditorContext = new StackPanel();
				keyFrameModelEditorContext.Orientation = Orientation.Vertical;
				keyFrameModelEditorContext.Background = "#00000016".ToBrush();
				keyFrameModelEditorContext.Margin = new Thickness(4);

				CreateKeyModelEditorView();

				// Register events
				if (!string.IsNullOrEmpty(modelKeyFrameAttr.connectedProperty)) {
					model.ModelUpdated += (EditableModel updatedModel, FieldInfo updatedFieldInfo, object editorView) => {
						if(updatedFieldInfo.Name == modelKeyFrameAttr.connectedProperty) {
							keyFrameModelEditorContext.Children.Clear();

							MethodInfo onConnectedPropertyUpdated = model.GetType().GetMethod(modelKeyFrameAttr.onConnectedPropertyUpdated, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
							onConnectedPropertyUpdated?.Invoke(model, null);

							keyModel = fieldInfo.GetValue(model) as EditableModel;
							CreateKeyModelEditorView();
						}
					};
				}

				return keyFrameModelEditorContext;

				void CreateKeyModelEditorView() {
					if (keyModel != null) {
						keyModel.ClearEvents();

						CreateModelEditorView(keyModel, keyFrameModelEditorContext, ModelEditorType.EditKeyFrameModel);
						keyModel.ModelUpdated += (EditableModel nestedKeyModel, FieldInfo keyFieldInfo, object keyEditorView) => {
							model.NotifyModelUpdated(nestedKeyModel, keyFieldInfo, keyEditorView);
						};
					}
				}
			};

			CreateModelEditorView(model, editorViewContext, ModelEditorType.EditModel, createKeyFrameEditorView);
		}

		public static void CreateModelEditorView(EditableModel model, StackPanel editorViewContext, ModelEditorType editorType = ModelEditorType.EditModel, CreateSpecificEditorViewDelegate createSpecificEditorView = null) {
			Type modelType = model.GetType();
			FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(field => field.MetadataToken).ToArray();

			foreach (FieldInfo field in fields) {
				// Create ValueEditor
				if (field.GetCustomAttributes<ValueEditorAttribute>().Count() == 0 &&
					field.GetCustomAttributes<ValueEditorComponentAttribute>().Count() == 0)
					continue;

				ValueEditorAttribute editorAttr = field.GetCustomAttributes<ValueEditorAttribute>().FirstOrDefault();
				if (editorType == ModelEditorType.EditKeyFrameModel && editorAttr.noKey)
					continue;

				FrameworkElement editorView = createSpecificEditorView?.Invoke(field);
				if (editorView == null) {
					editorView = new ValueEditorView(model, field, editorType);
				}

				
				editorViewContext.Children.Add(editorView);
			}
		}
	}
}
