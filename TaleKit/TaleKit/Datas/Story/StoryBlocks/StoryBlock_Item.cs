using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using GKit.Json;

namespace TaleKit.Datas.Story {
	public class StoryBlock_Item : StoryBlockBase {
		public event Action<OrderBase> OrderAdded;
		public event Action<OrderBase> OrderRemoved;

		// Status
		public string DisplayDescription {
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

		// Data
		[ValueEditor_EnumComboBox("트리거")]
		public StoryBlockTrigger passTrigger;

		// [ Constructor ]
		public StoryBlock_Item(StoryFile ownerFile) : base(ownerFile, StoryBlockType.Item) {
			OrderList = new List<OrderBase>();

			passTrigger = StoryBlockTrigger.Click;
		}

		// [ Event ]
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

		// [ Control ]
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

		// [ Serialize ]
		public override JObject ToJObject() {
			JObject jBlock = new JObject();

			JObject jFields = new JObject();
			jBlock.Add("Fields", jFields);
			jFields.AddAttrFields<SavableFieldAttribute>(this);

			JArray jOrders = new JArray();
			jBlock.Add("Orders", jOrders);

			foreach (OrderBase order in OrderList) {
				jOrders.Add(order.ToJObject());
			}

			return jBlock;
		}
	}

}
