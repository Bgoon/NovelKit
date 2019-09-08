using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace TaleKitEditor.UI.Controls {
	public class FixedLayoutAnchorable : LayoutAnchorable {
		public FixedLayoutAnchorable() : base() {
			CanDockAsTabbedDocument = true;
			CanHide = false;
			CanAutoHide = false;
		}
	}
}
