using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.ModelEditor;
using GKitForUnity;
using UColor = UnityEngine.Color;
using UnityEngine;

namespace TaleKit.Datas.UI.UIItem {
	[Serializable]
	public class UIItem_Text : UIItemBase {

		[ValueEditorComponent_Header("Text Attributes")]
		[ValueEditor_TextBox("Text")]
		public string text;

		[ValueEditor_ColorBox("Font Color")]
		public UColor fontColor = UColor.black;

		[ValueEditor_TextBox("Font Family")]
		public string fontFamily;

		[ValueEditor_NumberBox("Font Size", NumberType.Int, minValue = 1)]
		public int fontSize = 11;

		[ValueEditor_EnumComboBox("Text Anchor")]
		public TextAnchor textAnchor;


		public UIItem_Text(UIFile ownerFile) : base(ownerFile, UIItemType.Text) {

		}
	}
}
