using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story.Orders {
	/// <summary>
	/// 대화씬에서의 스크립트 텍스트 Job
	/// </summary>
	public class MessageOrder : OrderBase {
		protected static MotionText ScriptText => UiManager.ScriptText;

		public string Talker;
		public string Message;

		public MessageOrder(StoryBlock ownerBlock) : base(ownerBlock) {
		}

		public override void Skip() {
			base.Skip();

			ScriptText.SkipMotion();
		}
	}
}
