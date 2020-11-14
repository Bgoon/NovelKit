using GKitForWPF;
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
using TaleKit.Datas.ModelEditor;
using NumberType = GKitForUnity.NumberType;
using Screen = System.Windows.Forms.Screen;

namespace TaleKitEditor.UI.ModelEditor {
	public partial class ValueEditor_NumberBox : UserControl, IValueEditor, INotifyPropertyChanged {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(float), typeof(ValueEditor_NumberBox), new PropertyMetadata(0f));
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.RegisterAttached(nameof(MinValue), typeof(float), typeof(ValueEditor_NumberBox), new PropertyMetadata(float.MinValue));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.RegisterAttached(nameof(MaxValue), typeof(float), typeof(ValueEditor_NumberBox), new PropertyMetadata(float.MaxValue));
		public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached(nameof(NumberType), typeof(NumberType), typeof(ValueEditor_NumberBox), new PropertyMetadata(NumberType.Float));
		public static readonly DependencyProperty NumberFormatProperty = DependencyProperty.RegisterAttached(nameof(NumberFormat), typeof(string), typeof(ValueEditor_NumberBox), new PropertyMetadata());

		public float DragAdjustFactor {
			get; set;
		} = 1f;
		private const int DragAdjustPortalThreshold = 2;

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
				float newValue = value;

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
				if (value is int) {
					value = (float)(int)value;
				}
				value = Mathf.Clamp((float)value, MinValue, MaxValue);

				Value = (float)value;
				EditableValueChanged?.Invoke(value);
			}
		}

		//Cursor drag
		private bool onDragging;
		private float dragStartValue;
		private float dragStartCursorPosX;
		private Screen dragStartScreen;

		// [ Constructor ]
		[Obsolete]
		public ValueEditor_NumberBox() {
			//For designer
			InitializeComponent();
			RegisterEvents();
		}
		public ValueEditor_NumberBox(ValueEditor_NumberBoxAttribute attribute) {
			InitializeComponent();
			RegisterEvents();

			this.NumberType = attribute.numberType;
			this.MinValue = attribute.minValue;
			this.MaxValue = attribute.maxValue;
			this.DragAdjustFactor = attribute.dragAdjustFactor;

			UpdateUI();
		}
		private void RegisterEvents() {
			PropertyChanged += ValueEditorElement_NumberBox_PropertyChanged;
		}

		// [ Event ]
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

		// Drag to adjust value
		private void AdjustButton_MouseDown(object sender, MouseButtonEventArgs e) {
			Mouse.Capture(AdjustButton);

			onDragging = true;
			dragStartValue = Value;
			dragStartCursorPosX = MouseInput.AbsolutePosition.x;

			if(dragStartScreen == null) {
				Vector2 cursorAbsolutePos = MouseInput.AbsolutePosition;
				dragStartScreen = Screen.FromPoint(new System.Drawing.Point((int)cursorAbsolutePos.x, (int)cursorAbsolutePos.y));
			}
		}
		private void AdjustButton_MouseMove(object sender, MouseEventArgs e) {
			if (!onDragging)
				return;

			// If cursor near to screen end, set opposite side position
			Vector2 cursorAbsolutePos = MouseInput.AbsolutePosition;

			float screenLeftDelta = dragStartScreen.Bounds.Left + DragAdjustPortalThreshold - cursorAbsolutePos.x;
			float screenRightDelta = dragStartScreen.Bounds.Right - DragAdjustPortalThreshold - cursorAbsolutePos.x;

			if (screenLeftDelta > 0) {
				int targetPosX = dragStartScreen.Bounds.Right - (DragAdjustPortalThreshold + 10);
				MouseInput.SetAbsolutePosition(new Vector2Int(targetPosX, (int)cursorAbsolutePos.y));

				AdjustButton_MouseDown(null, null);
			} else if (screenRightDelta < 0) {
				int targetPosX = dragStartScreen.Bounds.Left + (DragAdjustPortalThreshold + 10);
				MouseInput.SetAbsolutePosition(new Vector2Int(targetPosX, (int)cursorAbsolutePos.y));

				AdjustButton_MouseDown(null, null);
			}

			float cursorPosXDiff = cursorAbsolutePos.x - dragStartCursorPosX;

			EditableValue = dragStartValue + cursorPosXDiff * 0.3f * DragAdjustFactor;
		}
		private void AdjustButton_MouseUp(object sender, MouseButtonEventArgs e) {
			Mouse.Capture(null);

			onDragging = false;
			dragStartScreen = null;
		}

		// Update
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

	}
}
