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
using UnityEngine.UIElements;
using UAnchorPreset = GKitForUnity.GKit.AnchorPreset;
using UAxisAnchor = GKitForUnity.GKit.AxisAnchor;

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
			//AnchorPreset
			UAxisAnchor axisAnchorX = Data.AnchorX;
			UAxisAnchor axisAnchorY = Data.AnchorY;
			
			HorizontalAlignment = axisAnchorX.ToHorizontalAlignment();
			VerticalAlignment = axisAnchorY.ToVerticalAlignment();

			//Margin
			Margin = new Thickness(Data.margin.xMin, Data.margin.yMax, Data.margin.xMax, Data.margin.yMin);

			//Size
			if(axisAnchorX == UAxisAnchor.Stretch) {
				Width = double.NaN;
			} else {
				Width = Data.size.x;
			}

			if(axisAnchorY == UAxisAnchor.Stretch) {
				Height = double.NaN;
			} else {
				Height = Data.size.y;
			}
		}
	}
}
