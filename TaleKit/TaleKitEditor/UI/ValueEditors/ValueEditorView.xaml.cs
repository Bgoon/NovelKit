using System;
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

		protected FieldInfo field;
		protected object model;

		public UIElementCollection Children {
			get { 
				return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
			} private set { 
				SetValue(ChildrenProperty, value);
			}
		}

		public ValueEditorView() {
			InitializeComponent();

			Children = ValueEditorElementContext.Children;
		}
		public ValueEditorView(object model, FieldInfo fieldInfo, ModelValueChangedDelegate modelValueChanged = null) : this() {
			this.model = model;
			this.field = fieldInfo;

			//Classify components : 헤더 등등 부가적인 정보
			ValueEditorComponentAttribute[] components = fieldInfo.GetCustomAttributes(typeof(ValueEditorComponentAttribute)).Select(x=>(ValueEditorComponentAttribute)x).ToArray();
			foreach(ValueEditorComponentAttribute component in components) {
				UserControl view = CreateEditorComponentView(component, model, fieldInfo);

				ValueEditorComponentContext.Children.Add(view);
			}

			//Classify elements : 실제 값
			int editorAttrCount = fieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count();
			if (editorAttrCount > 1) {
				throw new Exception("Field에 ValueEditorAttribute가 2개 이상 있습니다.");
			} else if (editorAttrCount == 0)
				return;

			//ValueName
			ValueEditorAttribute element = fieldInfo.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;
			ValueNameText = element.valueName;

			//ValueEditor
			UserControl editorElement = CreateEditorElementView(element);
			valueEditorElement = (IValueEditorElement)editorElement;

			valueEditorElement.EditableValueChanged += IEditorElement_ElementValueChanged;
			valueEditorElement.EditableValue = fieldInfo.GetValue(model);

			//Outter event
			valueEditorElement.EditableValueChanged += (object value) => {
				modelValueChanged?.Invoke(model, fieldInfo, valueEditorElement);
			};

			switch (element.layout) {
				case ValueEditorLayout.Wide:
					SetWideEditorElementContext();
					break;
			}

			Children.Add(editorElement);
		}

		private void IEditorElement_ElementValueChanged(object value) {
			ElementEditorValueChanged?.Invoke(value);
			field.SetValue(model, value);
		}

		private UserControl CreateEditorComponentView(ValueEditorComponentAttribute componentAttr, object model, FieldInfo fieldInfo) {
			UserControl view;
			if (componentAttr is ValueEditorComponent_HeaderAttribute) {
				var header = new ValueEditorComponent_Header();
				header.Text = ((ValueEditorComponent_HeaderAttribute)componentAttr).headerText;
				view = header;
			} else if (componentAttr is ValueEditorComponent_ItemSeparatorAttribute) {
				view = new ItemSeparator();
			} else if (componentAttr is ValueEditorComponent_FilePreviewAttribute) {
				view = new ValueEditorComponent_FilePreview(fieldInfo.GetValue(model) as string);
			} else {
				throw new NotImplementedException();
			}

			return view;
		}
		private UserControl CreateEditorElementView(ValueEditorAttribute elementAttr) {
			UserControl view;
			if (elementAttr is ValueEditor_NumberBoxAttribute) {
				view = new ValueEditorElement_NumberBox(elementAttr as ValueEditor_NumberBoxAttribute);

			} else if (elementAttr is ValueEditor_SliderAttribute) {
				view = new ValueEditorElement_Slider();

			} else if (elementAttr is ValueEditor_SwitchAttribute) {
				view = new ValueEditorElement_Switch();

			} else if (elementAttr is ValueEditor_TextBoxAttribute) {
				view = new ValueEditorElement_TextBox(elementAttr as ValueEditor_TextBoxAttribute);

			} else if (elementAttr is ValueEditor_NumberBoxAttribute) {
				view = new ValueEditorElement_NumberBox(elementAttr as ValueEditor_NumberBoxAttribute);

			} else if (elementAttr is ValueEditor_Vector2Attribute) {
				view = new ValueEditorElement_Vector2(elementAttr as ValueEditor_Vector2Attribute);

			} else if (elementAttr is ValueEditor_Vector3Attribute) {
				view = new ValueEditorElement_Vector3(elementAttr as ValueEditor_Vector3Attribute);

			} else if (elementAttr is ValueEditor_ColorBoxAttribute) {
				view = new ValueEditorElement_ColorBox();

			} else if (elementAttr is ValueEditor_AnchorPresetAttribute) {
				view = new ValueEditorElement_AnchorPreset();

			} else if (elementAttr is ValueEditor_MarginAttribute) {
				view = new ValueEditorElement_Margin();
			} else if (elementAttr is ValueEditor_AssetSelectorAttribute) {
				view = new ValueEditorElement_AssetSelector(elementAttr as ValueEditor_AssetSelectorAttribute);
			} else {
				throw new NotImplementedException();
			}

			return view;
		}

		private void SetWideEditorElementContext() {
			Grid.SetColumn(ValueEditorElementContext, 0);
			Grid.SetColumnSpan(ValueEditorElementContext, 2);
			Grid.SetRow(ValueEditorElementContext, 1);

			ValueSeparator.Visibility = Visibility.Visible;
		}
	}
}
