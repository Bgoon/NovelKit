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
using TaleKit.Datas;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.StoryBoardElements;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;
using GKitForWPF;
using GKitForWPF.UI.Controls;
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements;
using TaleKitEditor.UI.Workspaces.CommonTabs;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// StoryBoard.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static StoryFile EditingStoryFile => EditingTaleData.StoryFile;
		private static UiFile EditingUiFile => EditingTaleData.UiFile;
		private static ViewportTab ViewportTab => MainWindow.ViewportTab;

		private Dictionary<StoryBlockBase, StoryBlockItemView> dataToViewDict;
		public StoryBlockItemView SelectedBlockViewSingle {
			get {
				if (StoryBlockTreeView.SelectedItemSet.Count > 0) {
					return (StoryBlockItemView)StoryBlockTreeView.SelectedItemSet.Last();
				}
				return null;
			}
		}
		public StoryBlock SelectedBlockSingle {
			get {
				StoryBlockItemView selectedItemView = SelectedBlockViewSingle;
				return selectedItemView == null ? null : selectedItemView.Data as StoryBlock;
			}
		}

		public StoryClip EditingClip {
			get; private set;
		}

		// [ Constructor ]
		public StoryBlockTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			// Init members
			dataToViewDict = new Dictionary<StoryBlockBase, StoryBlockItemView>();

			// Register events
			StoryBlockListController.CreateItemButtonClick += StoryBlockListController_CreateItemButtonClick;
			StoryBlockListController.RemoveItemButtonClick += StoryBlockListController_RemoveItemButtonClick;

			StoryBlockTreeView.ItemMoved += StoryBlockListView_ItemMoved;
			StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
			
			MainWindow.ProjectLoaded += MainWindow_DataLoaded;
			MainWindow.ProjectUnloaded += MainWindow_DataUnloaded;
		}



		// [ Event ]
		private void MainWindow_DataLoaded(TaleData obj) {
			EditingStoryFile.ItemCreated += StoryFile_ItemCreated;
			EditingStoryFile.ItemRemoved += StoryFile_ItemRemoved;

			EditingClip = EditingStoryFile.RootClip;
		}
		private void MainWindow_DataUnloaded(TaleData obj) {
			EditingStoryFile.ItemCreated -= StoryFile_ItemCreated;
			EditingStoryFile.ItemRemoved -= StoryFile_ItemRemoved;
		}

		private void StoryBlockListController_CreateItemButtonClick() {
			EditingStoryFile.CreateStoryBlockItem(EditingClip);
		}
		private void StoryBlockListController_RemoveItemButtonClick() {
			foreach (StoryBlockItemView itemView in StoryBlockTreeView.SelectedItemSet) {
				StoryBlockBase data = itemView.Data;

				EditingStoryFile.RemoveStoryBlockItem(data);
			}
		}

		private void StoryFile_ItemCreated(StoryBlockBase item, StoryClip parentItem) {
			if (parentItem == null)
				return;
			if (parentItem != EditingClip)
				return;

			//Create view
			StoryBlockItemView itemView = new StoryBlockItemView(item);
			StoryBlockTreeView.ChildItemCollection.Add(itemView);
			itemView.ParentItem = StoryBlockTreeView;

			dataToViewDict.Add(item, itemView);
		}
		private void StoryFile_ItemRemoved(StoryBlockBase item, StoryClip parentItem) {
			//Remove view
			dataToViewDict[item].DetachParent();
			//StoryFile.RemoveStoryBlockItem(item);

			dataToViewDict.Remove(item);
		}

		private void StoryBlockListView_ItemMoved(ITreeItem itemView, ITreeFolder oldParentView, ITreeFolder newParentView, int index) {
			//Data에 적용하기
			StoryBlockBase item = ((StoryBlockItemView)itemView).Data;

			EditingClip.RemoveChildItem(item);
			EditingClip.InsertChildItem(index, item);
		}

		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			OnStoryBlockSelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			OnStoryBlockSelectionChanged();
		}
		private void OnStoryBlockSelectionChanged() {
			ApplyOrderToSelection();
		}

		// [ Control ]
		public void ApplyOrderToSelection() {
			if(StoryBlockTreeView.SelectedItemSet.Count == 1) {
				StoryBlockBase selectedBlockBase = (StoryBlockTreeView.SelectedItemSet.First as StoryBlockItemView).Data;
				int selectedBlockIndex = EditingStoryFile.RootClip.ChildItemList.IndexOf(selectedBlockBase);

				ApplyOrders(selectedBlockIndex);
			} else {
				ApplyOrders(-1);
			}
		}
		public void ApplyOrders(int lastIndex) {
			UiRenderer rootRenderer = EditingUiFile.Item_To_ViewDict[EditingUiFile.RootUiItem] as UiRenderer;
			rootRenderer.Render(true);

			HashSet<UiRenderer> renderedRendererHashSet = new HashSet<UiRenderer>();	

			for (int i = 0; i <= lastIndex; ++i) {
				StoryBlockBase blockBase = EditingStoryFile.RootClip.ChildItemList[i];
				switch (blockBase.Type) {
					case StoryBlockType.StoryBlock:
						foreach (OrderBase order in (blockBase as StoryBlock).OrderList) {
							if (order.OrderType == OrderType.UI) {
								Order_UI order_UI = order as Order_UI;
								UiItemBase UiItem = EditingUiFile.UiItemList.Where(x => x.name == order_UI.targetUiName).FirstOrDefault();
								if (UiItem != null) {
									UiRenderer renderer = EditingUiFile.Item_To_ViewDict[UiItem] as UiRenderer;
									if(!renderedRendererHashSet.Contains(renderer)) {
										renderedRendererHashSet.Add(renderer);

										renderer.Render();
									}

									if(ViewportTab.PlayStateButton.IsActive) {
										renderer.RenderFromData(order_UI.UiKeyData);
									}
								}
							}
						}
						break;
					case StoryBlockType.StoryClip:
						break;
				}
			}
		}

	}
}
