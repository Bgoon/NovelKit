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
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views;
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

		public StoryBlockBase EditingBlock {
			get; private set;
		}

		// [ Constructor ]
		public StoryBlockDetailPanel() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			// Init
			orderToEditorViewDict = new Dictionary<OrderBase, OrderItemEditorView>();
			
			// Register events
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;

			SelectionChanged();
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
			AddOrderPanel.ShowDialog(EditingBlock as StoryBlock_Item, (Vector2)AddOrderButton.GetAbsolutePosition(new Point(10f, (float)AddOrderButton.ActualHeight * 0.5f)));
		}

		private void EditingBlock_OrderAdded(OrderBase order) {
			OrderItemEditorView editorView = new OrderItemEditorView(order);

			// Add to collection
			orderToEditorViewDict.Add(order, editorView);
			OrderEditorViewContext.Children.Add(editorView);

			// Register events
			order.ModelUpdated += Order_ModelUpdated;

			ClearCurrentBlockCache();
		}
		private void EditingBlock_OrderRemoved(OrderBase order) {
			order.ClearEvents();
			OrderEditorViewContext.Children.Remove(orderToEditorViewDict[order]);

			orderToEditorViewDict.Remove(order);

			StoryBlockTab.ApplyBlockToSelectionToRenderer();

			ClearCurrentBlockCache();
		}

		private void Order_ModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView) {
			StoryBlockTab.ApplyBlockToSelectionToRenderer();

			ClearCurrentBlockCache();
		}

		private void SelectionChanged() {
			SelectedItemSet selectedItemSet = StoryBlockTab.StoryBlockTreeView.SelectedItemSet;
			bool showEditingContext = selectedItemSet.Count == 1;

			EditorContext.Visibility = showEditingContext ? Visibility.Visible : Visibility.Collapsed;
			MessageContext.Visibility = showEditingContext ? Visibility.Collapsed : Visibility.Visible;

			if (showEditingContext) {
				if (KeyInput.GetKeyHold(WinKey.X))
					return;

				AttachBlock(StoryBlockTab.SelectedBlockViewSingle.Data);
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

		// [ Tree ]
		public void AttachBlock(StoryBlockBase block) {
			DetachBlock();
			this.EditingBlock = block;

			if (block == null)
				return;


			ModelEditorUtility.CreateOrderEditorView(block, StoryBlockEditorViewContext);
			if(block.blockType == StoryBlockType.Item) {
				StoryBlock_Item itemBlock = block as StoryBlock_Item;
				foreach (OrderBase order in itemBlock.OrderList) {
					EditingBlock_OrderAdded(order);
				}
				//Register events
				itemBlock.OrderAdded += EditingBlock_OrderAdded;
				itemBlock.OrderRemoved += EditingBlock_OrderRemoved;

				StoryBlock_ItemEditorContext.Visibility = Visibility.Visible;
			} else if(block.blockType == StoryBlockType.Clip) {
				StoryBlock_ItemEditorContext.Visibility = Visibility.Collapsed;
			}
		}
		public void DetachBlock() {
			if (EditingBlock == null)
				return;

			if(EditingBlock.blockType == StoryBlockType.Item) {
				StoryBlock_Item itemBlock = EditingBlock as StoryBlock_Item;

				foreach (OrderBase order in itemBlock.OrderList) {
					EditingBlock_OrderRemoved(order);
				}

				//Remove events
				itemBlock.OrderAdded -= EditingBlock_OrderAdded;
				itemBlock.OrderRemoved -= EditingBlock_OrderRemoved;
			} else if(EditingBlock.blockType == StoryBlockType.Clip) {
				
			}

			this.EditingBlock = null;

			StoryBlockEditorViewContext.Children.Clear();
			OrderEditorViewContext.Children.Clear();
		}

		private void ClearCurrentBlockCache() {
			EditingBlock.OwnerFile.UiCacheManager.ClearCacheAfterBlock(EditingBlock);
		}
	}
}
