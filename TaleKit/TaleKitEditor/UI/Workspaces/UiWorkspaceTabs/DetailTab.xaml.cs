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

namespace TaleKitEditor.UI.Workspaces.UiWorkspaceTabs {
	
	public partial class DetailTab : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static UiWorkspace UiWorkspace => MainWindow.UiWorkspace;
		private static LayerTab LayerTab => UiWorkspace.LayerTab;

		public UiItem EditingUiItem {
			get; private set;
		}

		private bool isInitialized;

		public DetailTab() {
			InitializeComponent();
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			if (this.IsDesignMode() || isInitialized)
				return;
			isInitialized = true;

			InitMembers();
			RegisterEvents();

			SelectionChanged();
		}
		private void InitMembers() {
		}
		private void RegisterEvents() {
			LayerTab.UiTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			LayerTab.UiTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
		}

		private void SelectedItemSet_SelectionAdded(GKit.WPF.UI.Controls.ITreeItem item) {
			SelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(GKit.WPF.UI.Controls.ITreeItem item) {
			SelectionChanged();
		}

		private void SelectionChanged() {
			AttachUiItem(LayerTab.SelectedUiItemSingle);
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
