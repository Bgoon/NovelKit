using Newtonsoft.Json.Linq;
using System;
using TaleKit.Datas.Story.StoryBlock;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// 변수를 조작하는 명령
	/// </summary>
	public class Order_Logic : OrderBase {

		public Order_Logic(StoryBlock_Item ownerBlock) : base(ownerBlock) {
			orderType = OrderType.Logic;
		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
