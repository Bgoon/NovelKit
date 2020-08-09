using GKitForWPF;
using GKitForWPF.IO;
using GKitForWPF.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
using TaleKit.Datas;
using TaleKit.Datas.Asset;
using TaleKit.Datas.Resource;
using TaleKitEditor.UI.ModelEditor;
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
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static AssetManager AssetManager => EditingTaleData.AssetManager;
		private static DetailTab DetailTab => MainWindow.DetailTab;
		private static CommonDetailPanel CommonDetailPanel => DetailTab.CommonDetailPanel;

		public static string[] ExcludeDirNames = new string[] {
			".vs",
			".git",
			".svn",
		};
		public static string[] ExcludeFileExtensions = new string[] {
			".meta",
		};

		public string AssetDir => EditingTaleData.AssetDir;
		public string ExploringDir {
			get; private set;
		}

		private FileSystemWatcher watcher;

		private DirTreeItemView rootDirTreeItemView;

		//Selection
		private SelectedItemSet selectedFileItemSet;
		private string selectedFileItemKey;

		public AssetTab() {
			this.RegisterLoadedOnce(OnLoadedOnce);
			InitializeComponent();
		}
		private void InitMembers() {
			selectedFileItemSet = new SelectedItemSet();
		}
		private void RegisterEvents() {
			MainWindow.ProjectLoaded += MainWindow_ProjectLoaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;

			GotoParentButton.RegisterButtonReaction();
			GotoParentButton.RegisterClickEvent(GotoParentButton_Click);

			selectedFileItemSet.SelectionAdded += SelectedFileItemSet_SelectionAdded;
			selectedFileItemSet.SelectionRemoved += SelectedFileItemSet_SelectionRemoved;
		}

		private void OnLoadedOnce(object sender, RoutedEventArgs e) {
			InitMembers();
			RegisterEvents();
		}
		private void MainWindow_ProjectLoaded(TaleData obj) {
			InitDirTree();
			ExploreDir(AssetDir);
		}
		private void MainWindow_ProjectUnloaded(TaleData obj) {
			UnwatchDirectory();
			ClearDirTree();
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

		private void SelectedFileItemSet_SelectionRemoved(ISelectable item) {
			OnSelectionChanged(item as FileItemView);
		}
		private void SelectedFileItemSet_SelectionAdded(ISelectable item) {
			OnSelectionChanged(item as FileItemView);
		}
		private void OnSelectionChanged(FileItemView itemView) {
			CommonDetailPanel.DetachModel();
			DetailTab.DeactiveDetailPanel();

			if (selectedFileItemSet.Count == 1) {
				AssetItem item = AssetManager.LoadOrCreateMeta(itemView.AssetRelPath);
				selectedFileItemKey = item.Key;
				item.UpdatePreviewImageSource();

				CommonDetailPanel.AttachModel(item, SelectedAssetItem_ValueChanged);
				DetailTab.ActiveDetailPanel(DetailPanelType.Common);
			} else {
				selectedFileItemKey = null;
			}
		}

		private void SelectedAssetItem_ValueChanged(object model, FieldInfo fieldInfo, IValueEditorElement valueEditorElement) {
			AssetItem item = model as AssetItem;
			if(fieldInfo.Name == nameof(item.Key)) {
				if(!AssetManager.SetAssetKey(item, selectedFileItemKey, item.Key)) {
					Console.WriteLine("중복된 NameKey 가 이미 있습니다.");

					valueEditorElement.EditableValue = selectedFileItemKey;
				}
			}
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
		private void ClearDirTree() {
			rootDirTreeItemView = null;
			DirTreeContext.Children.Clear();
			selectedFileItemSet.UnselectItems();
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
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, dir);
				FileItemView itemView = new FileItemView(dir, assetRelPath, FileIconType.Directory);
				itemView.RegisterDoubleClickEvent(() => {
					ExploreDir(itemView.FullFilename);
				});

				FileItemContext.Children.Add(itemView);
			}
			foreach (string file in files) {
				string assetRelPath = IOUtility.GetRelativePath(AssetDir, file);
				FileItemView itemView = new FileItemView(file, assetRelPath, FileIconType.File);

				FileItemContext.Children.Add(itemView);

				itemView.Click += ItemView_Click;

				void ItemView_Click() {
					selectedFileItemSet.SetSelectedItem(itemView);
				}
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
