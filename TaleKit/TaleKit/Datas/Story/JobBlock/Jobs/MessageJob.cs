using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story.Jobs {
	/// <summary>
	/// 대화씬에서의 스크립트 텍스트 Job
	/// </summary>
	public class MessageJob : BlockJobBase {
		protected static MotionText ScriptText => UiManager.ScriptText;

		public string Talker;
		public string Message;

		public MessageJob(JobBlock ownerBlock) : base(ownerBlock) {
		}

		public override void Skip() {
			base.Skip();

			ScriptText.SkipMotion();
		}
	}
}
