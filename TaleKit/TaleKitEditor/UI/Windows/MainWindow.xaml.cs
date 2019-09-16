﻿using System;
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
using TaleKitEditor.UI.Workspaces;
using GKit;
using GKit.WPF;
using TaleKit.Datas;
using TaleKitEditor.UI.Controls;
using PenMotionEditor.UI.Tabs;

namespace TaleKitEditor.UI.Windows {
	public partial class MainWindow : Window {
		//Workspaces
		public UiWorkspace uiWorkspace {
			get; private set;
		}
		public MotionEditorContext motionWorkspace {
			get; private set;
		}
		public StoryWorkspace storyWorkspace {
			get; private set;
		}
		private Tuple<UserControl, WorkspaceButton>[] workspacePairs;

		//Datas
		public TaleData EditingData {
			get; private set;
		}

		public event Action<TaleData> DataLoaded;
		public event Action<TaleData> DataUnloaded;

		public MainWindow() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			ContentRendered += MainWindow_ContentRendered;
		}

		private void MainWindow_ContentRendered(object sender, EventArgs e) {
			InitData();
			InitWorkspaces();
			RegisterEvents();

			CreateData();
		}

		private void InitWorkspaces() {
			uiWorkspace = new UiWorkspace();
			motionWorkspace = MotionEditorContext;
			storyWorkspace = new StoryWorkspace();

			workspacePairs = new Tuple<UserControl, WorkspaceButton>[] {
				new Tuple<UserControl, WorkspaceButton>(uiWorkspace, UiWorkspaceButton),
				new Tuple<UserControl, WorkspaceButton>(motionWorkspace, MotionWorkspaceButton),
				new Tuple<UserControl, WorkspaceButton>(storyWorkspace, StoryWorkspaceButton),
			};

			foreach(Tuple<UserControl, WorkspaceButton> workspacePair in workspacePairs) {
				if (workspacePair.Item1.Parent == null) {
					WorkspaceContext.Children.Add(workspacePair.Item1);
				}
			}

			ActiveWorkspace(WorkspaceType.Story);
		}
		private void InitData() {
		}
		private void RegisterEvents() {

		}

		public void CreateData() {
			motionWorkspace.CreateFile();
			EditingData = new TaleData();

			DataLoaded?.Invoke(EditingData);

			EditingData.PostInit();
		}
		public void UnloadData() {
			DataUnloaded?.Invoke(EditingData);

			EditingData = null;
		}

		public void ActiveWorkspace(WorkspaceType type) {
			DeactiveWorkspaces();

			Tuple<UserControl, WorkspaceButton> workspacePair = workspacePairs[(int)type];
			
			workspacePair.Item1.Visibility = Visibility.Visible;
			workspacePair.Item2.IsActiveWorkspace = true;
		}
		private void DeactiveWorkspaces() {
			Tuple<UserControl, WorkspaceButton> workspacePair;
			for (int i = 0; i < workspacePairs.Length; ++i) {
				workspacePair = workspacePairs[i];
				workspacePair.Item1.Visibility = Visibility.Collapsed;
				workspacePair.Item2.IsActiveWorkspace = false;
			}
		}

		private void UiWorkspaceButton_Click(object sender, RoutedEventArgs e) {
			ActiveWorkspace(WorkspaceType.Ui);
		}
		private void MotionWorkspaceButton_Click(object sender, RoutedEventArgs e) {
			ActiveWorkspace(WorkspaceType.Motion);
		}
		private void StoryWorkspaceButton_Click(object sender, RoutedEventArgs e) {
			ActiveWorkspace(WorkspaceType.Story);
		}
	}
}
