using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Story.Jobs {
	public class EventJob : BlockJobBase {
		public string EventKey;

		public EventJob(JobBlock ownerBlock) : base(ownerBlock) {
			
		}
	}
}
