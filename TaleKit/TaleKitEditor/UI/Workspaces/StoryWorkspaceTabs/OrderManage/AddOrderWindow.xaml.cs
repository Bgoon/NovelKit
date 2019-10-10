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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaleKit.Datas.Story;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public partial class AddOrderWindow : Window {

		private Vector2 dstPosition;

		public StoryBlock OwnerBlock {
			get; private set;
		}

		private bool isClosing;

		public AddOrderWindow() {
			InitializeComponent();
			CreateTypeItems(); 
		}
		public AddOrderWindow(StoryBlock ownerBlock, Vector2 dstPosition) : this() {
			this.OwnerBlock = ownerBlock;
			this.dstPosition = dstPosition;

		}
		public void ShowNoFlicker() {
			Opacity = 0d;

			Show();
		}

		private void CreateTypeItems() {
			string[] orderTypeTexts = Enum.GetNames(typeof(OrderType));

			for (int i = 0; i < orderTypeTexts.Length; ++i) {
				string orderTypeText = orderTypeTexts[i];

				OrderTypeItemView typeItemView = new OrderTypeItemView();
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
			const int DurationMillisec = 300;

			double dstTop = Top;
			Top = dstTop + 10d;
			DependencyObject targetElement = RootContext;

			//Create animation
			IEasingFunction easing = new PowerEase() { EasingMode = EasingMode.EaseOut };
			Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, DurationMillisec));
			DoubleAnimation anim = new DoubleAnimation() {
				To = dstTop,
				Duration = duration,
				EasingFunction = easing,
			};

			this.BeginAnimation(Window.TopProperty, anim);
		}

		private void Window_ContentRendered(object sender, EventArgs e) {
			Vector2 windowPos = dstPosition;
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
	}
}
