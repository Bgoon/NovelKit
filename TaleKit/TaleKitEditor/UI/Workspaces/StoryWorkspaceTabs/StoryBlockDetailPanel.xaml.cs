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
using TaleKit.Datas;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class StoryBlockDetailPanel : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static StoryFile EditingStoryFile => EditingTaleData.StoryFile;
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
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedBlockSet_SelectionAdded;
			StoryBlockTab.StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedBlockSet_SelectionRemoved;

			SelectedBlockSet_SelectionChanged();
		}

		// [ Event ]
		private void SelectedBlockSet_SelectionAdded(ISelectable item) {
			SelectedBlockSet_SelectionChanged();
			DetailTab.ActiveDetailPanel(DetailPanelType.StoryBlock);
		}
		private void SelectedBlockSet_SelectionRemoved(ISelectable item) {
			SelectedBlockSet_SelectionChanged();
			DetailTab.DeactiveDetailPanel();
		}
		private void SelectedBlockSet_SelectionChanged() {
			SelectedItemSet selectedItemSet = StoryBlockTab.StoryBlockTreeView.SelectedItemSet;
			bool showEditorContext = selectedItemSet.Count == 1;

			SetEditorContextVisible(showEditorContext);

			if (showEditorContext) {
#if DEBUG
				if (KeyInput.GetKeyHold(WinKey.X))
					return;
#endif
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

			void Order_ModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView_) {
				// TODO : 이 함수를 사용해서 RootClip 외엔 Preview가 안 돼므로 방식 변경 필요
				StoryBlockTab.ApplyBlockToSelectionToRenderer();

				// Update preview text
				if (StoryBlockTab.Data_To_ViewDict.ContainsKey(order.OwnerBlock)) {
					StoryBlockView blockView = StoryBlockTab.Data_To_ViewDict[order.OwnerBlock];
					StoryBlockViewContent_Item viewContent_Item = blockView.ViewContent as StoryBlockViewContent_Item;
					viewContent_Item.UpdatePreviewText();
				}

				ClearCurrentBlockCache();
			}
		}
		private void EditingBlock_OrderRemoved(OrderBase order) {
			order.ClearEvents();
			OrderEditorViewContext.Children.Remove(orderToEditorViewDict[order]);

			orderToEditorViewDict.Remove(order);

			StoryBlockTab.ApplyBlockToSelectionToRenderer();

			ClearCurrentBlockCache();
		}

		private void ClipBlock_ModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView) {
			StoryBlock_Clip clipBlock = model as StoryBlock_Clip;

			if(fieldInfo.Name == nameof(clipBlock.targetClipGuid)) {
				StoryBlockView blockView = StoryBlockTab.Data_To_ViewDict[clipBlock];
				StoryBlockViewContent_Clip clipViewContent = blockView.ViewContent as StoryBlockViewContent_Clip;

				clipViewContent.UpdatePreviewText();
			}
		}

		// [ Tree ]
		public void AttachBlock(StoryBlockBase block) {
			DetachBlock();
			this.EditingBlock = block;

			if (block == null)
				return;

			StoryBlockView blockView = StoryBlockTab.Data_To_ViewDict[block];

			ModelEditorUtility.CreateOrderEditorView(block, StoryBlockEditorViewContext);

			if(block.blockType == StoryBlockType.Item) {
				StoryBlock_Item itemBlock = block as StoryBlock_Item;
				foreach (OrderBase order in itemBlock.OrderList) {
					EditingBlock_OrderAdded(order);
				}
				OrderEditorContext.Visibility = Visibility.Visible;

				//Register events
				itemBlock.OrderAdded += EditingBlock_OrderAdded;
				itemBlock.OrderRemoved += EditingBlock_OrderRemoved;
			} else if(block.blockType == StoryBlockType.Clip) {
				OrderEditorContext.Visibility = Visibility.Collapsed;

				block.ModelUpdated += ClipBlock_ModelUpdated;
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

			SetEditorContextVisible(false);
			StoryBlockEditorViewContext.Children.Clear();
			OrderEditorViewContext.Children.Clear();
		}

		private void ClearCurrentBlockCache() {
			EditingBlock.OwnerFile.UICacheManager.ClearCacheAfterBlock(EditingBlock);
		}

		private void SetEditorContextVisible(bool visible) {
			EditorContext.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
			MessageContext.Visibility = visible ? Visibility.Hidden : Visibility.Visible;
		}
	}
}
