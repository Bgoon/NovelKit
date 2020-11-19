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
using TaleKit.Datas;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.UI.ModelEditor;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;
using TaleKitEditor.Workspaces;
using UVector2 = UnityEngine.Vector2;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiWorkspace UiWorkspace => MainWindow.UiWorkspace;
		private static UiOutlinerTab UiOutlinerTab => UiWorkspace.UiOutlinerTab;
		private static UIFile EditingUiFile => MainWindow.EditingTaleData.UiFile;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		private UIRenderer rootRenderer;

		private UISnapshot renderUiSnapshot;
		// TODO : 재생용 UiRoot를 Viewport가 하나 소지하고 있고, 이 데이터를 기반으로 화면에 보여주게 할 것

		// [ Constructor ]
		public ViewportTab() {
			this.RegisterLoadedOnce(OnLoadedOnce);
			InitializeComponent();
		}

		// [ Event ]
		private void OnLoadedOnce(object sender, RoutedEventArgs e) {
			// Register events
			MainWindow.WorkspaceActived += MainWindow_WorkspaceActived;
			MainWindow.ProjectPreloaded += MainWindow_ProjectPreloaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;
			
			UiOutlinerTab.ItemMoved += UiOutlinerTab_ItemMoved;
			PlayStateButton.ActiveChanged += PlayStateButton_ActiveChanged;

			PlayStateButton.IsActive = true;

			ResolutionSelector.ResolutionChanged += ResolutionSelector_ResolutionChanged;
			ResolutionSelector.ZoomChanged += ResolutionSelector_ZoomChanged;

			ResolutionSelector.OnResolutionChanged();
			ResolutionSelector.RaiseZoomChanged();

			Viewport.RegisterClickEvent(Viewport_Click, true);
		}

		private void MainWindow_WorkspaceActived(WorkspaceComponent workspace) {
			// Viewport 데이터를 UI에디터로 초기화
			ResetSnapshot();
			RenderAll();
		}
		private void MainWindow_ProjectPreloaded(TaleData taleData) {
			ResetSnapshot();

			EditingUiFile.ItemCreated += UiFile_ItemCreated;
			EditingUiFile.ItemRemoved += UiFile_ItemRemoved;

			ScrollToCenter();
		}
		private void MainWindow_ProjectUnloaded(TaleKit.Datas.TaleData taleData) {
			RendererContext.Children.Remove(rootRenderer);
		}

		// Viewport
		private void ResolutionSelector_ZoomChanged(double newZoomScale, double oldZoomScale) {
			double scaleDelta = newZoomScale - oldZoomScale;
			UVector2 sizeDelta = ResolutionSelector.ResolutionNumberEditor.Value * (float)scaleDelta;

			Viewport.LayoutTransform = new ScaleTransform(newZoomScale, newZoomScale);

			ViewportScrollViewer.ScrollToVerticalOffset(ViewportScrollViewer.VerticalOffset + sizeDelta.y * 0.5f);
			ViewportScrollViewer.ScrollToHorizontalOffset(ViewportScrollViewer.HorizontalOffset + sizeDelta.x * 0.5f);
		}
		private void ResolutionSelector_ResolutionChanged(int width, int height) {
			Viewport.Width = width;
			Viewport.Height = height;
		}
		private void PlayStateButton_ActiveChanged() {
			StoryBlockTab.ApplyBlockToSelectionToRenderer();
		}

		// File event
		private void UiFile_ItemCreated(UIItemBase item, UIItemBase parentItem) {
			// Manage renderUI
			UIItemBase renderUi;
			if (parentItem == null) {
				renderUi = renderUiSnapshot.rootUiItem = item.Clone() as UIItemBase;
			} else {
				UIItemBase parentRenderUi = renderUiSnapshot.GetUiItem(parentItem.guid);
				renderUi = item.Clone() as UIItemBase;

				parentRenderUi.AddChildItem(renderUi);
			}
			renderUi.InitializeClone();
			renderUiSnapshot.RegisterUiItem(renderUi.guid, renderUi);


			// Manage renderer
			UIRenderer renderer = new UIRenderer(renderUi);
			EditingUiFile.Guid_To_RendererDict.Add(item.guid, renderer);

			if (parentItem == null) {
				rootRenderer = renderer;
				RendererContext.Children.Add(rootRenderer);
			} else {
				UIRenderer parentView = GetRenderer(parentItem);
				parentView.ChildItemContext.Children.Add(renderer);
			}

			renderer.Render(false);
		}
		private void UiFile_ItemRemoved(UIItemBase item, UIItemBase parentItem) {
			// Manage renderUi
			UIItemBase renderUi = renderUiSnapshot.GetUiItem(item.guid);
			if(renderUi.ParentItem != null) {
				renderUi.ParentItem.RemoveChildItem(renderUi);
			}
			renderUiSnapshot.RemoveUiItem(renderUi.guid);

			// Manage renderer
			UIRenderer renderer = GetRenderer(item);
			renderer.DetachParent();

			EditingUiFile.Guid_To_RendererDict.Remove(item.guid);
		}
		internal void UiItemDetailPanel_UiItemValueChanged(object model, FieldInfo fieldInfo, object editorView) {
			UIItemBase UiItem = model as UIItemBase;

			// Manage renderUi
			UIItemBase renderUi = renderUiSnapshot.GetUiItem(UiItem.guid);
			fieldInfo.SetValue(renderUi, fieldInfo.GetValue(UiItem));

			// Manage renderer
			UIRenderer renderer = GetRenderer(UiItem);

			renderer.Render(false);
		}
		private void UiOutlinerTab_ItemMoved(UIItemBase item, UIItemBase newParentItem, UIItemBase oldParentItem, int index) {
			// Manage renderUi
			UIItemBase renderUi = renderUiSnapshot.GetUiItem(item.guid);
			renderUiSnapshot.GetUiItem(oldParentItem.guid).RemoveChildItem(renderUi);
			renderUiSnapshot.GetUiItem(newParentItem.guid).InsertChildItem(index, renderUi);
			
			// Manage renderer
			UIRenderer renderer = GetRenderer(item);
			UIRenderer parentView = GetRenderer(newParentItem);

			renderer.DetachParent();
			parentView.ChildItemContext.Children.Insert(index, renderer);
			renderer.Render(false);
		} 

		// Input
		private void Viewport_Click() {
			if (StoryBlockTab.StoryBlockTreeView.SelectedItemSet.Count != 1)
				return;

			StoryBlockTab.SelectNextBlock();
		}

		// [ Render ]
		public void ResetSnapshot() {
			renderUiSnapshot = EditingUiFile.UISnapshot.Clone();
		}
		public void RenderAll() {
			UIRenderer renderer = GetRenderer(renderUiSnapshot.rootUiItem);

			renderer.Render(true);
		}

		// [ Access ]
		private UIRenderer GetRenderer(UIItemBase item) {
			return EditingUiFile.Guid_To_RendererDict[item.guid] as UIRenderer;
		}

		public async void ScrollToCenter() {
			await Task.Delay(10);

			ViewportScrollViewer.ScrollToHorizontalOffset(ViewportScrollViewer.ScrollableWidth / 2d);
			ViewportScrollViewer.ScrollToVerticalOffset(ViewportScrollViewer.ScrollableHeight / 2d);
		}
	}
}
