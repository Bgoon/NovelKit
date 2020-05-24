using GKit;
using GKit.WPF;
using System;
using System.Collections.Generic;
using System.IO;
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
using TaleKit.Datas;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs.AssetElements;
using TaleKitEditor.Workspaces.Tabs;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	/// <summary>
	/// AssetTab.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class AssetTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData TaleFile => MainWindow.EditingProject;

		public static string[] ExcludeDirNames = new string[] {
			".vs",
			".git",
			".svn",
		};
		public static string[] ExcludeFileExtensions = new string[] {
			".meta",
		};

		public string AssetDir => TaleFile.AssetDir;
		public string ExploringDir {
			get; private set;
		}

		private FileSystemWatcher watcher;

		private DirTreeItemView rootDirTreeItemView;

		//Selection
		private List<FileItemView> selectedFileItemList;

		public AssetTab() {
			this.RegisterLoaded(OnLoaded);
			InitializeComponent();
		}
		private void RegisterEvents() {
			MainWindow.ProjectLoaded += MainWindow_ProjectLoaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;

			GotoParentButton.RegisterButtonReaction();
			GotoParentButton.RegisterOnClick(GotoParentButton_Click);
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			RegisterEvents();
		}

		private void MainWindow_ProjectLoaded(TaleData obj) {
			selectedFileItemList = new List<FileItemView>();
			Directory.CreateDirectory(AssetDir);

			InitDirTree();
			ExploreDir(AssetDir);
		}
		private void MainWindow_ProjectUnloaded(TaleData obj) {
			UnwatchDirectory();
			ResetDirTree();
		}
		private void ExplorerContext_Drop(object sender, DragEventArgs e) {
			string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);

			foreach(string filename in filenames) {
				if(File.Exists(filename)) {
					ImportAsset(filename);
				}
			}
		}
		private void GotoParentButton_Click() {
			if(!IOUtility.ComparePath(AssetDir, ExploringDir)) {
				ExploreDir(Directory.GetParent(ExploringDir).FullName);
			}
		}
		private void Watcher_Changed(object sender, FileSystemEventArgs e) {
			Dispatcher.BeginInvoke(new Action(() => {
				UpdateExplorer();
			}));
		}

		public void ExploreDir(string dir) {
			ExploringDir = dir;
			
			UpdateExplorer();
		}

		public void ImportAsset(string filename) {
			ImportAsset(filename, ExploringDir);
		}
		public void ImportAsset(string filename, string targetDir) {
			string onlyFilename = Path.GetFileName(filename);
			try {
				Directory.CreateDirectory(targetDir);
				File.Copy(filename, Path.Combine(targetDir, onlyFilename));
			} catch(Exception ex) {
				MessageBox.Show($"파일 '{onlyFilename}' 을 가져오는 데 문제가 발생했습니다.");
			}
		}

		public void ShowAlertText(string text) {
			AlertTextBlock.Text = text;
		}

		private void InitDirTree() {
			DirTreeContext.Children.Clear();

			rootDirTreeItemView = new DirTreeItemView(AssetDir);
			DirTreeContext.Children.Add(rootDirTreeItemView);
		}
		private void ResetDirTree() {
			rootDirTreeItemView = null;
			DirTreeContext.Children.Clear();
		}

		private void WatchDirectory() {
			UnwatchDirectory();

			watcher = new FileSystemWatcher() {
				Path = ExploringDir,
				IncludeSubdirectories = false,
			};
			watcher.Created += Watcher_Changed;
			watcher.Deleted += Watcher_Changed;
			watcher.Renamed += Watcher_Changed;
			watcher.Changed += Watcher_Changed;
			watcher.EnableRaisingEvents = true;
		}
		private void UnwatchDirectory() {
			if (watcher != null) {
				watcher.Dispose();
				watcher = null;
			}
		}

		private void UpdateExplorer() {
			//TODO : 디렉토리가 존재하는지 확인하고 없으면 상위디렉토리로 가도록 하자

			WatchDirectory();

			CurrentDirectoryTextBlock.Text = GetRelativePath(ExploringDir);

			FileItemContext.Children.Clear();

			try {
				CreateFileItemViews();
				OpenTargetDirTree();

				ShowAlertText("");
			} catch(Exception ex) {
				ShowAlertText("파일 목록을 불러오는 중 오류가 발생했습니다.");
			}
		}

		private void CreateFileItemViews() {
			string[] dirs = Directory.GetDirectories(ExploringDir).Where((string dirName) => {
				return !ExcludeDirNames.Contains(dirName);
			}).ToArray();
			string[] files = Directory.GetFiles(ExploringDir).Where((string fileName) => {
				if (!fileName.Contains('.')) {
					return true;
				}
				return !ExcludeFileExtensions.Contains(Path.GetExtension(fileName));
			}).ToArray();

			foreach (string dir in dirs) {
				FileItemView itemView = new FileItemView(dir, FileIconType.Directory);
				itemView.RegisterDoubleClickEvent(() => {
					ExploreDir(itemView.FullFilename);
				});

				FileItemContext.Children.Add(itemView);
			}
			foreach (string file in files) {
				FileItemView itemView = new FileItemView(file, FileIconType.File);

				FileItemContext.Children.Add(itemView);
			}
		}
		private void OpenTargetDirTree() {
			string relativeExploringDir = GetRelativePath(ExploringDir);

			string[] dirTreeNames = relativeExploringDir.Split('\\').Skip(1).ToArray();

			DirTreeItemView treeItem = rootDirTreeItemView;
			foreach(string dirName in dirTreeNames) {
				treeItem.SetTreeOpen(true);

				treeItem = treeItem.FindChildDir(dirName);
				if (treeItem == null)
					break;
			}
		}

		private string GetRelativePath(string fullPath) {
			const string AssetDirName = "Assets";

			if (fullPath.Contains(AssetDirName)) {
				return fullPath.Substring(fullPath.IndexOf(AssetDirName));
			} else {
				return fullPath;
			}
		}

		
	}
}
