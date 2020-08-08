﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaleKit.Datas.Editor;
using TaleKitEditor.UI.ValueEditors;

namespace TaleKitEditor.UI.ValueEditors {
	public delegate void ModelValueChangedDelegate(object model, FieldInfo field, IValueEditorElement valueEditorElement);

	[ContentProperty(nameof(Children))]
	public partial class ValueEditorView : UserControl {
		public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(nameof(Children), typeof(UIElementCollection), typeof(ValueEditorView), new PropertyMetadata());
		public static readonly DependencyProperty ValueNameTextProperty = DependencyProperty.RegisterAttached(nameof(ValueNameText), typeof(string), typeof(ValueEditorView), new PropertyMetadata(null));

		private static Dictionary<Type, Type> ComponentAttr_To_ComponentViewDict = new Dictionary<Type, Type>() {
			{ typeof(ValueEditorComponent_FilePreviewAttribute), typeof(ValueEditorComponent_FilePreview) },
			{ typeof(ValueEditorComponent_HeaderAttribute), typeof(ValueEditorComponent_Header) },
			{ typeof(ValueEditorComponent_ItemSeparatorAttribute), typeof(ItemSeparator) },

		};
		private static Dictionary<Type, Type> EditorAttr_To_EditorViewDict = new Dictionary<Type, Type>() {
			{ typeof(ValueEditor_NumberBoxAttribute), typeof(ValueEditor_NumberBox) },
			{ typeof(ValueEditor_SliderAttribute), typeof(ValueEditor_Slider) },
			{ typeof(ValueEditor_SwitchAttribute), typeof(ValueEditor_Switch) },
			{ typeof(ValueEditor_TextBoxAttribute), typeof(ValueEditor_TextBox) },
			{ typeof(ValueEditor_Vector2Attribute), typeof(ValueEditor_Vector2) },
			{ typeof(ValueEditor_Vector3Attribute), typeof(ValueEditor_Vector3) },
			{ typeof(ValueEditor_ColorBoxAttribute), typeof(ValueEditor_ColorBox) },
			{ typeof(ValueEditor_AnchorPresetAttribute), typeof(ValueEditor_AnchorPreset) },
			{ typeof(ValueEditor_MarginAttribute), typeof(ValueEditor_Margin) },
			{ typeof(ValueEditor_AssetSelectorAttribute), typeof(ValueEditor_AssetSelector) }, 
			{ typeof(ValueEditor_TextBlockViewerAttribute), typeof(ValueEditor_TextBlockViewer) }, 
		};

		public event Action<object> ElementEditorValueChanged;

		public string ValueNameText {
			get {
				return (string)GetValue(ValueNameTextProperty);
			}
			set {
				SetValue(ValueNameTextProperty, value);
			}
		}
		public object EditableValue {
			get {
				return valueEditorElement.EditableValue;
			} set {
				valueEditorElement.EditableValue = value;
			}
		}
		protected IValueEditorElement valueEditorElement;

		protected ValueEditorAttribute attribute;
		protected FieldInfo field;
		protected IEditableModel model;

		public UIElementCollection Children {
			get { 
				return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
			} private set { 
				SetValue(ChildrenProperty, value);
			}
		}

		// [ Constructor ]
		public ValueEditorView() {
			InitializeComponent();

			Children = ValueEditorElementContext.Children;
		}
		public ValueEditorView(IEditableModel model, FieldInfo fieldInfo, ModelValueChangedDelegate modelValueChanged = null) : this() {
			this.model = model;
			this.field = fieldInfo;

			// Classify components : 헤더 등등 부가적인 정보
			ValueEditorComponentAttribute[] components = fieldInfo.GetCustomAttributes(typeof(ValueEditorComponentAttribute)).Select(x=>(ValueEditorComponentAttribute)x).ToArray();
			foreach(ValueEditorComponentAttribute component in components) {
				UserControl view = CreateEditorComponentView(component, model, fieldInfo);

				ValueEditorComponentContext.Children.Add(view);
			}

			// Classify elements : 실제 값
			int editorAttrCount = fieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count();
			if (editorAttrCount > 1) {
				throw new Exception("Field contains more than one ValueEditorAttribute.");
			} else if (editorAttrCount == 0)
				return;

			// Set editor attributes
			attribute = fieldInfo.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;
			ValueNameText = attribute.valueName;

			switch (attribute.layout) {
				case ValueEditorLayout.Wide:
					SetWideEditorElementContext();
					break;
			}

			// ValueEditor element
			UserControl editorElement = CreateEditorElementView(attribute);
			valueEditorElement = (IValueEditorElement)editorElement;

			valueEditorElement.EditableValueChanged += IEditorElement_ElementValueChanged;
			valueEditorElement.EditableValue = fieldInfo.GetValue(model);

			// Register events
			valueEditorElement.EditableValueChanged += (object value) => {
				modelValueChanged?.Invoke(model, fieldInfo, valueEditorElement);
			};
			model.ModelUpdated += UpdateVisible;

			Children.Add(editorElement);
		}

		// [ Event ]
		private void IEditorElement_ElementValueChanged(object value) {
			ElementEditorValueChanged?.Invoke(value);
			field.SetValue(model, value);
			model.UpdateModel();
		}

		private UserControl CreateEditorComponentView(ValueEditorComponentAttribute attr, object model, FieldInfo fieldInfo) {
			UserControl view;
			if (attr is ValueEditorComponent_FilePreviewAttribute) {
				view = new ValueEditorComponent_FilePreview(fieldInfo.GetValue(model) as string);
			} else {
				view = CreateAttributeView(attr, ComponentAttr_To_ComponentViewDict);
			}

			return view;
		}
		private UserControl CreateEditorElementView(ValueEditorAttribute attr) {
			return CreateAttributeView(attr, EditorAttr_To_EditorViewDict);
		}

		private UserControl CreateAttributeView(Attribute attr, Dictionary<Type, Type> attrType_To_ViewTypeDict) {
			// Find constructor
			Type componentViewType = attrType_To_ViewTypeDict[attr.GetType()];
			ConstructorInfo[] componentViewConstructors = componentViewType.GetConstructors();
			ConstructorInfo componentViewConstructor = null;
			foreach (var constructorIterator in componentViewConstructors) {
				if (componentViewConstructor == null || constructorIterator.GetParameters().Length > componentViewConstructor.GetParameters().Length) {
					componentViewConstructor = constructorIterator;
				}
			}
			int constructorParamCount = componentViewConstructor.GetParameters().Length;

			// Call constructor
			UserControl view;
			if (constructorParamCount == 0) {
				view = (UserControl)componentViewConstructor.Invoke(null);
			} else if (constructorParamCount == 1) {
				view = (UserControl)componentViewConstructor.Invoke(new object[] { attr });
			} else {
				throw new Exception("Constructor parameter count is not in (0, 1).");
			}
			return view;
		}

		private void UpdateVisible() {
			if (string.IsNullOrEmpty(attribute.visibleCondition))
				return;

			bool visible = (bool)model.GetType().GetProperty(attribute.visibleCondition, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(model);
			Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		}

		private void SetWideEditorElementContext() {
			Grid.SetColumn(ValueEditorElementContext, 0);
			Grid.SetColumnSpan(ValueEditorElementContext, 2);
			Grid.SetRow(ValueEditorElementContext, 1);

			ValueSeparator.Visibility = Visibility.Visible;
		}
	}
}
