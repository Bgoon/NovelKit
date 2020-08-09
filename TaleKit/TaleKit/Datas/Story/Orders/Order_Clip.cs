using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// StoryBlock의 Clip을 재생하는 명령
	/// </summary>
	public class Order_Clip : OrderBase {

		public override OrderType OrderType => OrderType.Clip;

		public Order_Clip(StoryBlock ownerBlock) : base(ownerBlock) {

		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
