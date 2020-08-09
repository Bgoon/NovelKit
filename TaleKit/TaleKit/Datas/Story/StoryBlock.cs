using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TaleKit.Datas.Story {
	public class StoryBlock : StoryBlockBase {

		public event Action<OrderBase> OrderAdded;
		public event Action<OrderBase> OrderRemoved;

		public string DisplayName {
			get; set;
		}
		public List<OrderBase> OrderList {
			get; private set;
		}
		public bool IsComplete {
			get {
				for (int i = 0; i < OrderList.Count; ++i) {
					if (!OrderList[i].IsComplete) {
						return false;
					}
				}
				return true;
			}
		}

		public StoryBlock(StoryFile ownerFile) : base(ownerFile, StoryBlockType.StoryBlock) {
			OrderList = new List<OrderBase>();
		}

		public void OnEnter() {
			foreach (OrderBase job in OrderList) {
				job.OnStart();
			}
		}
		public void OnTick() {
			foreach (OrderBase job in OrderList) {
				job.OnTick();
			}
		}
		public void OnEnd() {
			foreach (OrderBase job in OrderList) {
				job.OnComplete();
			}
		}

		public void Skip() {
			for (int i = 0; i < OrderList.Count; ++i) {
				OrderList[i].Skip();
			}
		}

		public OrderBase AddOrder(OrderType orderType) {
			OrderBase order = OrderFactory.CreateOrder(this, orderType);
			OrderList.Add(order);

			OrderAdded?.Invoke(order);

			return order;
		}
		public void RemoveOrder(OrderBase order) {
			OrderList.Remove(order);

			OrderRemoved?.Invoke(order);
		}

		public override JObject ToJObject() {
			JObject jBlock = new JObject();

			//Add components
			JArray jOrders = new JArray();
			jBlock.Add("Orders", jOrders);

			foreach (OrderBase order in OrderList) {
				jOrders.Add(order.ToJObject());
			}

			return jBlock;
		}
	}

}
