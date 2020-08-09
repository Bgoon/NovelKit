using GKitForWPF.Graphics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_Switch : UserControl, IValueEditorElement {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(bool), typeof(ValueEditor_Switch), new PropertyMetadata(false));

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

		public ValueEditor_Switch() {
			InitializeComponent();
			RegisterEvents();

			UpdateUI();
		}
		private void RegisterEvents() {
			EditableValueChanged += OnEditableValueChanged;
		}

		private void OnEditableValueChanged(object obj) {
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
