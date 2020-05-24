using GKit;
using GKit.WPF;
using GKit.WPF.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiWorkspace UiWorkspace => MainWindow.UiWorkspace;
		private static UiOutlinerTab UiOutlinerTab => UiWorkspace.UiOutlinerTab;
		private static UiFile UiFile => MainWindow.EditingProject.UiFile;

		public UiFile EditingUiFile {
			get; private set;
		}

		private UiRenderer rootRenderer;

		public ViewportTab() {
			this.RegisterLoaded(OnLoaded);
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			MainWindow.ProjectLoaded += MainWindow_ProjectLoaded;
		}
		private void MainWindow_ProjectLoaded(TaleKit.Datas.TaleData obj) {
			UiFile.ItemCreated += UiFile_ItemCreated;
			UiFile.ItemRemoved += UiFile_ItemRemoved;

			ScrollToCenter();
		}
		private void UiFile_ItemCreated(UiItem item, UiItem parentItem) {
			RenderAll(true, true);
		}
		private void UiFile_ItemRemoved(UiItem item, UiItem parentItem) {
			RenderAll(true, true);
		}
		internal void UiItemDetailPanel_UiItemValueChanged(object model, FieldInfo fieldInfo) {
			RenderAll(true, false);
		}


		public async void ScrollToCenter() {
			await Task.Delay(10);

			CanvasScrollViewer.ScrollToHorizontalOffset(CanvasScrollViewer.ScrollableWidth / 2d);
			CanvasScrollViewer.ScrollToVerticalOffset(CanvasScrollViewer.ScrollableHeight / 2d);
		}

		private void RenderAll(bool renderChilds, bool rebuild) {
			if (rebuild) {
				CanvasContext.Children.Clear();

				rootRenderer = new UiRenderer(UiFile.RootUiItem);
				CanvasContext.Children.Add(rootRenderer);
				rootRenderer.Render(renderChilds, rebuild);
			} else {
				if(rootRenderer != null) {
					rootRenderer.Render(renderChilds, rebuild);
				}
			}
		}
	}
}
