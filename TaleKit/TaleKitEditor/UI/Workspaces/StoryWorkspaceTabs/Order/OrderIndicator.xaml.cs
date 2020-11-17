using GKitForWPF;
using GKitForWPF.Graphics;
using GKitForWPF.UI.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using TaleKit.Datas.Story;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class OrderIndicator : UserControl {
		public static readonly DependencyProperty IsCountVisibleProperty = DependencyProperty.RegisterAttached(nameof(IsCountVisible), typeof(bool), typeof(OrderIndicator), new PropertyMetadata(true));
		public static readonly DependencyProperty OrderTypeProperty = DependencyProperty.RegisterAttached(nameof(OrderType), typeof(OrderType), typeof(OrderIndicator), new PropertyMetadata(OrderType.UI));
		public static readonly DependencyProperty CountProperty = DependencyProperty.RegisterAttached(nameof(Count), typeof(int), typeof(OrderIndicator), new PropertyMetadata(0));

		public bool IsCountVisible {
			get {
				return (bool)GetValue(IsCountVisibleProperty);
			}
			set {
				SetValue(IsCountVisibleProperty, value);
			}
		}
		public OrderType OrderType {
			get {
				return (OrderType)GetValue(OrderTypeProperty);
			}
			set {
				SetValue(OrderTypeProperty, value);
			}
		}
		public int Count {
			get {
				return (int)GetValue(CountProperty);
			}
			set {
				SetValue(CountProperty, value);
			}
		}

		public OrderIndicator() {
			InitializeComponent();
			InitBindings();
		}
		private void InitBindings() {
			CountTextBlock.SetBinding(TextBlock.VisibilityProperty, new Binding(nameof(IsCountVisible)) { Source = this, Mode = BindingMode.OneWay, Converter=new BoolToVisibilityConverter() });
			IndicatorShape.SetBinding(Border.BackgroundProperty, new Binding(nameof(OrderType)) { Source = this, Mode = BindingMode.OneWay, Converter = new OrderTypeToBrushConverter() });
			CountTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Count)) { Source = this, Mode = BindingMode.OneWay });
		}
	}

	public class OrderTypeToBrushConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			OrderType orderType = (OrderType)value;

			string colorCode;
			switch (orderType) {
				default:
				//case OrderType.Message:
				//	colorCode = "B5B5B5";
				//	break;
				case OrderType.UI:
					colorCode = "B5B5B5";
					break;
				case OrderType.Logic:
					colorCode = "468CF2";
					break;
				case OrderType.Event:
					colorCode = "C853D1";
					break;
			}
			return colorCode.ToBrush();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
