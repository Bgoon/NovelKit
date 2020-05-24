using System;
using System.Collections.Generic;
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
using GKit.WPF;
using TaleKit.Datas.UI;
using TaleKitEditor.UI.ValueEditors;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using TaleKitEditor.UI.Workspaces.CommonTabs.DetailPanelElements;

namespace TaleKitEditor.UI.Workspaces.UiWorkspaceTabs {
	
	public partial class UiItemDetailPanel : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiWorkspace UiWorkspace => MainWindow.UiWorkspace;
		private static UiOutlinerTab UiOutlinerTab => UiWorkspace.UiOutlinerTab;
		private static DetailTab DetailTab => MainWindow.DetailTab;

		public UiItem EditingUiItem {
			get; private set;
		}

		public UiItemDetailPanel() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			InitMembers();
			RegisterEvents();

			SelectionChanged();
		}
		private void InitMembers() {
		}
		private void RegisterEvents() {
			UiOutlinerTab.UiTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			UiOutlinerTab.UiTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}

		private void SelectedItemSet_SelectionAdded(GKit.WPF.UI.Controls.ITreeItem item) {
			SelectionChanged();
			DetailTab.ActiveDetailPanel(DetailPanelType.UiItem);
		}
		private void SelectedItemSet_SelectionRemoved(GKit.WPF.UI.Controls.ITreeItem item) {
			SelectionChanged();
			DetailTab.DeactiveDetailPanel();
		}

		private void SelectionChanged() {
			AttachUiItem(UiOutlinerTab.SelectedUiItemSingle);
		}

		private void AttachUiItem(UiItem uiItem) {
			DetachUiItem();

			if (uiItem == null)
				return;

			ValueEditorUtility.CreateValueEditorViews(uiItem, EditorViewContext);
		}
		private void DetachUiItem() {
			if (EditingUiItem == null)
				return;

			this.EditingUiItem = null;

			EditorViewContext.Children.Clear();
		}
	}
}
