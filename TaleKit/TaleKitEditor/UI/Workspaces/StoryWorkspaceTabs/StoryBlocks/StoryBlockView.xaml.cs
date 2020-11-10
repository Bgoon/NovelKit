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
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.StoryBlocks {
	/// <summary>
	/// StoryBlock.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockView : UserControl, ITreeItem {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		public string description;
		public readonly StoryBlockBase Data;

		//ITreeItem interface
		public string DisplayName => PreviewTextBlock.Text;
		public ITreeFolder ParentItem {
			get; set;
		}
		public FrameworkElement ItemContext => this;


		// [ Constructor ]
		public StoryBlockView() {
			InitializeComponent();
		}
		public StoryBlockView(StoryBlockBase data) : this() {
			this.Data = data;

			// Register events
			if (data.Type == StoryBlockType.StoryBlock) {
				StoryBlock_Item storyBlockData = data as StoryBlock_Item;
				storyBlockData.OrderAdded += StoryBlockData_OrderAdded;
				storyBlockData.OrderRemoved += StoryBlockData_OrderRemoved;
			}
			VisibleButton.RegisterClickEvent(VisibleButton_Click, true);
		}

		// [ Event ]
		private void StoryBlockData_OrderAdded(OrderBase obj) {
			UpdateOrderIndicator();
		}
		private void StoryBlockData_OrderRemoved(OrderBase obj) {
			UpdateOrderIndicator();
		}
		private void VisibleButton_Click() {
			Data.isVisible = !Data.isVisible;
			
			UpdateVisibleButton();
			Data.OwnerFile.UiCacheManager.ClearCacheAfterBlock(Data);
			StoryBlockTab.ApplyBlockToSelectionToRenderer();
		}

		// [ Control ]
		public void SetDisplayName(string name) {
			PreviewTextBlock.Text = name;
		}

		public void SetSelected(bool isSelected) {
			ItemPanel.Background = (Brush)Application.Current.Resources[isSelected ? "ItemBackground_Selected" : "ItemBackground"];
		}

		// [ UI Update ]
		// TODO : Load이후 호출해줄것
		public void UpdateVisibleButton() {
			VisibleButton.Opacity = Data.isVisible ? 1d : 0d;
		}
		private void UpdateOrderIndicator() {
			OrderIndicatorContext.Children.Clear();

			if (Data.Type == StoryBlockType.StoryBlock) {
				StoryBlock_Item storyBlockData = (StoryBlock_Item)Data;

				Dictionary<OrderType, OrderIndicator> indicatorDict = new Dictionary<OrderType, OrderIndicator>();

				foreach (OrderBase order in storyBlockData.OrderList) {
					if(indicatorDict.ContainsKey(order.OrderType)) {
						indicatorDict[order.OrderType].Count++;
					} else {
						OrderIndicator indicator = new OrderIndicator();
						indicator.OrderType = order.OrderType;
						indicator.Count = 1;
						OrderIndicatorContext.Children.Add(indicator);

						indicatorDict.Add(order.OrderType, indicator);
					}
				}
			}
		}

	}
}
