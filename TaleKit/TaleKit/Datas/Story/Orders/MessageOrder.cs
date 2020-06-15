using Newtonsoft.Json.Linq;
using System;
using TaleKit.Datas.Editor;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// 대화씬에서의 텍스트대사 설정 명령
	/// </summary>
	public class MessageOrder : OrderBase {
		protected static MotionText ScriptText => UiManager.ScriptText;

		[ValueEditor_TextBox("화자")]
		public string talker;
		[ValueEditor_TextBox("메세지")]
		public string message;

		public override OrderType OrderType => OrderType.Message;

		public MessageOrder(StoryBlock ownerBlock) : base(ownerBlock) {
		}

		public override void Skip() {
			base.Skip();

			ScriptText.SkipMotion();
		}

		public override JObject ToJObject() {
			throw new NotImplementedException();
		}
	}
}
