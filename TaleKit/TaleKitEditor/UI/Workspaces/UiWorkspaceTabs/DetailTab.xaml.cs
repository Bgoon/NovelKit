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

namespace TaleKitEditor.UI.Workspaces.UiWorkspaceTabs {
	
	public partial class DetailTab : UserControl {

		[FindByTag] private Button AnchorTopLeftButton;
		[FindByTag] private Button AnchorTopMidButton;
		[FindByTag] private Button AnchorTopRightButton;
		[FindByTag] private Button AnchorMidLeftButton;
		[FindByTag] private Button AnchorMidMidButton;
		[FindByTag] private Button AnchorMidRightButton;
		[FindByTag] private Button AnchorBotLeftButton;
		[FindByTag] private Button AnchorBotMidButton;
		[FindByTag] private Button AnchorBotRightButton;
		[FindByTag] private Button AnchorWideLeftButton;
		[FindByTag] private Button AnchorWideMidButton;
		[FindByTag] private Button AnchorWideRightButton;
		[FindByTag] private Button AnchorTopWideButton;
		[FindByTag] private Button AnchorMidWideButton;
		[FindByTag] private Button AnchorBotWideButton;
		[FindByTag] private Button AnchorWideWideButton;

		public DetailTab() {
			InitializeComponent();
			this.FindControlsByTag();
		}

		
	}
}
