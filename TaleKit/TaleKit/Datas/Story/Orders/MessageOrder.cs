using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TaleKit.Datas.Editor;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// 대화씬에서의 텍스트대사 설정 명령
	/// </summary>
	public class MessageOrder : OrderBase {
		protected static MotionText ScriptText => UiManager.ScriptText;

		[EditableValue("화자", ValueEditorType.Text)]
		public string Talker {
			get; set;
		}
		[EditableValue("메세지", ValueEditorType.Text)]
		public string Message {
			get; set;
		}

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
