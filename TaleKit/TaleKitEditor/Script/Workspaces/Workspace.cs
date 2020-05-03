using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaleKitEditor.UI.Controls;

namespace TaleKitEditor.Workspaces {
	public class Workspace {
		public UserControl context;
		public WorkspaceButton button;

		public Workspace(UserControl context, WorkspaceButton button) {
			this.context = context;
			this.button = button;
		}
	}
}
