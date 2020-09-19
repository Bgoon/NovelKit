using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaleKitEditor.UI.Controls;

namespace TaleKitEditor.Workspaces {
	public class WorkspaceComponent {
		public WorkspaceType type;
		public UserControl context;
		public WorkspaceButton button;

		public WorkspaceComponent(WorkspaceType type, UserControl context, WorkspaceButton button) {
			this.type = type;
			this.context = context;
			this.button = button;
		}
	}
}
