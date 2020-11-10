using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// 변수를 조작하는 명령
	/// </summary>
	public class Order_Logic : OrderBase {

		public override OrderType OrderType => OrderType.Logic;

		public Order_Logic(StoryBlock_Item ownerBlock) : base(ownerBlock) {

		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
