extern alias GKitForUnity;
using GKit;
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
using TaleKit.Datas.UI;
using TaleKitEditor.Utility;
using UAnchorPreset = GKitForUnity.GKit.AnchorPreset;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements {
	/// <summary>
	/// UiRenderer.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class UiRenderer : UserControl {

		public UiItem Data {
			get; private set;
		}

		[Obsolete]
		public UiRenderer() {
			InitializeComponent();
		}
		public UiRenderer(UiItem data) {
			InitializeComponent();
			this.Data = data;
		}

		public void Render(bool renderChilds, bool rebuild) {
			SolidRenderer.Background = Data.color.ToColor().ToBrush();

			UpdateAlignment();

			if (renderChilds) {

				if(rebuild) {
					ChildItemContext.Children.Clear();
					foreach(UiItem data in Data.ChildItemList) {
						UiRenderer renderer = new UiRenderer(data);
						ChildItemContext.Children.Add(renderer);

						renderer.Render(renderChilds, rebuild);
					}
				} else {
					foreach(UiRenderer data in ChildItemContext.Children) {
						data.Render(renderChilds, rebuild);
					}
				}
			}
		}

		//Alignment
		private void UpdateAlignment() {
			//TODO : Remove test code
			Margin = new Thickness(10);
			MinWidth = 100;
			MinHeight = 100;

			VerticalAlignment verticalAlign;
			switch(Data.anchorPreset) {
				default:
				case UAnchorPreset.TopLeft:
				case UAnchorPreset.TopMid:
				case UAnchorPreset.TopRight:
				case UAnchorPreset.TopStretch:
					verticalAlign = VerticalAlignment.Top;
					break;
				case UAnchorPreset.MidLeft:
				case UAnchorPreset.MidMid:
				case UAnchorPreset.MidRight:
				case UAnchorPreset.MidStretch:
					verticalAlign = VerticalAlignment.Center;
					break;
				case UAnchorPreset.BotLeft:
				case UAnchorPreset.BotMid:
				case UAnchorPreset.BotRight:
				case UAnchorPreset.BotStretch:
					verticalAlign = VerticalAlignment.Bottom;
					break;
				case UAnchorPreset.StretchLeft:
				case UAnchorPreset.StretchMid:
				case UAnchorPreset.StretchRight:
				case UAnchorPreset.StretchAll:
					verticalAlign = VerticalAlignment.Stretch;
					break;
			}
			VerticalAlignment = verticalAlign;

			HorizontalAlignment horizontalAlign;
			switch(Data.anchorPreset) {
				default:
				case UAnchorPreset.TopLeft:
				case UAnchorPreset.MidLeft:
				case UAnchorPreset.BotLeft:
				case UAnchorPreset.StretchLeft:
					horizontalAlign = HorizontalAlignment.Left;
					break;
				case UAnchorPreset.TopMid:
				case UAnchorPreset.MidMid:
				case UAnchorPreset.BotMid:
				case UAnchorPreset.StretchMid:
					horizontalAlign = HorizontalAlignment.Center;
					break;
				case UAnchorPreset.TopRight:
				case UAnchorPreset.MidRight:
				case UAnchorPreset.BotRight:
				case UAnchorPreset.StretchRight:
					horizontalAlign = HorizontalAlignment.Right;
					break;
				case UAnchorPreset.TopStretch:
				case UAnchorPreset.MidStretch:
				case UAnchorPreset.BotStretch:
				case UAnchorPreset.StretchAll:
					horizontalAlign = HorizontalAlignment.Stretch;
					break;
			}
			HorizontalAlignment = horizontalAlign;
		}
	}
}
