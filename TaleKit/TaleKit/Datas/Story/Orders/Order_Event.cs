using Newtonsoft.Json.Linq;
using System;
using TaleKit.Datas.ModelEditor;

namespace TaleKit.Datas.Story {
	public class Order_Event : OrderBase {
		[ValueEditor_TextBox("이벤트 키")]
		public string eventKey;

		public override OrderType OrderType => OrderType.Event;

		public Order_Event(StoryBlock_Item ownerBlock) : base(ownerBlock) {

		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
