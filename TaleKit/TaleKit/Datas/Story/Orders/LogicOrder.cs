using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// 변수를 조작하는 명령
	/// </summary>
	public class LogicOrder : OrderBase {

		public override OrderType OrderType => OrderType.Logic;

		public LogicOrder(StoryBlock ownerBlock) : base(ownerBlock) {

		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
