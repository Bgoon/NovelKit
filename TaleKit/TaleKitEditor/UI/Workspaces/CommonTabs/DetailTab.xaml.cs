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
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;
using TaleKitEditor.Workspaces.Tabs;

namespace TaleKitEditor.UI.Workspaces.CommonTabs {
	/// <summary>
	/// DetailTab.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DetailTab : UserControl, INeedPostInitTab {

		public StoryBlockDetailPanel StoryBlockDetailPanel;
		public UiItemDetailPanel UiItemDetailPanel;

		public DetailTab() {
			InitializeComponent();
		}
		public void PostInit() {
			StoryBlockDetailPanel = new StoryBlockDetailPanel();
			UiItemDetailPanel = new UiItemDetailPanel();
		}
		public void ActiveDetailPanel(DetailPanelType type) {
			UserControl panel = null;

			switch(type) {
				case DetailPanelType.UiItem:
					panel = UiItemDetailPanel;
					break;
				case DetailPanelType.Scene:
					break;
				case DetailPanelType.StoryBlock:
					panel = StoryBlockDetailPanel;
					break;
				case DetailPanelType.StoryClip:
					break;
			}

			if(panel != null) {
				Content = panel;
			}
		}
		public void DeactiveDetailPanel() {
			Content = null;
		}
	}
}
