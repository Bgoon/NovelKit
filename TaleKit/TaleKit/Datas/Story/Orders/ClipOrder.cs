using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// StoryBlock의 Clip을 재생하는 명령
	/// </summary>
	public class ClipOrder : OrderBase {

		public override OrderType OrderType => OrderType.Clip;

		public ClipOrder(StoryBlock ownerBlock) : base(ownerBlock) {

		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
