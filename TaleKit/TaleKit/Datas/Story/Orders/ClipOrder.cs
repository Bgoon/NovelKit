using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
