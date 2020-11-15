using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using GKitForWPF;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.ModelEditor;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// ComponentHeader.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class OrderItemEditorView : UserControl {

		private OrderBase order;

		public string OrderTypeText => order == null ? null : order.orderType.ToString();

		[Obsolete]
		internal OrderItemEditorView() {
			InitializeComponent();
		}
		public OrderItemEditorView(OrderBase order) {
			this.order = order;

			InitializeComponent();
			Indicator.IsCountVisible = false;
			Indicator.OrderType = order.orderType;

			ModelEditorUtility.CreateOrderEditorView(order, ValueEditorViewContext);

			// Register events
			DeleteButton.RegisterClickEvent(DeleteButton_Click, true);
		}

		private void DeleteButton_Click() {
			order.OwnerBlock.RemoveOrder(order);
		}
	}
}
