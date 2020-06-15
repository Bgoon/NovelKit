using GKitForWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TaleKitEditor.UI.Utility {
	public static class WindowUtility {
		
		public static void SetFocusableWindow(this Window window) {
			FrameworkElement rootContext = window.Content as FrameworkElement;
			rootContext.Focusable = true;

			window.MouseDown += (object sender, MouseButtonEventArgs e) => {
				rootContext.Focus();
			};
		}

		public static void PlaySlideInAnim(this Window window, float slideLength = 10f) {
			//Slide animation
			const int SlideDurationMillisec = 300;

			double slideDstTop = window.Top;
			window.Top = slideDstTop + slideLength;

			IEasingFunction powerEasing = new PowerEase() { EasingMode = EasingMode.EaseOut };
			Duration slideDuration = new Duration(new TimeSpan(0, 0, 0, 0, SlideDurationMillisec));
			DoubleAnimation slideAnim = new DoubleAnimation() {
				To = slideDstTop,
				Duration = slideDuration,
				EasingFunction = powerEasing,
			};
			window.BeginAnimation(Window.TopProperty, slideAnim);
		}
	}
}
