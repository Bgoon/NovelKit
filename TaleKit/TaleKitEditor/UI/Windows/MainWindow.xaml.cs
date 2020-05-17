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
using TaleKitEditor.UI.Workspaces;
using GKit;
using GKit.WPF;
using TaleKit.Datas;
using TaleKitEditor.UI.Controls;
using PenMotionEditor.UI.Tabs;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using TaleKitEditor.Workspaces;
using TaleKitEditor.UI.Dialogs;
using TaleKitEditor.UI.Utility;

namespace TaleKitEditor.UI.Windows {
	public partial class MainWindow : Window {
		private Workspace[] workspaces;

		//Datas
		public TaleData EditingData {
			get; private set;
		}

		public event Action<TaleData> DataLoaded;
		public event Action<TaleData> DataUnloaded;

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

			//CreateFile();
		}

		private void InitWorkspaces() {
			workspaces = new Workspace[] {
				new Workspace(UiWorkspace, UiWorkspaceButton),
				new Workspace(MotionWorkspace, MotionWorkspaceButton),
				new Workspace(StoryWorkspace, StoryWorkspaceButton),
			};

			foreach(Workspace workspace in workspaces) {
				if (workspace.context.Parent == null) {
					WorkspaceContext.Children.Add(workspace.context);
				}
			}

			WorkspaceContext.Visibility = Visibility.Collapsed;
		}
		private void RegisterEvents() {
			FileManagerBar.CreateFileButtonClick += CreateFile;
			FileManagerBar.OpenFileButtonClick += OpenFile;
			FileManagerBar.SaveFileButtonClick += SaveFile;
			FileManagerBar.ExportButtonClick += ExportData;
		}

		public void CreateFile() {
			if (!ShowCheckSaveDialog())
				return;

			CommonOpenFileDialog dialog = new CommonOpenFileDialog() {
				IsFolderPicker = true,
			};
			if (dialog.ShowDialog(this) != CommonFileDialogResult.Ok)
				return;

			try {
				EditingData = new TaleData();
				EditingData.projectDir = dialog.FileName;
				EditingData.MotionFile.SetMotionFileData(MotionWorkspace.EditorContext.EditingFile);
			} catch(Exception ex) {
				MessageBox.Show("파일을 여는 데 실패했습니다.");
			}

			DataLoaded?.Invoke(EditingData);

			EditingData.PostInit();

			WorkspaceContext.Visibility = Visibility.Visible;
		}
		public void UnloadData() {
			DataUnloaded?.Invoke(EditingData);

			EditingData = null;
		}
		public void OpenFile() {
			if (!ShowCheckSaveDialog())
				return;

			OpenFileDialog dialog = new OpenFileDialog();
			if (!dialog.ShowDialog(this).HasTrueValue())
				return;

			
		}
		public void SaveFile() {
			SaveFileDialog dialog = new SaveFileDialog();

			if (!dialog.ShowDialog(this).HasTrueValue())
				return;
		}
		public void ExportData() {
			if (!ShowCheckSaveDialog())
				return;

			SaveFileDialog dialog = new SaveFileDialog();

			if (!dialog.ShowDialog(this).HasTrueValue())
				return;

			EditingData.Export(dialog.FileName);
		}
		public bool ShowCheckSaveDialog() {
			return true;
		}

		public void ActiveWorkspace(WorkspaceType type) {
			DeactiveWorkspaces();

			Workspace workspace = workspaces[(int)type];
			
			workspace.context.Visibility = Visibility.Visible;
			workspace.button.IsActiveWorkspace = true;
		}
		private void DeactiveWorkspaces() {
			Workspace workspace;
			for (int i = 0; i < workspaces.Length; ++i) {
				workspace = workspaces[i];
				workspace.context.Visibility = Visibility.Collapsed;
				workspace.button.IsActiveWorkspace = false;
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
