using GKit;
using GKit.WPF;
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
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.RegisterAttached(nameof(MinValue), typeof(float), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(float.MinValue));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.RegisterAttached(nameof(MaxValue), typeof(float), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(float.MaxValue));
		public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached(nameof(NumberType), typeof(NumberType), typeof(ValueEditorElement_NumberBox), new PropertyMetadata(NumberType.Float));
		public static readonly DependencyProperty NumberFormatProperty = DependencyProperty.RegisterAttached(nameof(NumberFormat), typeof(string), typeof(ValueEditorElement_NumberBox), new PropertyMetadata());

		public event PropertyChangedEventHandler PropertyChanged;
		public event EditableValueChangedDelegate EditableValueChanged;

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
				float newValue = Mathf.Clamp(value, MinValue, MaxValue);

				SetValue(ValueProperty, newValue);
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

		//Cursor drag
		private bool onDragging;
		private float dragStartValue;
		private float dragStartCursorPosX;

		[Obsolete]
		public ValueEditorElement_NumberBox() {
			//For designer
			InitializeComponent();
			RegisterEvents();
		}
		public ValueEditorElement_NumberBox(ValueEditor_NumberBoxAttribute attribute) {
			InitializeComponent();
			RegisterEvents();

			this.NumberType = attribute.numberType;
			this.MinValue = attribute.minValue;
			this.MaxValue = attribute.maxValue;

			UpdateUI();
		}
		private void RegisterEvents() {
			PropertyChanged += ValueEditorElement_NumberBox_PropertyChanged;
		}

		private void RaisePropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			string numPattern;
			if (NumberType == NumberType.Int) {
				numPattern = "[^0-9\\-]+";
			} else {
				numPattern = "[^0-9.\\-]+";
			}

			Regex regex = new Regex(numPattern);
			e.Handled = regex.IsMatch(e.Text);
		}
		private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e) {
			UpdateValue();
		}
		private void ValueTextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				UpdateValue();

				e.Handled = true;
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
		private void UpdateValue() {
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

		private void AdjustButton_MouseDown(object sender, MouseButtonEventArgs e) {
			Mouse.Capture(AdjustButton);

			onDragging = true;
			dragStartValue = Value;
			dragStartCursorPosX = (float)e.GetPosition(AdjustButton).X;
		}
		private void AdjustButton_MouseMove(object sender, MouseEventArgs e) {
			const float AdjustFactor = 0.3f;

			if (!onDragging)
				return;

			float cursorPosXDiff = (float)e.GetPosition(AdjustButton).X - dragStartCursorPosX;

			EditableValue = dragStartValue + cursorPosXDiff * AdjustFactor;
		}
		private void AdjustButton_MouseUp(object sender, MouseButtonEventArgs e) {
			Mouse.Capture(null);

			onDragging = false;
		}
	}
}
