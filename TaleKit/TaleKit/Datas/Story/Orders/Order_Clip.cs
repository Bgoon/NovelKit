using Newtonsoft.Json.Linq;
using System;
using TaleKit.Datas.Story.StoryBlock;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// StoryBlock의 Clip을 재생하는 명령
	/// </summary>
	public class Order_Clip : OrderBase {

		public Order_Clip(StoryBlock_Item ownerBlock) : base(ownerBlock) {
			orderType = OrderType.Clip;
		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
