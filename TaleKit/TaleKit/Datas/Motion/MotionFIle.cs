using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Motion {
	public class MotionFile : ITaleDataFile {
		public readonly TaleData OwnerTaleData;

		public DateTime exportedTime;

		public PenMotion.Datas.MotionFile motionFileData;

		public MotionFile(TaleData ownerTaleData) {
			this.OwnerTaleData = ownerTaleData;
		}

		public void SetMotionFileData(PenMotion.Datas.MotionFile motionFile) {
			this.motionFileData = motionFile;
		}

		public JObject ToJObject() {
			return motionFileData.ToJObject();
		}
		public bool LoadFromJson(JObject jMotionFile) {
			motionFileData.LoadFromJson(jMotionFile);
			return true;
		}

	}
}
