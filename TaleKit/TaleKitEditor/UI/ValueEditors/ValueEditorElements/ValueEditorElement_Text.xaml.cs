using System;
using System.Collections.Generic;
using System.Linq;
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

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Text : UserControl, IValueEditorElement {
		public event Action<object> EditableValueChanged;

		public ValueEditorElement_Text() {
			InitializeComponent();
		}
		public ValueEditorElement_Text(ValueEditor_TextAttribute attribute) : this() {
			ValueTextBox.AcceptsReturn = attribute.allowMultiline;
			ValueTextBox.MaxLength = attribute.maxLength;
			ValueTextBox.AcceptsTab = true;
		}

		public object EditableValue {
			get => ValueTextBox.Text;
			set {
				ValueTextBox.Text = (string)value;
				EditableValueChanged?.Invoke(value);
			}
		}

		private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			EditableValueChanged?.Invoke((string)EditableValue);
		}
	}
}
