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
		public static readonly DependencyProperty ElementValueProperty = DependencyProperty.RegisterAttached(nameof(EditableValue), typeof(bool), typeof(ValueEditorElement_Switch), new PropertyMetadata(false));

		private static SolidColorBrush DeactiveBackBrush = "737373".ToBrush();
		private static SolidColorBrush ActiveBackBrush = "408DC7".ToBrush();

		public event Action<object> EditableValueChanged;

		public object EditableValue {
			get {
				return GetValue(ElementValueProperty);
			} set {
				SetValue(ElementValueProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}
		public bool BoolValue {
			get {
				return (bool)EditableValue;
			} set {
				EditableValue = value;
			}
		}

		public ValueEditorElement_Switch() {
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			BoolValue = !BoolValue;
			if(BoolValue) {
				BtnBack.Fill = ActiveBackBrush;
				Button.HorizontalAlignment = HorizontalAlignment.Right;
			} else {
				BtnBack.Fill = DeactiveBackBrush;
				Button.HorizontalAlignment = HorizontalAlignment.Left;
			}
		}
	}
}
