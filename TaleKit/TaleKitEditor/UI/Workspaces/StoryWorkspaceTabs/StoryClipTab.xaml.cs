﻿using GKitForWPF;
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
		private static UIFile EditingUIFile => EditingTaleData.UIFile;
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

			MainWindow.ProjectPreloaded += MainWindow_ProjectPreloaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;

			StoryClipTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryClipTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}



		// [ Event ]
		private void MainWindow_ProjectPreloaded(TaleData taleData) {
			EditingStoryFile.ClipCreated += EditingStoryFile_ClipCreated;
			EditingStoryFile.ClipRemoved += EditingStoryFile_ClipRemoved;
		}
		private void MainWindow_ProjectUnloaded(TaleData taleData) {
			EditingStoryFile.ClipCreated += EditingStoryFile_ClipCreated;
			EditingStoryFile.ClipRemoved += EditingStoryFile_ClipRemoved;
		}

		private void CreateItemButton_Click() {
			EditingStoryFile.CreateStoryClip();
		}
		private void RemoveItemButton_Click() {
			foreach(var item in StoryClipTreeView.SelectedItemSet) {
				StoryClipView clipView = item as StoryClipView;

				EditingStoryFile.RemoveStoryClip(clipView.Data);
			}
		}

		private void EditingStoryFile_ClipCreated(StoryClip clip) {
			StoryClipView clipView = new StoryClipView(clip);
			StoryClipTreeView.ChildItemCollection.Add(clipView);
			clipView.ParentItem = StoryClipTreeView;
			clipView.UpdateNameText();

			dataToViewDict.Add(clip, clipView);

			// Register events
			clipView.NameEditText.TextEdited += ClipViewNameEditText_TextEdited;

			void ClipViewNameEditText_TextEdited(string oldText, string newText, ref bool cancelEdit) {
				if (string.IsNullOrEmpty(newText)) {
					cancelEdit = true;
					return;
				}

				clip.name = newText;

				StoryBlockTab.UpdateBlockPreviews();
			}
		}
		private void EditingStoryFile_ClipRemoved(StoryClip clip) {
			dataToViewDict[clip].DetachParent();

			dataToViewDict.Remove(clip);
		}

		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			SelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			SelectionChanged();
		}
		private void SelectionChanged() {
			StoryClipView lastSelectedClipView = StoryClipTreeView.SelectedItemSet.LastOrDefault() as StoryClipView;
			StoryClip lastSelectedClip;
			if(lastSelectedClipView == null) {
				lastSelectedClip = EditingStoryFile.RootClip;
			} else {
				lastSelectedClip = lastSelectedClipView.Data;
			}

			StoryBlockTab.AttachClip(lastSelectedClip);
		}
	}
}
