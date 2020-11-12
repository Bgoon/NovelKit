using GKitForWPF;
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
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class StoryClipTab : UserControl {
		private static Root Root => Root.Instance;
		private static GLoopEngine LoopEngine => Root.LoopEngine;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static StoryFile EditingStoryFile => EditingTaleData.StoryFile;
		private static UiFile EditingUiFile => EditingTaleData.UiFile;
		private static ViewportTab ViewportTab => MainWindow.ViewportTab;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		private readonly Dictionary<StoryClip, StoryClipView> dataToViewDict;

		// [ Constructor ]
		public StoryClipTab() {
			InitializeComponent();

			// Initialize
			dataToViewDict = new Dictionary<StoryClip, StoryClipView>();

			// Register events
			StoryClipListController.CreateItemButtonClick.Add(CreateItemButton_Click);
			StoryClipListController.RemoveItemButtonClick.Add(RemoveItemButton_Click);

			MainWindow.ProjectLoaded += MainWindow_ProjectLoaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;
		}


		private void MainWindow_ProjectLoaded(TaleData taleData) {
			EditingStoryFile.ClipCreated += EditingStoryFile_ClipCreated;
			EditingStoryFile.ClipRemoved += EditingStoryFile_ClipRemoved;
		}
		private void MainWindow_ProjectUnloaded(TaleData taleData) {
			EditingStoryFile.ClipCreated += EditingStoryFile_ClipCreated;
			EditingStoryFile.ClipRemoved += EditingStoryFile_ClipRemoved;
		}

		// [ Event ]
		private void CreateItemButton_Click() {
			EditingStoryFile.CreateStoryClip();
		}
		private void RemoveItemButton_Click() {
			foreach(var item in StoryClipListView.SelectedItemSet) {
				StoryClipView clipView = item as StoryClipView;

				EditingStoryFile.RemoveStoryClip(clipView.Data);
			}
		}

		private void EditingStoryFile_ClipCreated(StoryClip clip) {
			//TODO : 구현	
			StoryClipView clipView = new StoryClipView(clip);
			StoryClipListView.RootFolder.ChildItemCollection.Add(clipView);
			clipView.ParentItem = StoryClipListView;

			dataToViewDict.Add(clip, clipView);
		}
		private void EditingStoryFile_ClipRemoved(StoryClip clip) {
			dataToViewDict[clip].DetachParent();

			dataToViewDict.Remove(clip);
		}
	}
}
