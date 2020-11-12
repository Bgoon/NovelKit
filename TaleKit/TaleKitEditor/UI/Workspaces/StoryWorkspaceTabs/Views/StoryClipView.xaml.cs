using GKitForWPF;
using GKitForWPF.UI.Controls;
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
using TaleKit.Datas.Story;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views {
	/// <summary>
	/// StoryBlock.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryClipView : UserControl, ITreeItem {

		public string description;
		public readonly StoryClip Data;

		//ITreeItem interface
		public string DisplayName => NameEditText.Text;
		public ITreeFolder ParentItem {
			get; set;
		}
		public FrameworkElement ItemContext => this;


		// [ Constructor ]
		public StoryClipView() {
			InitializeComponent();
		}
		public StoryClipView(StoryClip data) : this() {
			this.Data = data;
		}

		// [ Control ]
		public void SetDisplayName(string name) {
			//PreviewTextBlock.Text = name;
		}

		public void SetSelected(bool isSelected) {
			ItemPanel.Background = (Brush)Application.Current.Resources[isSelected ? "ItemBackground_Selected" : "ItemBackground"];
		}
	}
}
