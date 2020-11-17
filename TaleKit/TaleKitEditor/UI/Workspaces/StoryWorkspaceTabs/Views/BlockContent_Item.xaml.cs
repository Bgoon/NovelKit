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
using TaleKit.Datas.Story.StoryBlock;
using TaleKitEditor.Resources.VectorImages;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views {
	/// <summary>
	/// BlockContent_Item.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class BlockContent_Item : UserControl, IBlockContent {

		public readonly StoryBlockView OwnerBlockView;
		public StoryBlockBase Data => OwnerBlockView.Data;

		public BlockContent_Item(StoryBlockView ownerBlockView) {
			InitializeComponent();

			this.OwnerBlockView = ownerBlockView;
		}

		public void UpdateOrderIndicator() {
			OrderIndicatorContext.Children.Clear();

			if (Data.blockType == StoryBlockType.Item) {
				StoryBlock_Item storyBlockData = (StoryBlock_Item)Data;

				Dictionary<OrderType, OrderIndicator> indicatorDict = new Dictionary<OrderType, OrderIndicator>();

				foreach (OrderBase order in storyBlockData.OrderList) {
					if (indicatorDict.ContainsKey(order.orderType)) {
						indicatorDict[order.orderType].Count++;
					} else {
						OrderIndicator indicator = new OrderIndicator();
						indicator.OrderType = order.orderType;
						indicator.Count = 1;
						OrderIndicatorContext.Children.Add(indicator);

						indicatorDict.Add(order.orderType, indicator);
					}
				}
			}
		}
		public void UpdatePassTriggerIcon() {
			PassTriggerIconContext.Children.Clear();

			UserControl icon = null;
			switch((Data as StoryBlock_Item).passTrigger) {
				case StoryBlockTrigger.Click:
					icon = new PassTrigger_Click();
					break;
				case StoryBlockTrigger.Auto:
					icon = new PassTrigger_Auto();
					break;
			}
			if(icon != null) {
				PassTriggerIconContext.Children.Add(icon);
			}
		}
	}
}
