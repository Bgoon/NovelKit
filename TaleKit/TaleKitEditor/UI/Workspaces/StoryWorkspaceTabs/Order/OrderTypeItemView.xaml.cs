using GKitForWPF.UI.Converters;
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
using TaleKit.Datas.Story;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// AddOrderItem.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class OrderTypeItemView : UserControl {
		public static readonly DependencyProperty TypeNameTextProperty = DependencyProperty.RegisterAttached(nameof(TypeNameText), typeof(string), typeof(OrderTypeItemView), new PropertyMetadata(null));
		public static readonly DependencyProperty IsSeparatorVisibleProperty = DependencyProperty.RegisterAttached(nameof(IsSeparatorVisible), typeof(bool), typeof(OrderTypeItemView), new PropertyMetadata(true));
		public static readonly DependencyProperty OrderTypeProperty = DependencyProperty.RegisterAttached(nameof(OrderType), typeof(OrderType), typeof(OrderTypeItemView), new PropertyMetadata(OrderType.UI));

		public event Action<OrderType> Click;

		public string TypeNameText {
			get {
				return (string)GetValue(TypeNameTextProperty);
			}
			set {
				SetValue(TypeNameTextProperty, value);
			}
		}
		public bool IsSeparatorVisible {
			get {
				return (bool)GetValue(IsSeparatorVisibleProperty);
			}
			set {
				SetValue(IsSeparatorVisibleProperty, value);
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

		public Type orderType;

		public OrderTypeItemView() {
			InitializeComponent();
			InitBindings();
		}
		private void InitBindings() {
			TypeTextBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(TypeNameText)) { Source = this, Mode = BindingMode.TwoWay });
			ItemSeparator.SetBinding(Separator.VisibilityProperty, new Binding(nameof(IsSeparatorVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = new BoolToVisibilityConverter() });
			OrderIndicator.SetBinding(OrderIndicator.OrderTypeProperty, new Binding(nameof(OrderType)) { Source = this, Mode = BindingMode.OneWay });
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			Click?.Invoke(OrderType);
		}
	}
}
