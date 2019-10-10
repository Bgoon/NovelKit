extern alias GKitForUnity;
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
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs.LayerItem;
using GKit.WPF;
using GKit.WPF.UI.Controls;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// StoryBoard.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static StoryFile StoryFile => MainWindow.EditingData.StoryFile;

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
				return SelectedBlockViewSingle.Data as StoryBlock;
			}
		}

		public StoryClip EditingClip {
			get; private set;
		}

		public StoryBlockTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			InitMembers();
			InitEvents();
		}
		private void InitMembers() {
			dataToViewDict = new Dictionary<StoryBlockBase, StoryBlockItemView>();
		}
		private void InitEvents() {
			StoryBlockListController.CreateItemButtonClick += StoryBlockListController_CreateItemButtonClick;
			StoryBlockListController.RemoveItemButtonClick += StoryBlockListController_RemoveItemButtonClick;

			StoryBlockTreeView.ItemMoved += StoryBlockListView_ItemMoved;
			
			MainWindow.DataLoaded += MainWindow_DataLoaded;
			MainWindow.DataUnloaded += MainWindow_DataUnloaded;
		}

		private void MainWindow_DataLoaded(TaleData obj) {
			StoryFile.ItemCreated += StoryFile_ItemCreated;
			StoryFile.ItemRemoved += StoryFile_ItemRemoved;

			EditingClip = StoryFile.RootClip;
		}
		private void MainWindow_DataUnloaded(TaleData obj) {
			StoryFile.ItemCreated -= StoryFile_ItemCreated;
			StoryFile.ItemRemoved -= StoryFile_ItemRemoved;
		}

		private void StoryBlockListController_CreateItemButtonClick(object sender, RoutedEventArgs e) {
			StoryFile.CreateStoryBlockItem(EditingClip);
		}
		private void StoryBlockListController_RemoveItemButtonClick(object sender, RoutedEventArgs e) {
			foreach (StoryBlockItemView itemView in StoryBlockTreeView.SelectedItemSet) {
				StoryBlockBase data = itemView.Data;

				StoryFile.RemoveStoryBlockItem(data);
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


	}
}
