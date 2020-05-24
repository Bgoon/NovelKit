using GKit;
using GKit.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;

		public UiFile EditingUiFile {
			get; private set;
		}

		public ViewportTab() {
			this.RegisterLoaded(OnLoaded);
			InitializeComponent();
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			MainWindow.ProjectLoaded += MainWindow_ProjectLoaded;
		}
		private void MainWindow_ProjectLoaded(TaleKit.Datas.TaleData obj) {
			ScrollToCenter();
		}

		public void AttachUiFile(UiFile uiFile) {
			DetachUiFile();
			

		}
		public void DetachUiFile() {
			
		}

		public async void ScrollToCenter() {
			await Task.Delay(10);

			CanvasScrollViewer.ScrollToHorizontalOffset(CanvasScrollViewer.ScrollableWidth / 2d);
			CanvasScrollViewer.ScrollToVerticalOffset(CanvasScrollViewer.ScrollableHeight / 2d);
		}
	}
}
