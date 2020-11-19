using GKitForWPF;
using GKitForWPF.UI.Controls;
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
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views {
	/// <summary>
	/// StoryBlock.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockView : UserControl, ITreeItem, IDisposable {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		//ITreeItem interface
		public string DisplayName {
			get; set;
		}
		public ITreeFolder ParentItem {
			get; set;
		}
		public FrameworkElement ItemContext => this;

		public string description;
		public readonly StoryBlockBase Data;

		public StoryBlockType BlockType => Data.blockType;

		private IBlockContent viewContent;


		// [ Constructor ]
		public StoryBlockView() {
			InitializeComponent();
		}
		public StoryBlockView(StoryBlockBase data) : this() {
			this.Data = data;

			// Register events
			if (BlockType == StoryBlockType.Item) {
				viewContent = new BlockContent_Item(this);

				StoryBlock_Item storyBlockData = data as StoryBlock_Item;
				storyBlockData.OrderAdded += StoryBlockData_OrderAdded;
				storyBlockData.OrderRemoved += StoryBlockData_OrderRemoved;

				StoryBlockData_OrderCountChanged();
				StoryBlockData_PassTriggerChanged();
			} else if(BlockType == StoryBlockType.Clip) {
				viewContent = new BlockContent_Clip(this);
			}
			ContentContext.Children.Add(viewContent as FrameworkElement);

			VisibleButton.RegisterClickEvent(VisibleButton_Click, true);

			Data.ModelUpdated += Data_ModelUpdated;
		}
		public void Dispose() {
			if(BlockType == StoryBlockType.Item) {
				StoryBlock_Item storyBlockData = Data as StoryBlock_Item;
				storyBlockData.OrderAdded -= StoryBlockData_OrderAdded;
				storyBlockData.OrderRemoved -= StoryBlockData_OrderRemoved;
			}

			Data.ModelUpdated -= Data_ModelUpdated;
		}


		// [ Event ]
		private void StoryBlockData_OrderAdded(OrderBase obj) {
			StoryBlockData_OrderCountChanged();
		}
		private void StoryBlockData_OrderRemoved(OrderBase obj) {
			StoryBlockData_OrderCountChanged();
		}
		private void StoryBlockData_OrderCountChanged() {
			(viewContent as BlockContent_Item).UpdateOrderIndicator();
		}

		private void StoryBlockData_PassTriggerChanged() {
			(viewContent as BlockContent_Item).UpdatePassTriggerIcon();
		}

		private void VisibleButton_Click() {
			Data.isVisible = !Data.isVisible;
			
			UpdateVisibleButton();
			Data.OwnerFile.UiCacheManager.ClearCacheAfterBlock(Data);
			StoryBlockTab.ApplyBlockToSelectionToRenderer();
		}

		private void Data_ModelUpdated(EditableModel model, FieldInfo fieldInfo, object editorView) {
			if(BlockType == StoryBlockType.Item) {
				StoryBlock_Item itemBlock = Data as StoryBlock_Item;
				switch(fieldInfo.Name) {
					case nameof(itemBlock.passTrigger):
						StoryBlockData_PassTriggerChanged();
						break;
				}

			} else if(BlockType == StoryBlockType.Clip) {

			}
		}

		// [ Control ]
		public void SetDisplayName(string name) {
			if(BlockType == StoryBlockType.Item) {
				(viewContent as BlockContent_Item).PreviewTextBlock.Text = name;
			} else if(BlockType == StoryBlockType.Clip) {
				(viewContent as BlockContent_Clip).NameEditText.Text = name;
			}
		}

		public void SetSelected(bool isSelected) {
			ItemPanel.Background = (Brush)Application.Current.Resources[isSelected ? "ItemBackground_Selected" : "ItemBackground"];
		}

		// [ UI Update ]
		// TODO : Load이후 호출해줄것
		public void UpdateVisibleButton() {
			VisibleButton.Opacity = Data.isVisible ? 1d : 0d;
		}
	}
}
