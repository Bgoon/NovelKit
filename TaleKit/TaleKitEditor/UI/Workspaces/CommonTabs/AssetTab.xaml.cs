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

namespace TaleKitEditor.UI.Workspaces.CommonWorkspaceTabs {
	/// <summary>
	/// AssetTab.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class AssetTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData TaleFile => MainWindow.EditingData;

		public string AssetDir => TaleFile.AssetDir;
		public string ExploringDir {
			get; private set;
		}

		public AssetTab() {
			InitializeComponent();
		}
		private void ExplorerContext_Loaded(object sender, RoutedEventArgs e) {
			MainWindow.DataLoaded += MainWindow_DataLoaded;
		}
		private void ExplorerContext_Drop(object sender, DragEventArgs e) {
			string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);

			foreach(string filename in filenames) {
				if(File.Exists(filename)) {
					LoadAsset(filename);
				}
			}
		}

		private void MainWindow_DataLoaded(TaleData obj) {
			ExploreDir(AssetDir);
		}


		public void LoadAsset(string filename) {
			LoadAsset(filename, ExploringDir);
		}
		public void LoadAsset(string filename, string targetDir) {
			string onlyFilename = Path.GetFileName(filename);
			try {
				Directory.CreateDirectory(targetDir);
				File.Copy(filename, Path.Combine(targetDir, onlyFilename));
			} catch(Exception ex) {
				MessageBox.Show($"파일 '{onlyFilename}' 을 가져오는 데 문제가 발생했습니다.");
			}
		}

		public void ExploreDir(string dir) {
			ExploringDir = dir;
		}

	}
}
