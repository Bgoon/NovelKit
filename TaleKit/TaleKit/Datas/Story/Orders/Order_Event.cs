using Newtonsoft.Json.Linq;
using System;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.Story.StoryBlock;

namespace TaleKit.Datas.Story {
	public class Order_Event : OrderBase {
		[ValueEditor_TextBox("이벤트 키")]
		public string eventKey;

		public Order_Event(StoryBlock_Item ownerBlock) : base(ownerBlock) {
			orderType = OrderType.Event;
		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
