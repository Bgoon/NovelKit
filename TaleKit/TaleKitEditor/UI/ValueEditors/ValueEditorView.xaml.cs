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
		public ValueEditorView(object model, FieldInfo field) : this() {
			this.model = model;
			this.field = field;

			//Classify components : 헤더 등등
			ValueEditorComponentAttribute[] components = field.GetCustomAttributes(typeof(ValueEditorComponentAttribute)).Select(x=>(ValueEditorComponentAttribute)x).ToArray();
			foreach(ValueEditorComponentAttribute component in components) {
				UserControl view = CreateEditorComponentView(component);

				ValueEditorComponentContext.Children.Add(view);
			}

			//Classify elements : 실제 값
			ValueEditorAttribute element = field.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;
			ValueNameText = element.valueName;

			UserControl editorElement = CreateEditorElementView(element);
			valueEditorElement = (IValueEditorElement)editorElement;

			valueEditorElement.EditableValueChanged += IEditorElement_ElementValueChanged;
			valueEditorElement.EditableValue = field.GetValue(model);

			Children.Add(editorElement);
		}

		private UserControl CreateEditorComponentView(ValueEditorComponentAttribute componentAttribute) {
			UserControl view;
			if(componentAttribute is ValueEditorComponent_HeaderAttribute) {
				var header = new ValueEditorComponent_Header();
				header.Text = ((ValueEditorComponent_HeaderAttribute)componentAttribute).headerText;

				view = header;
			} else {
				throw new NotImplementedException();
			}

			return view;
		}
		private UserControl CreateEditorElementView(ValueEditorAttribute elementAttr) {
			UserControl view;
			if (elementAttr is ValueEditor_NumberBoxAttribute) {
				view = new ValueEditorElement_NumberBox();
			} else if (elementAttr is ValueEditor_SliderAttribute) {
				view = new ValueEditorElement_Slider();
			} else if (elementAttr is ValueEditor_SwitchAttribute) {
				view = new ValueEditorElement_Switch();
			} else if (elementAttr is ValueEditor_TextBoxAttribute) {
				var attr = elementAttr as ValueEditor_TextBoxAttribute;
				view = new ValueEditorElement_TextBox(attr);
			} else if (elementAttr is ValueEditor_NumberBoxAttribute) {
				var attr = elementAttr as ValueEditor_NumberBoxAttribute;
				view = new ValueEditorElement_NumberBox(attr);
			} else {
				throw new NotImplementedException();
			}

			return view;
		}

		private void IEditorElement_ElementValueChanged(object value) {
			ElementEditorValueChanged?.Invoke(value);
			field.SetValue(model, value);
		}
	}
}
