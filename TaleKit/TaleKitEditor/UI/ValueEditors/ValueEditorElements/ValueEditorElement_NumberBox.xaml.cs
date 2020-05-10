using GKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	public partial class ValueEditorElement_NumberBox : UserControl, IValueEditorElement, INotifyPropertyChanged {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(float), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(0f));
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.RegisterAttached(nameof(MinValue), typeof(float), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(0f));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.RegisterAttached(nameof(MaxValue), typeof(float), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(1f));
		public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached(nameof(NumberType), typeof(NumberType), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(NumberType.Float));
		public static readonly DependencyProperty NumberFormatProperty = DependencyProperty.RegisterAttached(nameof(NumberFormat), typeof(string), typeof(ValueEditorElement_NumberBox), new PropertyMetadata());

		public event PropertyChangedEventHandler PropertyChanged;
		public event Action<object> EditableValueChanged;

		public string DisplayValue {
			get {
				if (NumberType == NumberType.Float) {
					if(string.IsNullOrWhiteSpace(NumberFormat)) {
						return Value.ToString();
					} else {
						return Value.ToString(NumberFormat);
					}
				}
				else {
					return IntValue.ToString();
				}
			}
		}
		public int IntValue {
			get {
				return Mathf.RoundToInt(Value);
			}
		}

		public float Value {
			get {
				return (float)GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
				RaisePropertyChanged(nameof(Value));
			}
		}
		public float MinValue {
			get {
				return (float)GetValue(MinValueProperty);
			}
			set {
				SetValue(MinValueProperty, value);
				RaisePropertyChanged(nameof(MinValue));
			}
		}
		public float MaxValue {
			get {
				return (float)GetValue(MaxValueProperty);
			}
			set {
				SetValue(MaxValueProperty, value);
				RaisePropertyChanged(nameof(MaxValue));
			}
		}
		public NumberType NumberType {
			get {
				return (NumberType)GetValue(NumberTypeProperty);
			}
			set {
				SetValue(NumberTypeProperty, value);
				RaisePropertyChanged(nameof(NumberType));
			}
		}
		public string NumberFormat {
			get {
				return (string)GetValue(NumberFormatProperty);
			}
			set {
				SetValue(NumberFormatProperty, value);
				RaisePropertyChanged(nameof(NumberFormat));
			}
		}

		public object EditableValue {
			get {
				if (NumberType == NumberType.Int) {
					return IntValue;
				} else {
					return Value;
				}
			}
			set {
				if(value is int) {
					value = (float)(int)value;
				}
				Value = (float)value;
				EditableValueChanged?.Invoke(value);
			}
		}


		public ValueEditorElement_NumberBox() {
			InitializeComponent();
			RegisterEvents();
		}
		public ValueEditorElement_NumberBox(ValueEditor_NumberBoxAttribute attribute) {
			this.NumberType = attribute.numberType;
			this.MinValue = attribute.minValue;
			this.MaxValue = attribute.maxValue;
		}
		private void RegisterEvents() {
			PropertyChanged += ValueEditorElement_NumberBox_PropertyChanged;
		}

		private void OnEditableValueChanged(object value) {
			
		}


		private void RaisePropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			string numPattern;
			if (NumberType == NumberType.Int) {
				numPattern = "[^0-9]+";
			} else {
				numPattern = "[^0-9.]+";
			}

			Regex regex = new Regex(numPattern);
			e.Handled = regex.IsMatch(e.Text);
		}
		private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e) {
			if (NumberType == NumberType.Int) {
				int resultValue;
				if (!int.TryParse(ValueTextBox.Text, out resultValue)) {
					resultValue = 0;
				}
				EditableValue = resultValue;
			} else {
				float resultValue;
				if (!float.TryParse(ValueTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out resultValue)) {
					resultValue = 0f;
				}
				EditableValue = resultValue;
			}
		}
		private void ValueEditorElement_NumberBox_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
				case nameof(Value):
					RaisePropertyChanged(nameof(IntValue));
					UpdateUI();
					break;
				case nameof(MinValue):
				case nameof(MaxValue):
				case nameof(NumberType):
				case nameof(NumberFormat):
					RaisePropertyChanged(nameof(Value));
					break;
			}
		}

		private void UpdateUI() {
			ValueTextBox.Text = DisplayValue;
		}

	}
}
