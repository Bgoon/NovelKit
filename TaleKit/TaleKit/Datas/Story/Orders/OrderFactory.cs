using System;

namespace TaleKit.Datas.Story {
	public static class OrderFactory {
		public static OrderBase CreateOrder(StoryBlock ownerBlock, OrderType orderType) {
			switch (orderType) {
				case OrderType.UI:
					return new Order_UI(ownerBlock);
				case OrderType.Logic:
					return new Order_Logic(ownerBlock);
				case OrderType.Event:
					return new Order_Event(ownerBlock);
				case OrderType.Clip:
					return new Order_Clip(ownerBlock);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
