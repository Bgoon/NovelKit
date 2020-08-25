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
using System.Reflection;
using TaleKit.Datas.ModelEditor;
using TaleKitEditor.UI.ModelEditor;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.StoryBoardElements;
using GKitForWPF.UI.Controls;
using GKitForWPF;
using TaleKitEditor.UI.Dialogs;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class StoryBlockDetailPanel : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static StoryWorkspace StoryWorkspace => MainWindow.StoryWorkspace;
		private static StoryBlockTab StoryBlockTab => StoryWorkspace.StoryBlockTab;
		private static DetailTab DetailTab => MainWindow.DetailTab;

		private Dictionary<OrderBase, OrderItemEditorView> orderToEditorViewDict;

		public StoryBlock EditingBlock {
			get; private set;
		}

		// [ Constructor ]
		public StoryBlockDetailPanel() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			InitMembers();
			RegisterEvents();

			SelectionChanged();
		}
		private void InitMembers() {
			orderToEditorViewDict = new Dictionary<OrderBase, OrderItemEditorView>();
		}
		private void RegisterEvents() {
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}

		// [ Event ]
		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			SelectionChanged();
			DetailTab.ActiveDetailPanel(DetailPanelType.StoryBlock);
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			SelectionChanged();
			DetailTab.DeactiveDetailPanel();
		}

		private void AddOrderButton_Click(object sender, RoutedEventArgs e) {
			AddOrderPanel.ShowDialog(EditingBlock, (Vector2)AddOrderButton.TranslatePoint(new Point(10f, (float)AddOrderButton.ActualHeight * 0.5f), MainWindow));
		}

		private void EditingBlock_OrderAdded(OrderBase order) {
			OrderItemEditorView editorView = new OrderItemEditorView(order);

			// Add to collection
			orderToEditorViewDict.Add(order, editorView);
			OrderEditorViewContext.Children.Add(editorView);

			// Register events
			order.ModelUpdated += Order_ModelUpdated;
		}

		private void Order_ModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView) {
			StoryBlockTab.ApplyBlockToSelectionToRenderer();
		}

		private void EditingBlock_OrderRemoved(OrderBase order) {
			order.ClearEvents();
			OrderEditorViewContext.Children.Remove(orderToEditorViewDict[order]);

			orderToEditorViewDict.Remove(order);
		}
		private void SelectionChanged() {
			SelectedItemSet selectedItemSet = StoryBlockTab.StoryBlockTreeView.SelectedItemSet;
			bool showEditingContext = selectedItemSet.Count == 1;

			EditorContext.Visibility = showEditingContext ? Visibility.Visible : Visibility.Collapsed;
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

		// [ Control ]
		public void AttachBlock(StoryBlock blockItem) {
			DetachBlock();
			this.EditingBlock = blockItem;

			if (blockItem == null)
				return;

			foreach (OrderBase order in blockItem.OrderList) {
				EditingBlock_OrderAdded(order);
			}
			//Register events
			blockItem.OrderAdded += EditingBlock_OrderAdded;
			blockItem.OrderRemoved += EditingBlock_OrderRemoved;
		}
		public void DetachBlock() {
			if (EditingBlock == null)
				return;

			foreach(OrderBase order in EditingBlock.OrderList) {
				EditingBlock_OrderRemoved(order);
			}

			//Remove events
			EditingBlock.OrderAdded -= EditingBlock_OrderAdded;
			EditingBlock.OrderRemoved -= EditingBlock_OrderRemoved;

			this.EditingBlock = null;

			OrderEditorViewContext.Children.Clear();
		}

	}
}
