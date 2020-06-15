using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Motion {
	public class MotionFile : ITaleDataFile {
		public DateTime exportedTime;

		public PenMotion.Datas.MotionFile motionFileData;

		public MotionFile() {
		}

		public void SetMotionFileData(PenMotion.Datas.MotionFile motionFile) {
			this.motionFileData = motionFile;
		}

		public bool Save(string filename) {
			motionFileData.Save(filename);
			return true;
		}
		public bool Load(string filename) {
			motionFileData.Load(filename);
			return true;
		}

		public JObject ToJObject() {
			return motionFileData.ToJObject();
		}
	}
}
