using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story.Orders {
	public class EventOrder : OrderBase {
		public string EventKey;

		public EventOrder(StoryBlock ownerBlock) : base(ownerBlock) {
			
		}
	}
}
