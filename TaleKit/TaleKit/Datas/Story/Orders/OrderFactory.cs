using System;

namespace TaleKit.Datas.Story {
	public static class OrderFactory {
		public static OrderBase CreateOrder(StoryBlock ownerBlock, OrderType orderType) {
			switch (orderType) {
				case OrderType.Message:
					return new MessageOrder(ownerBlock);
				case OrderType.Ui:
					return new UiOrder(ownerBlock);
				case OrderType.Logic:
					return new LogicOrder(ownerBlock);
				case OrderType.Event:
					return new EventOrder(ownerBlock);
				case OrderType.Clip:
					return new ClipOrder(ownerBlock);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
