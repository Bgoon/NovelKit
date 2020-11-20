using GKitForWPF;
using GKitForWPF.UI.Controls;
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
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.UI.Dialogs;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements;
using TaleKitEditor.Workspaces;

namespace TaleKitEditor.UI.Workspaces.UIWorkspaceTabs {
	
	
	public partial class UIOutlinerTab : UserControl {
		public delegate void ItemMovedDelegate(UIItemBase item, UIItemBase newParentItem, UIItemBase oldParentItem, int index);

		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UIFile EditingUIFile => MainWindow.EditingTaleData.UIFile;
		private static DetailTab DetailTab => MainWindow.DetailTab;
		private static ViewportTab ViewportTab => MainWindow.ViewportTab;
		private static CommonDetailPanel CommonDetailPanel => DetailTab.CommonDetailPanel;

		public UIItemView RootItemView {
			get; private set;
		}

		// SelectedItem
		public UIItemView SelectedUIItemViewSingle {
			get {
				if (UITreeView.SelectedItemSet.Count > 0) {
					return (UIItemView)UITreeView.SelectedItemSet.Last();
				}
				return null;
			}
		}
		public UIItemBase SelectedUIItemSingle {
			get {
				UIItemView selectedItemView = SelectedUIItemViewSingle;
				return selectedItemView == null ? null : selectedItemView.Data;
			}
		}

		public event ItemMovedDelegate ItemMoved;

		private List<UIRenderer> focusedRendererList;

		// [ Constructor ]
		public UIOutlinerTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			// Init member
			focusedRendererList = new List<UIRenderer>();

			UITreeView.AutoApplyItemMove = false;

			// Register events
			UIItemListController.CreateItemButtonClick += UIItemListController_CreateItemButtonClick;
			UIItemListController.RemoveItemButtonClick += UIItemListController_RemoveItemButtonClick;

			UITreeView.ItemMoved += UITreeView_ItemMoved;

			MainWindow.ProjectPreloaded += MainWindow_ProjectPreloaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;
			MainWindow.WorkspaceActived += MainWindow_WorkspaceActived;

			UITreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			UITreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}

		private void MainWindow_WorkspaceActived(WorkspaceComponent workspace) {
			switch(workspace.type) {
				case WorkspaceType.UI:
					

					break;
				default:
					ClearFocusBoxVisible();
					break;
			}
		}

		// [ Event ]
		private void MainWindow_ProjectPreloaded(TaleData obj) {
			EditingUIFile.ItemCreatedPreview += UIFile_ItemCreatedPreview;
			EditingUIFile.ItemRemoved += UIFile_ItemRemoved;
		}
		private void MainWindow_ProjectUnloaded(TaleData obj) {
			EditingUIFile.ItemCreatedPreview -= UIFile_ItemCreatedPreview;
			EditingUIFile.ItemRemoved -= UIFile_ItemRemoved;
		}

		private void UIItemListController_CreateItemButtonClick() {
			// Show UIItemType menu
			Dialogs.MenuItem[] menuItems = ((UIItemType[])Enum.GetValues(typeof(UIItemType))).Select(
				(UIItemType itemType) => {
					UIItemType itemTypeInstance = itemType;
					return new Dialogs.MenuItem(itemType.ToString(), () => {
						CreateAndSelectUIItem(itemType);
					});
				}).ToArray();

			MenuPanel.ShowDialog(menuItems);

			void CreateAndSelectUIItem(UIItemType itemType) {
				UIItemBase parentItem = SelectedUIItemSingle ?? EditingUIFile.UISnapshot.rootUIItem;
				UIItemBase item = EditingUIFile.CreateUIItem(parentItem, itemType);

				UITreeView.SelectedItemSet.SetSelectedItem(item.View as ITreeItem);
			}
			
		}
		private void UIItemListController_RemoveItemButtonClick() {
			UIItemView[] selectedItems = UITreeView.SelectedItemSet.Select(item=>(UIItemView)item).ToArray();
			foreach (UIItemView itemView in selectedItems) {
				UIItemBase data = itemView.Data;

				EditingUIFile.RemoveUIItem(data);
			}
		}

		private void UIFile_ItemCreatedPreview(UIItemBase item, UIItemBase parentItem) {
			// 생성된 UIItem의 부모관계가 정해지기 전에 먼저 UIItemView 를 생성한다.
			// (Data_ChildInserted 이벤트에서 UIItemView를 필요로 하기 때문)

			UIItemView itemView = new UIItemView(item);

			if (parentItem == null) {
				//Create root
				itemView.SetRootItem();
				RootItemView = itemView;

				UITreeView.ChildItemCollection.Add(itemView);
				UITreeView.ManualRootFolder = itemView;
			} else {
				itemView.ParentItem = parentItem.View as ITreeFolder;
			}
			itemView.DisplayName = item.name;
			itemView.ItemTypeName = item.itemType.ToString();

			//Register events
			item.ChildInserted += Data_ChildInserted;
			item.ChildRemoved += Data_ChildRemoved;

			void Data_ChildInserted(int index, UIItemBase childItem) {
				UIItemView childItemView = childItem.View as UIItemView;
				itemView.ChildItemCollection.Insert(index, childItemView);
			}
			void Data_ChildRemoved(UIItemBase childItem, UIItemBase currentItem) {
				UIItemView childItemView = childItem.View as UIItemView;
				itemView.ChildItemCollection.Remove(childItemView);
			}

			UITreeChanged();
		}
		private void UIFile_ItemRemoved(UIItemBase item, UIItemBase parentItem) {
			//Remove view	
			UIItemView itemView = item.View as UIItemView;
			UITreeView.NotifyItemRemoved(itemView);
			itemView.DetachParent();

			UITreeChanged();
		}

		private void UITreeView_ItemMoved(ITreeItem itemView, ITreeFolder oldParentView, ITreeFolder newParentView, int index) {
			//Data에 적용하기
			UIItemBase item = ((UIItemView)itemView).Data;
			UIItemBase newParentItem = ((UIItemView)newParentView).Data;
			UIItemView oldParentItemView = (oldParentView as UIItemView);

			if(oldParentItemView != null) {
				oldParentItemView.Data.RemoveChildItem(item);
			}
			newParentItem.InsertChildItem(index, item);

			ItemMoved?.Invoke(item, newParentItem, oldParentItemView.Data, index);

			UITreeChanged();
		}

		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			OnSelectionChanged();

			UIItemView itemView = item as UIItemView;
			itemView.Data.ModelUpdated += ViewportTab.UIItemDetailPanel_UIItemValueChanged;
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			OnSelectionChanged();
		}

		private void OnSelectionChanged() {
			CommonDetailPanel.DetachModel();
			DetailTab.DeactiveDetailPanel();

			UpdateRendererFocusBoxVisible();

			if (UITreeView.SelectedItemSet.Count != 1)
				return;

			UIItemView itemView = UITreeView.SelectedItemSet.First as UIItemView;
			CommonDetailPanel.AttachModel(itemView.Data);
			DetailTab.ActiveDetailPanel(DetailPanelType.Common);
		}

		private void UITreeChanged() {
			EditingUIFile.OwnerTaleData.StoryFile.UICacheManager.ClearCacheAll();
		}


		private void UpdateRendererFocusBoxVisible() {
			ClearFocusBoxVisible();

			foreach (var item in UITreeView.SelectedItemSet) {
				UIItemView itemView = UITreeView.SelectedItemSet.First as UIItemView;
				UIRenderer renderer = EditingUIFile.Guid_To_RendererDict[itemView.Data.guid] as UIRenderer;

				renderer.SetFocusBoxVisible(true);
				focusedRendererList.Add(renderer);
			}
			// Guidline
		}
		private void ClearFocusBoxVisible() {
			foreach (UIRenderer renderer in focusedRendererList) {
				renderer.SetFocusBoxVisible(false);
			}

			focusedRendererList.Clear();
		}
	}
}
