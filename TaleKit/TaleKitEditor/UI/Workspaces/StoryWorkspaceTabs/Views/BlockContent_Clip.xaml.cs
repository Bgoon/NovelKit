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
using TaleKit.Datas.Story.StoryBlock;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views {
	/// <summary>
	/// BlockContent_Clip.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class BlockContent_Clip : UserControl, IBlockContent {

		public readonly StoryBlockView OwnerBlockView;
		public StoryBlockBase Data => OwnerBlockView.Data;

		public BlockContent_Clip(StoryBlockView ownerBlockView) {
			InitializeComponent();

			this.OwnerBlockView = ownerBlockView;
		}
	}
}
