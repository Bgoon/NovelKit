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
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Workspaces.UiWorkspaceTabs {
	public delegate void ItemMovedDelegate(UiItem item, UiItem newParentItem, UiItem oldParentItem);
	
	public partial class UiOutlinerTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiFile UiFile => MainWindow.EditingTaleData.UiFile;

		public UiItemView RootItemView {
			get; private set;
		}
		private Dictionary<UiItem, UiItemView> dataToViewDict;

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
			dataToViewDict = new Dictionary<UiItem, UiItemView>();

			UiTreeView.AutoApplyItemMove = false;
		}
		private void RegisterEvents() {
			UiItemListController.CreateItemButtonClick += UiItemListController_CreateItemButtonClick;
			UiItemListController.RemoveItemButtonClick += UiItemListController_RemoveItemButtonClick;

			UiTreeView.ItemMoved += UiTreeView_ItemMoved;

			MainWindow.ProjectLoaded += MainWindow_DataLoaded;
			MainWindow.ProjectUnloaded += MainWindow_DataUnloaded;
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
			UiFile.CreateUiItem(SelectedUiItemSingle);
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
				itemView.ParentItem = dataToViewDict[parentItem];
			}
			//Add to collection
			dataToViewDict.Add(item, itemView);

			//Register events
			item.ChildInserted += Data_ChildInserted;
			item.ChildRemoved += Data_ChildRemoved;

			void Data_ChildInserted(int index, UiItem childItem) {
				UiItemView childItemView = dataToViewDict[childItem];
				itemView.ChildItemCollection.Insert(index, childItemView);
			}
			void Data_ChildRemoved(UiItem childItem, UiItem currentItem) {
				UiItemView childItemView = dataToViewDict[childItem];
				itemView.ChildItemCollection.Remove(childItemView);
			}
		}
		private void UiFile_ItemRemoved(UiItem item, UiItem parentItem) {
			//Remove view	
			UiItemView itemView = dataToViewDict[item];
			UiTreeView.NotifyItemRemoved(itemView);
			itemView.DetachParent();
			dataToViewDict.Remove(item);
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
	}
}
