using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Story.Jobs;

namespace TaleKit.Story {
	public class JobBlock {

		public string DisplayName {
			get; set;
		}
		public List<BlockJobBase> JobList {
			get; private set;
		}
		public bool IsComplete {
			get {
				for(int i=0; i<JobList.Count; ++i) {
					if(!JobList[i].IsComplete) {
						return false;
					}
				}
				return true;
			}
		}

		public JobBlock() {
			JobList = new List<BlockJobBase>();
		}

		public void OnEnter() {
			foreach (BlockJobBase job in JobList) {
				job.OnStart();
			}
		}
		public void OnTick() {
			foreach(BlockJobBase job in JobList) {
				job.OnTick();
			}
		}
		public void OnEnd() {
			foreach (BlockJobBase job in JobList) {
				job.OnComplete();
			}
		}

		public void Skip() {
			for(int i=0; i<JobList.Count; ++i) {
				JobList[i].Skip();
			}
		}
	}
}
