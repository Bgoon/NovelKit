using Newtonsoft.Json.Linq;
using System;
using TaleKit.Datas.Editor;

namespace TaleKit.Datas.Story {
	public class EventOrder : OrderBase {
		[ValueEditor_TextBox("이벤트 키")]
		public string eventKey;

		public override OrderType OrderType => OrderType.Event;

		public EventOrder(StoryBlock ownerBlock) : base(ownerBlock) {

		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
