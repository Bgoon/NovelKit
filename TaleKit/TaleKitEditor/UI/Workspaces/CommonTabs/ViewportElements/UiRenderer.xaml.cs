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
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs;
using TaleKit.Datas.Story;
using System.Reflection;
using TaleKit.Datas.Asset;
using TaleKit.Datas;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements.UIContents;
using UAnchorPreset = GKitForUnity.AnchorPreset;
using UAxisAnchor = GKitForUnity.AxisAnchor;
using Grid = System.Windows.Controls.Grid;
using Mathf = UnityEngine.Mathf;
using UColor = UnityEngine.Color;
using UGRect = GKitForUnity.GRect;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements {
	public partial class UIRenderer : UserControl {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static StoryBlockTab StoryBlockTab => MainWindow.StoryWorkspace.StoryBlockTab;

		// Reference
		public TaleData EditingTaleData => Data.OwnerFile.OwnerTaleData;
		public AssetManager AssetManager => EditingTaleData.AssetManager;
		public StoryFile EditingStoryFile => EditingTaleData.StoryFile;
		public UIFile EditingUIFile => Data.OwnerFile;

		public UIItemBase Data {
			get; private set;
		}
		public UIItemBase RenderingData {
			get; private set;
		}

		// Member
		private RotateTransform rotateTransform;
		private IUIContent UIContent;

		[Obsolete]
		public UIRenderer() {
			InitializeComponent();
		}
		public UIRenderer(UIItemBase data) {
			InitializeComponent();

			this.Data = data;
			this.RenderingData = data.Clone() as UIItemBase;

			rotateTransform = new RotateTransform();
			this.RenderTransform = rotateTransform;
			this.RenderTransformOrigin = new Point(0.5f, 0.5f);


			// Remove unused contexts
			switch(data.itemType) {
				case UIItemType.Panel:
					UIContent = new UIContent_Panel(this);
					break;
				case UIItemType.Text:
					UIContent = new UIContent_Text(this);
					break;
				case UIItemType.MotionText:
					UIContent = new UIContent_MotionText(this);
					break;
			}
			ContentContext.Children.Add(UIContent as FrameworkElement);

			FocusBox.Visibility = System.Windows.Visibility.Collapsed;
		}

		public void Render(bool renderChilds = false) {
			// Render data
			RenderFromData(Data);

			// Render childs
			if (renderChilds) {
				foreach (UIRenderer childRenderer in ChildItemContext.Children) {
					childRenderer.Render(renderChilds);
				}
			}
		}
		public void RenderFromData(UIItemBase data) {
			RenderingData.CopyDataFrom(data);

			UIContent.Render(data);
			RenderBase(data);
		}

		private void RenderBase(UIItemBase data) {
			//AnchorPreset and Size
			SetProperty(data, nameof(data.AnchorX), (object value) => {
				UAxisAnchor axisAnchorX = (UAxisAnchor)value;
				HorizontalAlignment = axisAnchorX.ToHorizontalAlignment();

				if (axisAnchorX == UAxisAnchor.Stretch) {
					Width = double.NaN;
				} else {
					Width = data.size.x;
				}
			});
			SetProperty(data, nameof(data.AnchorY), (object value) => {
				UAxisAnchor axisAnchorY = (UAxisAnchor)value;
				VerticalAlignment = axisAnchorY.ToVerticalAlignment();

				if (axisAnchorY == UAxisAnchor.Stretch) {
					Height = double.NaN;
				} else {
					Height = data.size.y;
				}
			});

			//Margin
			SetProperty(data, nameof(data.margin), (object value) => {
				UGRect margin = (UGRect)value;
				Margin = new Thickness(margin.xMin, margin.yMax, margin.xMax, margin.yMin);
			});

			//Rotate
			SetProperty(data, nameof(data.rotation), (object value) => { rotateTransform.Angle = (float)value; });
		}

		public void SetFocusBoxVisible(bool visible) {
			FocusBox.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
		}

		internal void SetProperty(UIItemBase data, string propertyName, Arg1Delegate<object> applyPropertyDelegate) {
			// UI Editor에서는 모두 적용, StoryBoard에서는 키프레임만 적용
			if(!data.IsKeyFrameModel || data.KeyFieldNameHashSet.Contains(propertyName)) {
				object property;

				MemberInfo member = data.GetType().GetMember(propertyName).FirstOrDefault();
				if(member is FieldInfo) {
					property = (member as FieldInfo).GetValue(data);
				} else if(member is PropertyInfo) {
					property = (member as PropertyInfo).GetValue(data);
				} else {
					throw new Exception($"Can't find property '{propertyName}'.");
				}
				applyPropertyDelegate(property);
			}
		}
	}
}
