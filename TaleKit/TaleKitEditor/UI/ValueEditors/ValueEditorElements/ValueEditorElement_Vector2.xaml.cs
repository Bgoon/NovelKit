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
using GKit;
using TaleKit.Datas.Editor;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// ValueEditorElement_Vector2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Vector2 : UserControl, IValueEditorElement {

		public object EditableValue { 
			get {
				return new Vector2(ValueTextBox_X.Value, ValueTextBox_Y.Value);
			} set {
				Vector2 newValue = (Vector2)value;
				ValueTextBox_X.Value = newValue.x;
				ValueTextBox_Y.Value = newValue.y;
			}
		}

		public event Action<object> EditableValueChanged;

		public ValueEditorElement_Vector2(ValueEditor_Vector2Attribute attr) {
			InitializeComponent();
			RegisterEvents();

			ValueTextBox_X.MinValue = attr.minValue;
			ValueTextBox_X.MaxValue = attr.maxValue;
			ValueTextBox_X.NumberType = attr.numberType;

			ValueTextBox_Y.MinValue = attr.minValue;
			ValueTextBox_Y.MaxValue = attr.maxValue;
			ValueTextBox_Y.NumberType = attr.numberType;
		}
		private void RegisterEvents() {
			ValueTextBox_X.EditableValueChanged += ValueTextBox_X_EditableValueChanged;
			ValueTextBox_Y.EditableValueChanged += ValueTextBox_Y_EditableValueChanged;
		}

		private void ValueTextBox_X_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(new Vector2(ValueTextBox_X.Value, ValueTextBox_Y.Value));
		}
		private void ValueTextBox_Y_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(new Vector2(ValueTextBox_X.Value, ValueTextBox_Y.Value));
		}
	}
}
