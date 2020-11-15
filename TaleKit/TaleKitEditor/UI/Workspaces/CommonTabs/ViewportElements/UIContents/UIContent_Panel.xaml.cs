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
using TaleKit.Datas.Asset;
using TaleKit.Datas.Resource;
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.Utility;
using Mathf = UnityEngine.Mathf;
using UColor = UnityEngine.Color;
using UGRect = GKitForUnity.GRect;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements.UIContents {
	public partial class UIContent_Panel : UserControl, IUIContent {

		public UIRenderer OwnerRenderer {
			get; private set;
		}

		// [ Constructor ]
		[Obsolete]
		public UIContent_Panel() {
			InitializeComponent();
		}
		public UIContent_Panel(UIRenderer ownerRenderer) {
			InitializeComponent();

			this.OwnerRenderer = ownerRenderer;
		}

		public void Render(UIItemBase data) {
			UIItem_Panel panelData = data as UIItem_Panel;

			OwnerRenderer.SetProperty(panelData, nameof(panelData.color), (object value) => { SolidRenderer.Background = ((UColor)value).ToColor().ToBrush(); });

			Arg1Delegate<object> applyImage = (object value) => {

				AssetItem imageAsset = OwnerRenderer.AssetManager.GetAsset(panelData.imageAssetKey);
				if (imageAsset != null) {
					BitmapImage image = new BitmapImage(new Uri(panelData.GetImageAsset().AssetFilename));

					ImageRenderer.Source = image;
					NinePatchImageRenderer.ImageSource = image;

					if (panelData.useNinePatch) {
						ImageRenderer.Visibility = Visibility.Collapsed;
						NinePatchImageRenderer.Visibility = Visibility.Visible;
					} else {
						ImageRenderer.Visibility = Visibility.Visible;
						NinePatchImageRenderer.Visibility = Visibility.Collapsed;
					}
				} else {
					NinePatchImageRenderer.ImageSource = null;
					ImageRenderer.Source = null;
				}
			};
			OwnerRenderer.SetProperty(panelData, nameof(panelData.useNinePatch), applyImage);
			OwnerRenderer.SetProperty(panelData, nameof(panelData.imageAssetKey), applyImage);
			OwnerRenderer.SetProperty(panelData, nameof(panelData.ninePatchSideAspect), (object value) => {
				UGRect ninePatchSideAspect = (UGRect)value;

				NinePatchImageRenderer.SideAspect = new Thickness(ninePatchSideAspect.xMin, ninePatchSideAspect.yMax, ninePatchSideAspect.xMax, ninePatchSideAspect.yMin);
			});
		}
	}
}
