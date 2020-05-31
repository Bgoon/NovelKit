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
using UVector3 = UnityEngine.Vector3;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// ValueEditorElement_Vector2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Vector3 : UserControl, IValueEditorElement {

		public object EditableValue { 
			get {
				return new UVector3(ValueTextBox_X.Value, ValueTextBox_Y.Value, ValueTextBox_Z.Value);
			} set {
				UVector3 newValue = (UVector3)value;
				ValueTextBox_X.Value = newValue.x;
				ValueTextBox_Y.Value = newValue.y;
				ValueTextBox_Z.Value = newValue.z;
			}
		}

		public event EditableValueChangedDelegate EditableValueChanged;

		public ValueEditorElement_Vector3(ValueEditor_Vector3Attribute attr) {
			InitializeComponent();
			RegisterEvents();

			ValueTextBox_X.MinValue = attr.minValue;
			ValueTextBox_X.MaxValue = attr.maxValue;
			ValueTextBox_X.NumberType = attr.numberType;

			ValueTextBox_Y.MinValue = attr.minValue;
			ValueTextBox_Y.MaxValue = attr.maxValue;
			ValueTextBox_Y.NumberType = attr.numberType;

			ValueTextBox_Z.MinValue = attr.minValue;
			ValueTextBox_Z.MaxValue = attr.maxValue;
			ValueTextBox_Z.NumberType = attr.numberType;
		}
		private void RegisterEvents() {
			ValueTextBox_X.EditableValueChanged += ValueTextBox_X_EditableValueChanged;
			ValueTextBox_Y.EditableValueChanged += ValueTextBox_Y_EditableValueChanged;
			ValueTextBox_Z.EditableValueChanged += ValueTextBox_Z_EditableValueChanged;
		}

		private void ValueTextBox_X_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(new UVector3(ValueTextBox_X.Value, ValueTextBox_Y.Value, ValueTextBox_Z.Value));
		}
		private void ValueTextBox_Y_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(new UVector3(ValueTextBox_X.Value, ValueTextBox_Y.Value, ValueTextBox_Z.Value));
		}
		private void ValueTextBox_Z_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(new UVector3(ValueTextBox_X.Value, ValueTextBox_Y.Value, ValueTextBox_Z.Value));
		}
	}
}
