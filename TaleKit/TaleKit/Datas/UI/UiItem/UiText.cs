using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Editor;

namespace TaleKit.Datas.UI.UiItem {
	public class UiText : UiItemBase {

		[ValueEditorComponent_Header("Text UI")]
		[ValueEditor_TextBox("Text")]
		public string text;



		public UiText(UiFile ownerFile) : base(ownerFile, UiItemType.Text) {

		}
	}
}
