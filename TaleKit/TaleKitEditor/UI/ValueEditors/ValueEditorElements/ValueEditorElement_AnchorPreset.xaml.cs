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
	public partial class ValueEditorElement_AnchorPreset : UserControl, IValueEditorElement {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(AnchorPreset), typeof(ValueEditorElement_AnchorPreset), new PropertyMetadata(AnchorPreset.StretchAll));

		private static SolidColorBrush DeactiveBackBrush = "737373".ToBrush();
		private static SolidColorBrush ActiveBackBrush = "408DC7".ToBrush();

		public event Action<object> EditableValueChanged;

		public object EditableValue {
			get {
				return Value;
			} set {
				Value = (AnchorPreset)value;
			}
		}
		public AnchorPreset Value {
			get {
				return (AnchorPreset)GetValue(ValueProperty);
			} set {
				SetValue(ValueProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		public ValueEditorElement_AnchorPreset() {
			InitializeComponent();
			RegisterEvents();

			UpdateUI();
		}
		private void RegisterEvents() {
		}

		private void UpdateUI() {
		}
	}
}
