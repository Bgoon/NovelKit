using GKitForWPF.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.Utility;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using UColor = UnityEngine.Color;
using UGRect = GKitForUnity.GRect;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements.UIContents {
	public partial class UIContent_Text : UserControl, IUIContent {
		public UIRenderer OwnerRenderer {
			get; private set;
		}

		// [ Constructor ]
		[Obsolete]
		public UIContent_Text() {
			InitializeComponent();
		}
		public UIContent_Text(UIRenderer ownerRenderer) {
			InitializeComponent();

			this.OwnerRenderer = ownerRenderer;
		}

		public void Render(UIItemBase data) {
			UIItem_Text textData = data as UIItem_Text;

			OwnerRenderer.SetProperty(textData, nameof(textData.text), (object value) => { ContentTextBlock.Text = (string)value; });
			OwnerRenderer.SetProperty(textData, nameof(textData.fontFamily), (object value) => {
				string fontFamilyName = (string)value;
				FontFamily fontFamily;
				if (!string.IsNullOrEmpty(fontFamilyName)) {
					fontFamily = new FontFamily(fontFamilyName);
				} else {

					fontFamily = new FontFamily();
				}
				ContentTextBlock.FontFamily = fontFamily;
			});
			OwnerRenderer.SetProperty(textData, nameof(textData.fontSize), (object value) => { ContentTextBlock.FontSize = Mathf.Max(1, (int)value); });
			OwnerRenderer.SetProperty(textData, nameof(textData.fontColor), (object value) => { ContentTextBlock.Foreground = ((UColor)value).ToColor().ToBrush(); });
			OwnerRenderer.SetProperty(textData, nameof(textData.textAnchor), (object value) => {
				TextAnchor textAnchor = (TextAnchor)value;
				switch (textAnchor) {
					case TextAnchor.UpperLeft:
					case TextAnchor.MiddleLeft:
					case TextAnchor.LowerLeft:
						ContentTextBlock.TextAlignment = System.Windows.TextAlignment.Left;
						break;
					case TextAnchor.UpperCenter:
					case TextAnchor.MiddleCenter:
					case TextAnchor.LowerCenter:
						ContentTextBlock.TextAlignment = System.Windows.TextAlignment.Center;
						break;
					case TextAnchor.UpperRight:
					case TextAnchor.MiddleRight:
					case TextAnchor.LowerRight:
						ContentTextBlock.TextAlignment = System.Windows.TextAlignment.Right;
						break;
				}

				switch (textAnchor) {
					case TextAnchor.UpperLeft:
					case TextAnchor.UpperCenter:
					case TextAnchor.UpperRight:
						ContentTextBlock.VerticalAlignment = VerticalAlignment.Top;
						break;
					case TextAnchor.MiddleLeft:
					case TextAnchor.MiddleCenter:
					case TextAnchor.MiddleRight:
						ContentTextBlock.VerticalAlignment = VerticalAlignment.Center;
						break;
					case TextAnchor.LowerLeft:
					case TextAnchor.LowerCenter:
					case TextAnchor.LowerRight:
						ContentTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
						break;
				}
			});
		}
	}
}
