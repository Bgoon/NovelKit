using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Motion;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI;

namespace TaleKit.Datas {
	public class TaleData {

		public MotionFile MotionFile {
			get; private set;
		}
		public UiFile UiFile {
			get; private set;
		}
		public StoryFile StoryFile {
			get; private set;
		}

		public TaleData() {
			MotionFile = new MotionFile();
			UiFile = new UiFile();
			StoryFile = new StoryFile();
		}

		public void PostInit() {
		}

		/// <summary>
		/// 프로젝트 경로에 데이터들을 저장합니다.
		/// </summary>
		/// <param name="directoryName"></param>
		public void Save(string directoryName) {

		}

		/// <summary>
		/// 클라이언트에서 플레이할 수 있는 데이터 파일로 내보냅니다.
		/// </summary>
		public void Export(string fileName) {
			JObject taleData = new JObject();

			taleData.Add("MotionFile", MotionFile.ToJObject());
			taleData.Add("UiFile", UiFile.ToJObject());
			taleData.Add("StoryFile", StoryFile.ToJObject());

			File.WriteAllText(fileName, taleData.ToString(), Encoding.UTF8);
		}
	}
}
