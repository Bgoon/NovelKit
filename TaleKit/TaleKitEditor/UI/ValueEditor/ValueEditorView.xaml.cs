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

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// ValueEditorView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorView : UserControl {
		public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.RegisterAttached(nameof(HeaderText), typeof(string), typeof(ValueEditorView), new PropertyMetadata(null));

		public event Action<object> EditableValueChanged;

		public string HeaderText {
			get {
				return (string)GetValue(HeaderTextProperty);
			}
			set {
				SetValue(HeaderTextProperty, value);
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
		public ValueEditorView(object model, FieldInfo field, ValueEditorAttribute editorAttribute) : this() {
			this.DataContext = this;
			this.model = model;
			this.field = field;
			HeaderText = editorAttribute.header;

			UserControl editorElement = null;
			if (editorAttribute is ValueEditor_NumberAttribute) {
				editorElement = new ValueEditorElement_Number();
			} else if (editorAttribute is ValueEditor_SliderAttribute) {
				editorElement = new ValueEditorElement_Slider();
			} else if (editorAttribute is ValueEditor_SwitchAttribute) {
				editorElement = new ValueEditorElement_Switch();
			} else if (editorAttribute is ValueEditor_TextAttribute) {
				editorElement = new ValueEditorElement_Text();
			} else {
				throw new NotImplementedException();
			}

			valueEditorElement = (IValueEditorElement)editorElement;

			valueEditorElement.EditableValueChanged += IEditorElement_ElementValueChanged;

			ValueEditorElementContext.Children.Add(editorElement);
		}

		private void IEditorElement_ElementValueChanged(object value) {
			EditableValueChanged?.Invoke(value);
			field.SetValue(model, value);
		}
	}
}
