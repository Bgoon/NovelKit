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
using GKitForWPF;
using TaleKit.Datas.Editor;
using UVector3 = UnityEngine.Vector3;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// ValueEditorElement_Vector2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Vector3 : UserControl, IValueEditorElement {

		public UVector3 Value {
			get {
				return new UVector3(ValueTextBox_X.Value, ValueTextBox_Y.Value, ValueTextBox_Z.Value);
			}
			set {
				ValueTextBox_X.Value = value.x;
				ValueTextBox_Y.Value = value.y;
				ValueTextBox_Z.Value = value.z;
			}
		}
		public object EditableValue { 
			get {
				return Value;
			} set {
				Value = (UVector3)value;
			}
		}

		public event EditableValueChangedDelegate EditableValueChanged;

		public ValueEditorElement_Vector3(ValueEditor_Vector3Attribute attr) {
			InitializeComponent();

			ValueEditorElement_NumberBox[] numberBoxes = new ValueEditorElement_NumberBox[] {
				ValueTextBox_X,
				ValueTextBox_Y,
				ValueTextBox_Z,
			};
			foreach (var numberBox in numberBoxes) {
				numberBox.MinValue = attr.minValue;
				numberBox.MaxValue = attr.maxValue;
				numberBox.NumberType = attr.numberType;

				numberBox.EditableValueChanged += ValueTextBox_EditableValueChanged;
			}
		}

		private void ValueTextBox_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(Value);
		}
	}
}
