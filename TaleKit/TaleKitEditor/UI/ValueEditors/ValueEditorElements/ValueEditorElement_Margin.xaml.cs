extern alias GKitForUnity;
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
using UnityEngine;
using UBRect = GKitForUnity.GKit.BRect;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// ValueEditorElement_Vector2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Margin : UserControl, IValueEditorElement {
		public UBRect Value {
			get {
				return new UBRect(ValueTextBox_Left.Value, ValueTextBox_Bottom.Value, ValueTextBox_Right.Value, ValueTextBox_Top.Value);
			} set {
				ValueTextBox_Left.Value = value.xMin;
				ValueTextBox_Right.Value = value.xMax;
				ValueTextBox_Top.Value = value.yMax;
				ValueTextBox_Bottom.Value = value.yMin;
			}
		}
		public object EditableValue {
			get {
				return Value;
			} set {
				Value = (UBRect)value;
			}
		}

		public event EditableValueChangedDelegate EditableValueChanged;

		public ValueEditorElement_Margin() {
			InitializeComponent();

			ValueEditorElement_NumberBox[] numberBoxes = new ValueEditorElement_NumberBox[] {
				ValueTextBox_Left,
				ValueTextBox_Right,
				ValueTextBox_Top,
				ValueTextBox_Bottom,
			};
			foreach(var numberBox in numberBoxes) {
				numberBox.EditableValueChanged += ValueTextBox_EditableValueChanged;
			}
		}

		private void ValueTextBox_EditableValueChanged(object value) {
			EditableValueChanged?.Invoke(Value);
		}
	}
}
