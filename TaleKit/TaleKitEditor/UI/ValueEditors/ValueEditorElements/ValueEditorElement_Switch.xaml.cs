using GKit;
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

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Switch : UserControl, IValueEditorElement {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(bool), typeof(ValueEditorElement_Switch), new PropertyMetadata(false));

		private static SolidColorBrush DeactiveBackBrush = "737373".ToBrush();
		private static SolidColorBrush ActiveBackBrush = "408DC7".ToBrush();

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return Value;
			} set {
				Value = (bool)value;
			}
		}
		public bool Value {
			get {
				return (bool)GetValue(ValueProperty);
			} set {
				SetValue(ValueProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		public ValueEditorElement_Switch() {
			InitializeComponent();
			RegisterEvents();

			UpdateUI();
		}
		private void RegisterEvents() {
			EditableValueChanged += ValueEditorElement_Switch_EditableValueChanged;
		}

		private void ValueEditorElement_Switch_EditableValueChanged(object obj) {
			UpdateUI();
		}
		private void Button_Click(object sender, RoutedEventArgs e) {
			Value = !Value;
		}

		private void UpdateUI() {
			if (Value) {
				BtnBack.Fill = ActiveBackBrush;
				Button.HorizontalAlignment = HorizontalAlignment.Right;
			} else {
				BtnBack.Fill = DeactiveBackBrush;
				Button.HorizontalAlignment = HorizontalAlignment.Left;
			}
		}
	}
}
