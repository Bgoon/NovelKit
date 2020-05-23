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

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		public UiFile EditingUiFile {
			get; private set;
		}

		public ViewportTab() {
			InitializeComponent();
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			ScrollToCenter();
		}

		public void AttachUiFile(UiFile uiFile) {
			DetachUiFile();
			

		}
		public void DetachUiFile() {
			
		}

		private void ScrollToCenter() {
			CanvasScrollViewer.ScrollToHorizontalOffset(CanvasScrollViewer.ScrollableWidth / 2d);
			CanvasScrollViewer.ScrollToVerticalOffset(CanvasScrollViewer.ScrollableHeight / 2d);
		}
	}
}
