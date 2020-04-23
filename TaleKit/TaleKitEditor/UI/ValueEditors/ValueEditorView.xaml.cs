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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaleKit.Datas.Editor;
using TaleKitEditor.UI.ValueEditors;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// ValueEditorView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorView : UserControl {
		public static readonly DependencyProperty ValueNameTextProperty = DependencyProperty.RegisterAttached(nameof(ValueNameText), typeof(string), typeof(ValueEditorView), new PropertyMetadata(null));

		public event Action<object> EditableValueChanged;

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

		public ValueEditorView() {
			InitializeComponent();
		}
		public ValueEditorView(object model, FieldInfo field) : this() {
			this.DataContext = this;
			this.model = model;
			this.field = field;

			//Classify components
			ValueEditorComponentAttribute[] components = field.GetCustomAttributes(typeof(ValueEditorComponentAttribute)).Select(x=>(ValueEditorComponentAttribute)x).ToArray();
			foreach(ValueEditorComponentAttribute component in components) {
				UserControl view = CreateEditorComponentView(component);

				ValueEditorComponentContext.Children.Add(view);
			}

			//Classify elements
			ValueEditorAttribute element = field.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;
			ValueNameText = element.header;

			UserControl editorElement = CreateEditorElementView(element);
			valueEditorElement = (IValueEditorElement)editorElement;

			valueEditorElement.EditableValueChanged += IEditorElement_ElementValueChanged;
			valueEditorElement.EditableValue = field.GetValue(model);

			ValueEditorElementContext.Children.Add(editorElement);
		}

		private UserControl CreateEditorComponentView(ValueEditorComponentAttribute componentAttribute) {
			UserControl view;
			if(componentAttribute is ValueEditorComponent_HeaderAttribute) {
				view = new ValueEditorComponent_Header(((ValueEditorComponent_HeaderAttribute)componentAttribute).headerText);
			} else {
				throw new NotImplementedException();
			}

			return view;
		}
		private UserControl CreateEditorElementView(ValueEditorAttribute elementAttribute) {
			UserControl view;
			if (elementAttribute is ValueEditor_NumberBoxAttribute) {
				view = new ValueEditorElement_NumberBox();
			} else if (elementAttribute is ValueEditor_SliderAttribute) {
				view = new ValueEditorElement_Slider();
			} else if (elementAttribute is ValueEditor_SwitchAttribute) {
				view = new ValueEditorElement_Switch();
			} else if (elementAttribute is ValueEditor_TextAttribute) {
				view = new ValueEditorElement_Text();
			} else {
				throw new NotImplementedException();
			}

			return view;
		}

		private void IEditorElement_ElementValueChanged(object value) {
			EditableValueChanged?.Invoke(value);
			field.SetValue(model, value);
		}
	}
}
