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

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.StoryBoardElements {
	/// <summary>
	/// StoryBlock.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockView : UserControl, ITreeItem {

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
				StoryBlock storyBlockData = data as StoryBlock;
				storyBlockData.OrderAdded += StoryBlockData_OrderAdded;
				storyBlockData.OrderRemoved += StoryBlockData_OrderRemoved;
			}
		}

		// [ Event ]
		private void StoryBlockData_OrderAdded(OrderBase obj) {
			UpdateOrderIndicator();
		}
		private void StoryBlockData_OrderRemoved(OrderBase obj) {
			UpdateOrderIndicator();
		}

		// [ Control ]
		public void SetDisplayName(string name) {
			PreviewTextBlock.Text = name;
		}

		public void SetSelected(bool isSelected) {
			ItemPanel.Background = (Brush)Application.Current.Resources[isSelected ? "ItemBackground_Selected" : "ItemBackground"];
		}

		private void UpdateOrderIndicator() {
			OrderIndicatorContext.Children.Clear();

			if (Data.Type == StoryBlockType.StoryBlock) {
				StoryBlock storyBlockData = (StoryBlock)Data;

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
