using GKitForWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TaleKitEditor.UI.ModelEditor {
	public partial class ValueEditor_Slider : UserControl, IValueEditorElement, INotifyPropertyChanged {
		public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached(nameof(NumberType), typeof(NumberType), typeof(ValueEditor_Slider), new PropertyMetadata(NumberType.Float));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(float), typeof(ValueEditor_Slider), new PropertyMetadata(0f));
		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.RegisterAttached(nameof(DefaultValue), typeof(float), typeof(ValueEditor_Slider), new PropertyMetadata(0f));
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.RegisterAttached(nameof(MinValue), typeof(float), typeof(ValueEditor_Slider), new PropertyMetadata(0f));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.RegisterAttached(nameof(MaxValue), typeof(float), typeof(ValueEditor_Slider), new PropertyMetadata(1f));

		public event PropertyChangedEventHandler PropertyChanged;
		public event EditableValueChangedDelegate EditableValueChanged;

		private const string DisplayFormat = "0.00";
		public string DisplayValue {
			get {
				if (NumberType == NumberType.Float)
					return Value.ToString(DisplayFormat);
				else
					return IntValue.ToString();
			}
		}
		public string DisplayMinValue {
			get {
				if (NumberType == NumberType.Float)
					return MinValue.ToString(DisplayFormat);
				else
					return ((int)MinValue).ToString();
			}
		}
		public string DisplayMaxValue {
			get {
				if (NumberType == NumberType.Float)
					return MaxValue.ToString(DisplayFormat);
				else
					return ((int)MaxValue).ToString();
			}
		}
		public int IntValue {
			get {
				return Mathf.RoundToInt(Value);
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
		public float Value {
			get {
				return (float)GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
				RaisePropertyChanged(nameof(Value));
			}
		}
		public float DefaultValue {
			get {
				return (float)GetValue(DefaultValueProperty);
			}
			set {
				SetValue(DefaultValueProperty, value);
				RaisePropertyChanged(nameof(DefaultValue));
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

		public object EditableValue {
			get {
				if(NumberType == NumberType.Int) {
					return IntValue;
				} else {
					return Value;
				}
			}
			set {
				Value = (float)value;
				EditableValueChanged?.Invoke(value);
			}
		}

		//Input
		private bool onDragging;

		public ValueEditor_Slider() {
			InitializeComponent();
			Init();
			RegisterEvents();

			UpdateUI();
		}
		private void Init() {
			DataContext = this;

			MinValue = 0f;
			MaxValue = 1f;
			Value = 0f;
		}
		private void RegisterEvents() {
			PropertyChanged += ValueEditorElement_Slider_PropertyChanged;
			SizeChanged += ValueEditorElement_Slider_SizeChanged;
		}

		private void RaisePropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void InputContext_MouseDown(object sender, MouseButtonEventArgs e) {
			Mouse.Capture(InputContext);

			onDragging = true;
		}
		private void InputContext_MouseMove(object sender, MouseEventArgs e) {
			if (!onDragging)
				return;

			float sideMargin = (float)BackLine.Margin.Left;
			float sliderWidth = (float)InputContext.ActualWidth - sideMargin * 2f;
			Point cursorPos = e.GetPosition(InputContext);

			float inputValue = Mathf.Clamp01(((float)cursorPos.X - sideMargin) / sliderWidth);

			Value = Mathf.Max(0, inputValue * (MaxValue - MinValue) + MinValue);

			EditableValueChanged?.Invoke(Value);
		}
		private void InputContext_MouseUp(object sender, MouseButtonEventArgs e) {
			Mouse.Capture(null);

			onDragging = false;
		}
		private void ValueEditorElement_Slider_SizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateUI();
		}
		private void ValueEditorElement_Slider_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch(e.PropertyName) {
				case nameof(Value):
					RaisePropertyChanged(nameof(IntValue));
					RaisePropertyChanged(nameof(DisplayValue));
					UpdateUI();
					break;
				case nameof(MinValue):
					RaisePropertyChanged(nameof(DisplayMinValue));
					RaisePropertyChanged(nameof(Value));
					break;
				case nameof(MaxValue):
					RaisePropertyChanged(nameof(DisplayMaxValue));
					RaisePropertyChanged(nameof(Value));
					break;
				case nameof(NumberType):
					RaisePropertyChanged(nameof(Value));
					break;
			}
		}


		private void UpdateUI() {
			float valueRatio = (Value - MinValue) / (MaxValue - MinValue);
			float fullWidth = (float)ActualWidth;

			float lineWidth = Mathf.Max(0, (float)(valueRatio * (fullWidth - CircleButton.Width)));

			CircleButton.Margin = new Thickness(lineWidth, 0d, 0d, 0d);
			ForeLine.Width = lineWidth;
			ValueTextBlock.Margin = new Thickness(lineWidth, 0d, 0d, 0d);
		}

	}
}
