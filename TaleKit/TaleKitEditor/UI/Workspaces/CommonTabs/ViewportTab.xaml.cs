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
using TaleKitEditor.UI.Workspaces.UIWorkspaceTabs;
using TaleKitEditor.Workspaces;
using UVector2 = UnityEngine.Vector2;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	public partial class ViewportTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UIWorkspace UIWorkspace => MainWindow.UIWorkspace;
		private static UIOutlinerTab UIOutlinerTab => UIWorkspace.UIOutlinerTab;
		private static UIFile EditingUIFile => MainWindow.EditingTaleData.UIFile;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		private UIRenderer rootRenderer;

		public UISnapshot RenderUISnapshot {
			get; private set;
		}
		// TODO : 재생용 UIRoot를 Viewport가 하나 소지하고 있고, 이 데이터를 기반으로 화면에 보여주게 할 것

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
			
			UIOutlinerTab.ItemMoved += UIOutlinerTab_ItemMoved;
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
			RebuildSnapshot();

			EditingUIFile.ItemCreated += UIFile_ItemCreated;
			EditingUIFile.ItemRemoved += UIFile_ItemRemoved;

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
		private void UIFile_ItemCreated(UIItemBase item, UIItemBase parentItem) {
			// Manage renderUI
			UIItemBase renderUI;
			if (parentItem == null) {
				renderUI = RenderUISnapshot.rootUIItem = item.Clone() as UIItemBase;
			} else {
				UIItemBase parentRenderUI = RenderUISnapshot.GetUIItem(parentItem.guid);
				renderUI = item.Clone() as UIItemBase;

				parentRenderUI.AddChildItem(renderUI);
			}
			renderUI.InitializeClone();
			RenderUISnapshot.RegisterUIItem(renderUI.guid, renderUI);


			// Manage renderer
			UIRenderer renderer = new UIRenderer(renderUI);
			EditingUIFile.Guid_To_RendererDict.Add(item.guid, renderer);

			if (parentItem == null) {
				rootRenderer = renderer;
				RendererContext.Children.Add(rootRenderer);
			} else {
				UIRenderer parentView = GetRenderer(parentItem);
				parentView.ChildItemContext.Children.Add(renderer);
			}

			renderer.Render(false);
		}
		private void UIFile_ItemRemoved(UIItemBase item, UIItemBase parentItem) {
			// Manage renderUI
			UIItemBase renderUI = RenderUISnapshot.GetUIItem(item.guid);
			if(renderUI.ParentItem != null) {
				renderUI.ParentItem.RemoveChildItem(renderUI);
			}
			RenderUISnapshot.RemoveUIItem(renderUI.guid);

			// Manage renderer
			UIRenderer renderer = GetRenderer(item);
			renderer.DetachParent();

			EditingUIFile.Guid_To_RendererDict.Remove(item.guid);
		}
		internal void UIItemDetailPanel_UIItemValueChanged(object model, FieldInfo fieldInfo, object editorView) {
			UIItemBase UIItem = model as UIItemBase;

			// Manage renderUI
			UIItemBase renderUI = RenderUISnapshot.GetUIItem(UIItem.guid);
			fieldInfo.SetValue(renderUI, fieldInfo.GetValue(UIItem));

			// Manage renderer
			UIRenderer renderer = GetRenderer(UIItem);

			renderer.Render(false);
		}
		private void UIOutlinerTab_ItemMoved(UIItemBase item, UIItemBase newParentItem, UIItemBase oldParentItem, int index) {
			// Manage renderUI
			UIItemBase renderUI = RenderUISnapshot.GetUIItem(item.guid);
			RenderUISnapshot.GetUIItem(oldParentItem.guid).RemoveChildItem(renderUI);
			RenderUISnapshot.GetUIItem(newParentItem.guid).InsertChildItem(index, renderUI);
			
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
		public void RebuildSnapshot() {
			RenderUISnapshot = EditingUIFile.UISnapshot.Clone();
		}
		public void ResetSnapshot() {
			RenderUISnapshot.CopyDataFrom(EditingUIFile.UISnapshot.Clone());
		}
		public void RenderAll() {
			UIRenderer renderer = GetRenderer(RenderUISnapshot.rootUIItem);

			renderer.Render(true);
		}

		// [ Access ]
		private UIRenderer GetRenderer(UIItemBase item) {
			return EditingUIFile.Guid_To_RendererDict[item.guid] as UIRenderer;
		}

		public async void ScrollToCenter() {
			await Task.Delay(10);

			ViewportScrollViewer.ScrollToHorizontalOffset(ViewportScrollViewer.ScrollableWidth / 2d);
			ViewportScrollViewer.ScrollToVerticalOffset(ViewportScrollViewer.ScrollableHeight / 2d);
		}
	}
}
