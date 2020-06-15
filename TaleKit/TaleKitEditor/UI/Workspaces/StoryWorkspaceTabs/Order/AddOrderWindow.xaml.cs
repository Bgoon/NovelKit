using GKitForWPF;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.Utility;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class AddOrderWindow : Window {

		private Vector2 windowTalePosition;

		public StoryBlock OwnerBlock {
			get; private set;
		}

		private bool isClosing;
		private OrderTypeItemView[] itemViews;

		public AddOrderWindow() {
			//For WPF designer
			InitializeComponent();
		}
		public AddOrderWindow(StoryBlock ownerBlock, Vector2 windowTalePosition) : this() {
			this.OwnerBlock = ownerBlock;
			this.windowTalePosition = windowTalePosition;

			CreateTypeItems(); 
		}

		private void Window_ContentRendered(object sender, EventArgs e) {
			Vector2 windowPos = windowTalePosition;
			windowPos += -(Vector2)TailShape.TranslatePoint(new Point((float)TailShape.ActualWidth, (float)-TailShape.ActualHeight * 0.5f), this);

			Left = windowPos.x;
			Top = windowPos.y;

			PlayShowingAnimation();
			Opacity = 1d;
		}
		private void Window_Deactivated(object sender, EventArgs e) {
			if (!isClosing) {
				Close();
			}
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			isClosing = true;
		}

		private void TypeItemView_Click(OrderType obj) {
			if(!isClosing) {
				OwnerBlock.AddOrder(obj);

				Close();
			}
		}

		private void CreateTypeItems() {
			string[] orderTypeTexts = Enum.GetNames(typeof(OrderType));
			itemViews = new OrderTypeItemView[orderTypeTexts.Length];

			for (int i = 0; i < orderTypeTexts.Length; ++i) {
				string orderTypeText = orderTypeTexts[i];

				OrderTypeItemView typeItemView = itemViews[i] = new OrderTypeItemView();
				typeItemView.TypeNameText = orderTypeText;
				typeItemView.OrderType = (OrderType)Enum.Parse(typeof(OrderType), orderTypeText);
				typeItemView.Click += TypeItemView_Click;

				if (i == orderTypeTexts.Length - 1) {
					typeItemView.IsSeparatorVisible = false;
				}

				TypeItemStackPanel.Children.Add(typeItemView);
			}
		}

		private void PlayShowingAnimation() {
			//Slide animation
			this.PlaySlideInAnim();

			//BG alpha animation
			IEasingFunction powerEasing = new PowerEase() { EasingMode = EasingMode.EaseOut };

			Color startColor = "#FFFFFF55".ToColor();
			Color endColor = "#FFFFFF00".ToColor();
			const int AlphaDurationMillisec = 400;
			for(int i=0; i<itemViews.Length; ++i) {
				int reverseI = itemViews.Length - 1 - i;

				Duration alphaDuration = new Duration(new TimeSpan(0, 0, 0, 0, AlphaDurationMillisec));
				ColorAnimation alhpaAnim = new ColorAnimation() {
					BeginTime = new TimeSpan(0, 0, 0, 0, reverseI * 30),
					From = startColor,
					To = endColor,
					Duration = alphaDuration,
					EasingFunction = powerEasing,
				};
				OrderTypeItemView itemView = itemViews[i];
				itemView.Background = new SolidColorBrush(startColor);
				itemView.Background.BeginAnimation(SolidColorBrush.ColorProperty, alhpaAnim);
			}
		}


	}
}
