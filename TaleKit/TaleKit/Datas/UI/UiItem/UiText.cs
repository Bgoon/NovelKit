using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Editor;
using UColor = UnityEngine.Color;

namespace TaleKit.Datas.UI.UiItem {
	public class UiText : UiItemBase {

		[ValueEditorComponent_Header("Text Attributes")]
		[ValueEditor_TextBox("Text")]
		public string text;

		[ValueEditor_ColorBox("Color")]
		public UColor color = UColor.black;



		public UiText(UiFile ownerFile) : base(ownerFile, UiItemType.Text) {

		}
	}
}
