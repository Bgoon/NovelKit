using GKit;
using GKit.WPF.UI.Controls;
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

namespace TaleKitEditor.UI.Workspaces.StoryEditorTabs.StoryBoardElements {
	/// <summary>
	/// StoryBlock.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockItemView : UserControl, IListFolder {

		public string description;
		//public 

		public StoryBlockItemView() {
			InitializeComponent();
		}

		public UIElementCollection ChildItemCollection => ChildStackPanel.Children;

		public string DisplayName => PreviewTextBlock.Text;

		public FrameworkElement ItemContext => ItemPanel;

		public IListFolder ParentItem {
			get; set;
		}


		public void SetDisplayName(string name) {
			PreviewTextBlock.Text = name;
		}

		public void SetDisplaySelected(bool isSelected) {
			ItemPanel.Background = (Brush)Application.Current.Resources[isSelected ? "ItemBackground_Selected" : "ItemBackground"];
		}
	}
}
