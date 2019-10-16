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

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_Slider : UserControl, INotifyPropertyChanged, IValueEditor {
		public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(NumberType), typeof(ValueEditorElement_Slider), new PropertyMetadata(NumberType.Float));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(float), typeof(ValueEditorElement_Slider), new PropertyMetadata(0f));
		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.RegisterAttached(nameof(DefaultValue), typeof(float), typeof(ValueEditorElement_Slider), new PropertyMetadata(0f));
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.RegisterAttached(nameof(MinValue), typeof(float), typeof(ValueEditorElement_Slider), new PropertyMetadata(0f));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.RegisterAttached(nameof(MaxValue), typeof(float), typeof(ValueEditorElement_Slider), new PropertyMetadata(1f));

		public event PropertyChangedEventHandler PropertyChanged;

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
				return (int)Value;
			}
		}

		public NumberType NumberType {
			get {
				return (NumberType)GetValue(NumberTypeProperty);
			}
			set {
				SetValue(NumberTypeProperty, value);

			}
		}
		public float Value {
			get {
				return (float)GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntValue)));
			}
		}
		public float DefaultValue {
			get {
				return (float)GetValue(DefaultValueProperty);
			}
			set {
				SetValue(DefaultValueProperty, value);
			}
		}
		public float MinValue {
			get {
				return (float)GetValue(MinValueProperty);
			}
			set {
				SetValue(MinValueProperty, value);
			}
		}
		public float MaxValue {
			get {
				return (float)GetValue(MaxValueProperty);
			}
			set {
				SetValue(MaxValueProperty, value);
			}
		}

		public ValueEditorElement_Slider() {
			InitializeComponent();
			PropertyChanged += SliderValueEditor_PropertyChanged;
			SizeChanged += SliderValueEditor_SizeChanged;

			UpdateUI();
		}

		private void RaisePropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void SliderValueEditor_SizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateUI();
		}

		private void SliderValueEditor_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			switch(e.PropertyName) {
				case nameof(Value):
					UpdateUI();
					RaisePropertyChanged(nameof(DisplayValue));
					break;
				case nameof(MinValue):
					RaisePropertyChanged(nameof(DisplayMinValue));
					break;
				case nameof(MaxValue):
					RaisePropertyChanged(nameof(DisplayMaxValue));
					break;
			}
		}

		private void UpdateUI() {
			float valueRatio = (Value - MinValue) / (MaxValue - MinValue);
			float fullWidth = (float)ActualWidth;

			CircleButton.Margin = new Thickness(valueRatio * (fullWidth - CircleButton.Width), 0d, 0d, 0d);
			ForeLine.Width = valueRatio * (fullWidth - CircleButton.Width);
			ValueTextBlock.Margin = new Thickness(valueRatio * (fullWidth - CircleButton.Width), 0d, 0d, 0d);
		}
	}
}
