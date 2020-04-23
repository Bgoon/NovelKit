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
using GKit;
using GKit.WPF;
using TaleKit.Datas.Editor;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.ValueEditors;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// ComponentHeader.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class OrderItemView : UserControl {

		private OrderBase order;

		public string OrderTypeText => order == null ? null : order.OrderType.ToString();

		public OrderItemView() {
			InitializeComponent();
			Init();
		}
		public OrderItemView(OrderBase order) {
			this.order = order;

			InitializeComponent();
			Init();

			CreateValueEditors();
		}
		private void Init() {
			DataContext = this;

			Indicator.IsCountVisible = false;
		}

		private void CreateValueEditors() {
			Type modelType = order.GetType();
			FieldInfo[] fields = modelType.GetFields();

			foreach (FieldInfo field in fields) {
				ValueEditorAttribute editorAttribute = field.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;

				if (editorAttribute == null)
					continue;

				ValueEditorView valueEditorView = new ValueEditorView(order, field);
				ValueEditorViewContext.Children.Add(valueEditorView);
			}
		}
	}
}
