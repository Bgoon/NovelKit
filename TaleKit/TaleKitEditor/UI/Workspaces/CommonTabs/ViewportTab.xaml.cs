using GKitForWPF;
using GKitForWPF;
using GKitForWPF.UI.Controls;
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
using TaleKitEditor.UI.ModelEditor;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiWorkspace UiWorkspace => MainWindow.UiWorkspace;
		private static UiOutlinerTab UiOutlinerTab => UiWorkspace.UiOutlinerTab;
		private static UiFile EditingUiFile => MainWindow.EditingTaleData.UiFile;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		private UiRenderer rootRenderer;

		// [ Constructor ]
		public ViewportTab() {
			this.RegisterLoadedOnce(OnLoadedOnce);
			InitializeComponent();
		}

		// [ Event ]
		private void OnLoadedOnce(object sender, RoutedEventArgs e) {
			// Register events
			MainWindow.ProjectLoaded += MainWindow_ProjectLoaded;
			
			UiOutlinerTab.ItemMoved += UiOutlinerTab_ItemMoved;
			PlayStateButton.ActiveChanged += PlayStateButton_ActiveChanged;

			PlayStateButton.IsActive = true;

			ResolutionSelector.ResolutionChanged += ResolutionSelector_ResolutionChanged;
			ResolutionSelector.ZoomChanged += ResolutionSelector_ZoomChanged;

			ResolutionSelector.RaiseResolutionChanged();
			ResolutionSelector.RaiseZoomChanged();
		}

		// Viewport
		private void ResolutionSelector_ZoomChanged(double zoom) {
			Viewport.LayoutTransform = new ScaleTransform(zoom, zoom);
		}
		private void ResolutionSelector_ResolutionChanged(int width, int height) {
			Viewport.Width = width;
			Viewport.Height = height;
		}
		private void PlayStateButton_ActiveChanged() {
			StoryBlockTab.ApplyBlockToSelectionToRenderer();
		}

		// File event
		private void MainWindow_ProjectLoaded(TaleKit.Datas.TaleData obj) {
			EditingUiFile.ItemCreated += UiFile_ItemCreated;
			EditingUiFile.ItemRemoved += UiFile_ItemRemoved;

			ScrollToCenter();
		}
		private void UiFile_ItemCreated(UiItemBase item, UiItemBase parentItem) {
			UiRenderer renderer = new UiRenderer(item);
			EditingUiFile.Item_To_RendererDict.Add(item, renderer);

			if (parentItem == null) {
				rootRenderer = renderer;
				RendererContext.Children.Add(rootRenderer);
			} else {
				UiRenderer parentView = GetRenderer(parentItem);
				parentView.ChildItemContext.Children.Add(renderer);
			}

			renderer.Render(false);
		}
		private void UiFile_ItemRemoved(UiItemBase item, UiItemBase parentItem) {
			UiRenderer renderer = GetRenderer(item);
			renderer.DetachParent();

			EditingUiFile.Item_To_RendererDict.Remove(item);
		}
		internal void UiItemDetailPanel_UiItemValueChanged(object model, FieldInfo fieldInfo, object editorView) {
			UiRenderer renderer = GetRenderer(model as UiItemBase);

			renderer.Render(false);
		}
		private void UiOutlinerTab_ItemMoved(UiItemBase item, UiItemBase newParentItem, UiItemBase oldParentItem) {
			UiRenderer renderer = GetRenderer(item);
			UiRenderer parentView = GetRenderer(newParentItem);

			int index = newParentItem.ChildItemList.IndexOf(item);
			renderer.DetachParent();
			parentView.ChildItemContext.Children.Insert(index, renderer);
			renderer.Render(false);
			
		}

		// [ Access ]
		private UiRenderer GetRenderer(UiItemBase item) {
			return EditingUiFile.Item_To_RendererDict[item] as UiRenderer;
		}

		public async void ScrollToCenter() {
			await Task.Delay(10);

			ViewportScrollViewer.ScrollToHorizontalOffset(ViewportScrollViewer.ScrollableWidth / 2d);
			ViewportScrollViewer.ScrollToVerticalOffset(ViewportScrollViewer.ScrollableHeight / 2d);
		}
	}
}
