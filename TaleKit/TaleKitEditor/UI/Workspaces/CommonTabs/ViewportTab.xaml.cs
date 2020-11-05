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
using TaleKitEditor.Workspaces;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiWorkspace UiWorkspace => MainWindow.UiWorkspace;
		private static UiOutlinerTab UiOutlinerTab => UiWorkspace.UiOutlinerTab;
		private static UiFile EditingUiFile => MainWindow.EditingTaleData.UiFile;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		private UiRenderer rootRenderer;

		private UiSnapshot renderUiSnapshot;
		// TODO : 재생용 UiRoot를 Viewport가 하나 소지하고 있고, 이 데이터를 기반으로 화면에 보여주게 할 것

		// [ Constructor ]
		public ViewportTab() {
			this.RegisterLoadedOnce(OnLoadedOnce);
			InitializeComponent();

			// Initialize members
			renderUiSnapshot = new UiSnapshot();

			// Register events
			MainWindow.WorkspaceActived += MainWindow_WorkspaceActived;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;
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

			ResolutionSelector.OnResolutionChanged();
			ResolutionSelector.RaiseZoomChanged();
		}

		private void MainWindow_WorkspaceActived(WorkspaceComponent workspace) {
			// Viewport 데이터를 UI에디터로 초기화
			ResetSnapshot();
			RenderAll();
		}
		private void MainWindow_ProjectUnloaded(TaleKit.Datas.TaleData taleData) {
			RendererContext.Children.Remove(rootRenderer);
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
			// Manage renderUI
			UiItemBase renderUi;
			if (parentItem == null) {
				renderUi = renderUiSnapshot.rootUiItem = item.Clone() as UiItemBase;
			} else {
				UiItemBase parentRenderUi = renderUiSnapshot.GetUiItem(parentItem.guid);
				renderUi = item.Clone() as UiItemBase;

				parentRenderUi.AddChildItem(renderUi);
			}
			renderUi.InitializeClone();
			renderUiSnapshot.RegisterUiItem(renderUi.guid, renderUi);


			// Manage renderer
			UiRenderer renderer = new UiRenderer(renderUi);
			EditingUiFile.Guid_To_RendererDict.Add(item.guid, renderer);

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
			// Manage renderUi
			UiItemBase renderUi = renderUiSnapshot.GetUiItem(item.guid);
			if(renderUi.ParentItem != null) {
				renderUi.ParentItem.RemoveChildItem(renderUi);
			}
			renderUiSnapshot.RemoveUiItem(renderUi.guid);

			// Manage renderer
			UiRenderer renderer = GetRenderer(item);
			renderer.DetachParent();

			EditingUiFile.Guid_To_RendererDict.Remove(item.guid);
		}
		internal void UiItemDetailPanel_UiItemValueChanged(object model, FieldInfo fieldInfo, object editorView) {
			UiItemBase UiItem = model as UiItemBase;

			// Manage renderUi
			UiItemBase renderUi = renderUiSnapshot.GetUiItem(UiItem.guid);
			fieldInfo.SetValue(renderUi, fieldInfo.GetValue(UiItem));

			// Manage renderer
			UiRenderer renderer = GetRenderer(UiItem);

			renderer.Render(false);
		}
		private void UiOutlinerTab_ItemMoved(UiItemBase item, UiItemBase newParentItem, UiItemBase oldParentItem, int index) {
			// Manage renderUi
			UiItemBase renderUi = renderUiSnapshot.GetUiItem(item.guid);
			renderUiSnapshot.GetUiItem(oldParentItem.guid).RemoveChildItem(renderUi);
			renderUiSnapshot.GetUiItem(newParentItem.guid).InsertChildItem(index, renderUi);
			
			// Manage renderer
			UiRenderer renderer = GetRenderer(item);
			UiRenderer parentView = GetRenderer(newParentItem);

			renderer.DetachParent();
			parentView.ChildItemContext.Children.Insert(index, renderer);
			renderer.Render(false);
		} 

		// [ Render ]
		public void ResetSnapshot() {
			renderUiSnapshot.CopyDataFrom(EditingUiFile.UiSnapshot);
		}
		public void RenderAll() {
			UiRenderer renderer = GetRenderer(renderUiSnapshot.rootUiItem);

			renderer.Render(true);
		}

		// [ Access ]
		private UiRenderer GetRenderer(UiItemBase item) {
			return EditingUiFile.Guid_To_RendererDict[item.guid] as UiRenderer;
		}

		public async void ScrollToCenter() {
			await Task.Delay(10);

			ViewportScrollViewer.ScrollToHorizontalOffset(ViewportScrollViewer.ScrollableWidth / 2d);
			ViewportScrollViewer.ScrollToVerticalOffset(ViewportScrollViewer.ScrollableHeight / 2d);
		}
	}
}
