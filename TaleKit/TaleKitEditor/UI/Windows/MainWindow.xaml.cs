using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using PenMotionEditor.UI.Tabs;
using TaleKit.Datas;
using TaleKitEditor.UI.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using TaleKitEditor.Workspaces;
using TaleKitEditor.UI.Dialogs;
using TaleKitEditor.UI.Utility;
using TaleKitEditor.UI.Workspaces;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using TaleKitEditor.Workspaces.Tabs;
using System.IO;
using GKitForWPF;

namespace TaleKitEditor.UI.Windows {
	public partial class MainWindow : Window {
		private WorkspaceComponent[] workspaces;

		//Datas
		public TaleData EditingTaleData {
			get; private set;
		}

		//CommonTabs
		private UserControl[] commonTabs;
		public ViewportTab ViewportTab {
			get; private set;
		}
		public AssetTab AssetTab {
			get; private set;
		}
		public DetailTab DetailTab {
			get; private set;
		}

		//PostInitTabs
		private INeedPostInitTab[] needPostInitTabs;

		public event Action<WorkspaceComponent> WorkspaceActived;
		public event Action<TaleData> ProjectLoaded;
		public event Action<TaleData> ProjectUnloaded;

		public MainWindow() {
			if (this.IsDesignMode()) {
				InitializeComponent();
			}
		}
		public void Initialize() {
			InitializeComponent();
			ContentRendered += MainWindow_ContentRendered;

			this.SetFocusableWindow();
		}

		private void MainWindow_ContentRendered(object sender, EventArgs e) {
			InitWorkspaces();
			RegisterEvents();

			ActiveWorkspace(WorkspaceType.Ui);

			ProcessDebugTask();
		}

		private void InitWorkspaces() {
			//워크스페이스 생성
			workspaces = new WorkspaceComponent[] {
				new WorkspaceComponent(WorkspaceType.Ui, UiWorkspace, UiWorkspaceButton),
				new WorkspaceComponent(WorkspaceType.Motion, MotionWorkspace, MotionWorkspaceButton),
				new WorkspaceComponent(WorkspaceType.Story, StoryWorkspace, StoryWorkspaceButton),
				new WorkspaceComponent(WorkspaceType.ProjectSetting, SettingWorkspace, SettingWorkspaceButton),
			};

			foreach(WorkspaceComponent workspace in workspaces) {
				if (workspace.context.Parent == null) {
					WorkspaceContext.Children.Add(workspace.context);
				}
			}

			WorkspaceContext.Visibility = Visibility.Collapsed;

			//공통 탭 설정
			AssetTab = UiWorkspace.AssetTab;
			ViewportTab = UiWorkspace.ViewportTab;
			DetailTab = UiWorkspace.DetailTab;
			commonTabs = new UserControl[] {
				AssetTab,
				ViewportTab,
				DetailTab,
			};

			//Invoke PostInit
			needPostInitTabs = new INeedPostInitTab[] {
				DetailTab,
			};
			foreach(INeedPostInitTab tab in needPostInitTabs) {
				tab.PostInit();
			}
		}
		private void RegisterEvents() {
			FileManagerBar.CreateFileButtonClick += CreateProject;
			FileManagerBar.OpenFileButtonClick += OpenFile;
			FileManagerBar.SaveFileButtonClick += SaveFile;
			FileManagerBar.ExportButtonClick += ExportData;
		}

		private async void ProcessDebugTask() {
#if DEBUG
			const string ProjectPath = @"X:\Dropbox\WorkDesk\A_Unity\2019\20190209_ProjectV\Develop\TaleKit\TestProject";
			if (!Directory.Exists(ProjectPath))
				return;

			CreateProject(ProjectPath);
#endif
		}

		public void CreateProject() {
			if (!ShowCheckSaveDialog())
				return;

			CommonOpenFileDialog dialog = new CommonOpenFileDialog() {
				IsFolderPicker = true,
			};
			if (dialog.ShowDialog(this) != CommonFileDialogResult.Ok)
				return;

			CreateProject(dialog.FileName);
		}
		public void CreateProject(string projectDir) {
			try {
				EditingTaleData = new TaleData(projectDir, true);

				EditingTaleData.MotionFile.SetMotionFileData(MotionWorkspace.EditorContext.EditingFile);
			} catch (Exception ex) {
				MessageBox.Show("파일을 여는 데 실패했습니다.");
				return;
			}

			ProjectLoaded?.Invoke(EditingTaleData);

			EditingTaleData.PostInit();

			WorkspaceContext.Visibility = Visibility.Visible;
		}
		public void CloseFile() {

			ProjectUnloaded?.Invoke(EditingTaleData);

			EditingTaleData.Dispose();
			EditingTaleData = null;
		}
		public void OpenFile() {
			if (!ShowCheckSaveDialog())
				return;

			OpenFileDialog dialog = new OpenFileDialog();
			if (!dialog.ShowDialog(this).HasTrueValue())
				return;

			
		}
		public void SaveFile() {
			if(string.IsNullOrEmpty(EditingTaleData.ProjectDir)) {
				CommonOpenFileDialog dialog = new CommonOpenFileDialog() {
					IsFolderPicker = true,
				};

				if (dialog.ShowDialog(this) != CommonFileDialogResult.Ok)
					return;

				EditingTaleData.SetProjectDir(dialog.FileName);
			}

			EditingTaleData.Save();
		}
		public void ExportData() {
			if (!ShowCheckSaveDialog())
				return;

			SaveFileDialog dialog = new SaveFileDialog();

			if (!dialog.ShowDialog(this).HasTrueValue())
				return;

			EditingTaleData.Export(dialog.FileName);
		}
		public bool ShowCheckSaveDialog() {
			return true;
		}

		public void ActiveWorkspace(WorkspaceType type) {
			DeactiveWorkspaces();

			WorkspaceComponent workspace = workspaces[(int)type];
			
			workspace.context.Visibility = Visibility.Visible;
			workspace.button.IsActiveWorkspace = true;

			for (int i = 0; i < commonTabs.Length; ++i) {
				AttachTab(commonTabs[i], workspace);
			}

			WorkspaceActived?.Invoke(workspace);
		}
		private void DeactiveWorkspaces() {
			DetailTab.DeactiveDetailPanel();

			WorkspaceComponent workspace;
			for (int workspaceI = 0; workspaceI < workspaces.Length; ++workspaceI) {
				workspace = workspaces[workspaceI];
				workspace.context.Visibility = Visibility.Collapsed;
				workspace.button.IsActiveWorkspace = false;

				for(int tabI=0; tabI< commonTabs.Length; ++tabI) {
					DetachTab(commonTabs[tabI], workspace);
				}
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
		private void SettingWorkspaceButton_Click(object sender, RoutedEventArgs e) {
			ActiveWorkspace(WorkspaceType.ProjectSetting);
		}

		private void AttachTab(UserControl tab, WorkspaceComponent workspace) {
			// Workspace에 {Name}Context Panel이 존재하면 Reflection을 사용해 붙인다.

			string tabName = tab.GetType().Name;
			string tabContextName = tabName + "Context";

			FieldInfo tabContextMember = workspace.context.GetType().GetField(tabContextName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (tabContextMember != null) {
				LayoutAnchorable tabContext = (LayoutAnchorable)tabContextMember.GetValue(workspace.context);

				tabContext.Content = tab;
			}
		}
		private void DetachTab(UserControl tab, WorkspaceComponent workspace) {
			string tabName = tab.GetType().Name;
			string tabContextName = tabName + "Context";

			FieldInfo tabContextMember = workspace.context.GetType().GetField(tabContextName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(tabContextMember != null) {
				LayoutAnchorable tabContext = (LayoutAnchorable)tabContextMember.GetValue(workspace.context);

				tabContext.Content = null;
			}
		}
	}
}
