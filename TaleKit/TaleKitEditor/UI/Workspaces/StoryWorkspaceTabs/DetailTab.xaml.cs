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
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs;
using GKit;
using GKit.WPF;
using GKit.WPF.UI;
using GKit.WPF.UI.Controls;
using System.Reflection;
using TaleKit.Datas.Editor;
using TaleKitEditor.UI.ValueEditors;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class DetailTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static StoryWorkspace StoryWorkspace => MainWindow.StoryWorkspace;
		private static StoryBlockTab StoryBlockTab => StoryWorkspace.StoryBlockTab;

		private Dictionary<OrderBase, UserControl> orderControlDict;

		public StoryBlock EditingBlock {
			get; private set;
		}

		private bool isInitiliazed;

		public DetailTab() {
			InitializeComponent();
		}
		private void DetailTab_Loaded(object sender, RoutedEventArgs e) {
			if (this.IsDesignMode() || isInitiliazed)
				return;
			isInitiliazed = true;


			InitMembers();
			RegisterEvents();

			SelectionChanged();
		}
		private void InitMembers() {
			orderControlDict = new Dictionary<OrderBase, UserControl>();
		}
		private void RegisterEvents() {
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}

		//이게 두번 호출되는 버그가 있다
		//그건 바로 Window.Loaded가 두번 호출되는 버그여따.
		private void SelectedItemSet_SelectionAdded(ITreeItem item) {
			SelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(ITreeItem item) {
			SelectionChanged();
		}

		private void AddOrderButton_Click(object sender, RoutedEventArgs e) {
			AddOrderWindow window = new AddOrderWindow(EditingBlock, (Vector2)AddOrderButton.PointToScreen(new Point(10f, 0f)));
			window.ShowNoFlicker();
		}

		private void EditingBlock_OrderAdded(OrderBase order) {
			OrderItemView orderItemView = new OrderItemView(order);

			OrderStackPanel.Children.Add(orderItemView);
		}
		private void EditingBlock_OrderRemoved(OrderBase order) {
			OrderStackPanel.Children.Remove(orderControlDict[order]);
		}

		private void SelectionChanged() {
			SelectedListItemSet selectedItemSet = StoryBlockTab.StoryBlockTreeView.SelectedItemSet;
			bool showEditingContext = selectedItemSet.Count == 1;

			EditingContext.Visibility = showEditingContext ? Visibility.Visible : Visibility.Collapsed;
			MessageContext.Visibility = showEditingContext ? Visibility.Collapsed : Visibility.Visible;

			if (showEditingContext) {
				AttachBlock(StoryBlockTab.SelectedBlockSingle);
			} else {
				string message;
				if (selectedItemSet.Count == 0) {
					message = "편집할 블럭을 선택하세요.";
				} else {
					message = $"{selectedItemSet.Count} 개의 블럭 선택 중";
				}
				MessageTextBlock.Text = message;

				DetachBlock();
			}
		}

		public void AttachBlock(StoryBlock block) {
			DetachBlock();

			this.EditingBlock = block;

			foreach (OrderBase order in block.OrderList) {
				EditingBlock_OrderAdded(order);
			}
			//Register events
			block.OrderAdded += EditingBlock_OrderAdded;
			block.OrderRemoved += EditingBlock_OrderRemoved;
		}
		public void DetachBlock() {
			if (EditingBlock == null)
				return;

			//Remove events
			EditingBlock.OrderAdded -= EditingBlock_OrderAdded;
			EditingBlock.OrderRemoved -= EditingBlock_OrderRemoved;

			this.EditingBlock = null;

			OrderStackPanel.Children.Clear();
		}

	}
}
