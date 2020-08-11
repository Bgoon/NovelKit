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
using TaleKit.Datas.ModelEditor;
using UVector2 = UnityEngine.Vector2;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// ValueEditorElement_Vector2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_Vector2 : UserControl, IValueEditor {

		public UVector2 Value {
			get {
				return new UVector2(ValueTextBox_X.Value, ValueTextBox_Y.Value);
			}
			set {
				ValueTextBox_X.Value = value.x;
				ValueTextBox_Y.Value = value.y;
			}
		}
		public object EditableValue { 
			get {
				return Value;
			} set {
				Value = (UVector2)value;
			}
		}

		public event EditableValueChangedDelegate EditableValueChanged;

		public ValueEditor_Vector2(ValueEditor_Vector2Attribute attr) {
			InitializeComponent();

			ValueEditor_NumberBox[] numberBoxes = new ValueEditor_NumberBox[] {
				ValueTextBox_X,
				ValueTextBox_Y,
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
