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
using TaleKit.Datas.Editor;
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs;

namespace TaleKitEditor.UI.Workspaces.UiWorkspaceTabs {
	public delegate void ItemMovedDelegate(UiItem item, UiItem newParentItem, UiItem oldParentItem);
	
	public partial class UiOutlinerTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiFile UiFile => MainWindow.EditingTaleData.UiFile;
		private static DetailTab DetailTab => MainWindow.DetailTab;
		private static ViewportTab ViewportTab => MainWindow.ViewportTab;
		private static CommonDetailPanel CommonDetailPanel => DetailTab.CommonDetailPanel;

		public UiItemView RootItemView {
			get; private set;
		}

		//Selected
		public UiItemView SelectedUiItemViewSingle {
			get {
				if (UiTreeView.SelectedItemSet.Count > 0) {
					return (UiItemView)UiTreeView.SelectedItemSet.Last();
				}
				return null;
			}
		}
		public UiItem SelectedUiItemSingle {
			get {
				UiItemView selectedItemView = SelectedUiItemViewSingle;
				return selectedItemView == null ? null : selectedItemView.Data;
			}
		}

		public event ItemMovedDelegate ItemMoved;

		public UiOutlinerTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			InitMembers();
			RegisterEvents();
		}
		private void InitMembers() {
			UiTreeView.AutoApplyItemMove = false;
		}
		private void RegisterEvents() {
			UiItemListController.CreateItemButtonClick += UiItemListController_CreateItemButtonClick;
			UiItemListController.RemoveItemButtonClick += UiItemListController_RemoveItemButtonClick;

			UiTreeView.ItemMoved += UiTreeView_ItemMoved;

			MainWindow.ProjectLoaded += MainWindow_DataLoaded;
			MainWindow.ProjectUnloaded += MainWindow_DataUnloaded;

			UiTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			UiTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}


		//Events
		private void MainWindow_DataLoaded(TaleData obj) {
			UiFile.ItemCreatedPreview += UiFile_ItemCreated;
			UiFile.ItemRemoved += UiFile_ItemRemoved;

			UiFile_ItemCreated(UiFile.RootUiItem, null);
		}
		private void MainWindow_DataUnloaded(TaleData obj) {
			UiFile.ItemCreatedPreview -= UiFile_ItemCreated;
			UiFile.ItemRemoved -= UiFile_ItemRemoved;

			UiFile_ItemRemoved(UiFile.RootUiItem, null);
		}

		private void UiItemListController_CreateItemButtonClick() {
			UiItem item = UiFile.CreateUiItem(SelectedUiItemSingle);

			UiTreeView.SelectedItemSet.SetSelectedItem(item.View as ITreeItem);
		}
		private void UiItemListController_RemoveItemButtonClick() {
			UiItemView[] selectedItems = UiTreeView.SelectedItemSet.Select(item=>(UiItemView)item).ToArray();
			foreach (UiItemView itemView in selectedItems) {
				UiItem data = itemView.Data;

				UiFile.RemoveUiItem(data);
			}
		}

		private void UiFile_ItemCreated(UiItem item, UiItem parentItem) {
			UiItemView itemView = new UiItemView(item);

			if (parentItem == null) {
				//Create root
				itemView.SetRootItem();
				RootItemView = itemView;

				UiTreeView.ChildItemCollection.Add(itemView);
				UiTreeView.ManualRootFolder = itemView;
			} else {
				itemView.ParentItem = parentItem.View as ITreeFolder;
			}

			//Register events
			item.ChildInserted += Data_ChildInserted;
			item.ChildRemoved += Data_ChildRemoved;

			void Data_ChildInserted(int index, UiItem childItem) {
				UiItemView childItemView = childItem.View as UiItemView;
				itemView.ChildItemCollection.Insert(index, childItemView);
			}
			void Data_ChildRemoved(UiItem childItem, UiItem currentItem) {
				UiItemView childItemView = childItem.View as UiItemView;
				itemView.ChildItemCollection.Remove(childItemView);
			}
		}
		private void UiFile_ItemRemoved(UiItem item, UiItem parentItem) {
			//Remove view	
			UiItemView itemView = item.View as UiItemView;
			UiTreeView.NotifyItemRemoved(itemView);
			itemView.DetachParent();
		}

		private void UiTreeView_ItemMoved(ITreeItem itemView, ITreeFolder oldParentView, ITreeFolder newParentView, int index) {
			//Data에 적용하기
			UiItem item = ((UiItemView)itemView).Data;
			UiItem newParentItem = ((UiItemView)newParentView).Data;
			UiItemView oldParentItemView = (oldParentView as UiItemView);

			if(oldParentItemView != null) {
				oldParentItemView.Data.RemoveChildItem(item);
			}
			newParentItem.InsertChildItem(index, item);

			ItemMoved?.Invoke(item, newParentItem, oldParentItemView.Data);
		}

		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			OnSelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			OnSelectionChanged();
		}

		private void OnSelectionChanged() {
			CommonDetailPanel.DetachModel();
			DetailTab.DeactiveDetailPanel();

			if (UiTreeView.SelectedItemSet.Count == 1) {
				UiItemView itemView = UiTreeView.SelectedItemSet.First as UiItemView;
				CommonDetailPanel.AttachModel(itemView.Data as IEditableModel, ViewportTab.UiItemDetailPanel_UiItemValueChanged);
				DetailTab.ActiveDetailPanel(DetailPanelType.Common);
			}
		}
	}
}
