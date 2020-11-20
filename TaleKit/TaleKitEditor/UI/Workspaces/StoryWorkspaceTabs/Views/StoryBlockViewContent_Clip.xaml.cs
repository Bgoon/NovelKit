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
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views {
	/// <summary>
	/// BlockContent_Clip.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockViewContent_Clip : UserControl, IBlockContent {
		private StoryFile EditingStoryFile => Data.OwnerFile;
		private MainWindow MainWindow => Root.Instance.MainWindow;
		private StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		public readonly StoryBlockView OwnerBlockView;
		public StoryBlock_Clip Data => OwnerBlockView.Data as StoryBlock_Clip;

		public StoryBlockViewContent_Clip(StoryBlockView ownerBlockView) {
			InitializeComponent();

			this.OwnerBlockView = ownerBlockView;

			SetUnknownPreviewText();
		}

		public void UpdatePreviewText() {
			do {
				string targetClipGuid = Data.targetClipGuid;

				if (string.IsNullOrEmpty(targetClipGuid))
					break;
				if (!EditingStoryFile.Guid_To_ClipDict.ContainsKey(targetClipGuid))
					break;

				StoryClip clip = EditingStoryFile.Guid_To_ClipDict[targetClipGuid];
				if (string.IsNullOrEmpty(clip.name))
					break;

				PreviewTextBlock.Text = clip.name;
				return;
			} while (false);

			SetUnknownPreviewText();
		}
		private void SetUnknownPreviewText() {
			PreviewTextBlock.Text = "(None)";
		}
	}
}
