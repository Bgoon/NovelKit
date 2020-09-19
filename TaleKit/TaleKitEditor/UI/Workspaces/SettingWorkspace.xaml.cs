using System;
using System.Collections.Generic;
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
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;
using TaleKitEditor.Workspaces;

namespace TaleKitEditor.UI.Workspaces {
	public partial class SettingWorkspace : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;

		public SettingWorkspace() {
			InitializeComponent();

			// Register events
			MainWindow.WorkspaceActived += MainWindow_WorkspaceActived;
		}

		private void MainWindow_WorkspaceActived(WorkspaceComponent workspace) {

			if(workspace.type == WorkspaceType.ProjectSetting) {
				MainWindow.DetailTab.CommonDetailPanel.AttachModel(MainWindow.EditingTaleData.ProjectSetting);
				MainWindow.DetailTab.ActiveDetailPanel(DetailPanelType.Common);

			}
		}
	}
}
