using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Editor;
using GKitForUnity;
using UColor = UnityEngine.Color;
using UnityEngine;

namespace TaleKit.Datas.UI.UiItem {
	public class UiText : UiItemBase {

		[ValueEditorComponent_Header("Text Attributes")]
		[ValueEditor_TextBox("Text")]
		public string text;

		[ValueEditor_ColorBox("Color")]
		public UColor color = UColor.black;

		[ValueEditor_TextBox("Font Family")]
		public string fontFamily;

		[ValueEditor_NumberBox("Font Size", NumberType.Int, minValue = 1)]
		public int fontSize = 11;

		[ValueEditor_EnumComboBox("Text Anchor")]
		public TextAnchor anchor;


		public UiText(UiFile ownerFile) : base(ownerFile, UiItemType.Text) {

		}
	}
}
