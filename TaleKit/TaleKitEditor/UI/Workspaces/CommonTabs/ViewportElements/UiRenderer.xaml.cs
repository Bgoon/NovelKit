using GKitForWPF;
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
using TaleKit.Datas.Resource;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UiItem;
using TaleKitEditor.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using UAnchorPreset = GKitForUnity.AnchorPreset;
using UAxisAnchor = GKitForUnity.AxisAnchor;
using Grid = System.Windows.Controls.Grid;
using Mathf = UnityEngine.Mathf;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements {
	/// <summary>
	/// UiRenderer.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class UiRenderer : UserControl {

		public UiFile EditingUiFile => Data.OwnerFile;
		public UiItemBase Data {
			get; private set;
		}

		private RotateTransform rotateTransform;

		[Obsolete]
		public UiRenderer() {
			InitializeComponent();
		}
		public UiRenderer(UiItemBase data) {
			InitializeComponent();

			this.Data = data;

			rotateTransform = new RotateTransform();
			this.RenderTransform = rotateTransform;
			this.RenderTransformOrigin = new Point(0.5f, 0.5f);

			List<Grid> dataContextList = new List<Grid>() {
				PanelContext,
				TextContext,
			};

			// Remove unused contexts
			if(data.itemType == UiItemType.Panel) {
				dataContextList.Remove(PanelContext);
			} else if(data.itemType == UiItemType.Text) {
				dataContextList.Remove(TextContext);
			}
			foreach(var dataContext in dataContextList) {
				dataContext.DetachParent();
			}
		}

		public void Render(bool renderChilds) {
			if(Data.itemType == UiItemType.Panel) {
				RenderPanel();
			} else if(Data.itemType == UiItemType.Text) {
				RenderText();
			}

			UpdateAlignment();

			if (renderChilds) {
				foreach(UiRenderer data in ChildItemContext.Children) {
					data.Render(renderChilds);
				}
			}
		}
		private void RenderPanel() {
			UiPanel panelData = Data as UiPanel;

			SolidRenderer.Background = panelData.color.ToColor().ToBrush();

			AssetItem imageAsset = panelData.GetImageAsset();
			if (imageAsset != null) {
				ImageRenderer.Source = new BitmapImage(new Uri(panelData.GetImageAsset().AssetFilename));
			} else {
				ImageRenderer.Source = null;
			}
		}
		private void RenderText() {
			UiText textData = Data as UiText;

			TextRenderer.Text = textData.text;
			FontFamily fontFamily;
			if(!string.IsNullOrEmpty(textData.fontFamily)) {
				fontFamily = new FontFamily(textData.fontFamily);
			} else {

				fontFamily = new FontFamily();
			}
			TextRenderer.FontFamily = fontFamily;
			TextRenderer.FontSize = Mathf.Max(1, textData.fontSize);
			TextRenderer.Foreground = textData.color.ToColor().ToBrush();
			
			switch(textData.anchor) {
				case TextAnchor.UpperLeft:
				case TextAnchor.MiddleLeft:
				case TextAnchor.LowerLeft:
					TextRenderer.TextAlignment = System.Windows.TextAlignment.Left;
					break;
				case TextAnchor.UpperCenter:
				case TextAnchor.MiddleCenter:
				case TextAnchor.LowerCenter:
					TextRenderer.TextAlignment = System.Windows.TextAlignment.Center;
					break;
				case TextAnchor.UpperRight:
				case TextAnchor.MiddleRight:
				case TextAnchor.LowerRight:
					TextRenderer.TextAlignment = System.Windows.TextAlignment.Right;
					break;
			}

			switch(textData.anchor) {
				case TextAnchor.UpperLeft:
				case TextAnchor.UpperCenter:
				case TextAnchor.UpperRight:
					TextRenderer.VerticalAlignment = VerticalAlignment.Top;
					break;
				case TextAnchor.MiddleLeft:
				case TextAnchor.MiddleCenter:
				case TextAnchor.MiddleRight:
					TextRenderer.VerticalAlignment = VerticalAlignment.Center;
					break;
				case TextAnchor.LowerLeft:
				case TextAnchor.LowerCenter:
				case TextAnchor.LowerRight:
					TextRenderer.VerticalAlignment = VerticalAlignment.Bottom;
					break;
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

			//Rotate
			rotateTransform.Angle = Data.rotation;
		}
	}
}
