using Newtonsoft.Json.Linq;
using System;

namespace TaleKit.Datas.Motion {
	public class MotionFile : ITaleDataFile {
		public readonly TaleData OwnerTaleData;

		public DateTime exportedTime;

		public PenMotion.Datas.MotionFile motionData;

		public MotionFile(TaleData ownerTaleData, PenMotion.Datas.MotionFile motionData) {
			this.OwnerTaleData = ownerTaleData;

			this.motionData = motionData;
		}

		public JObject ToJObject() {
			return motionData.ToJObject();
		}
		public bool LoadFromJson(JObject jMotionFile) {
			motionData.LoadFromJson(jMotionFile);
			return true;
		}

	}
}
