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

		public static void PlaySlideInAnim(this Window window, float slideLength = 50f, float durationSec = 0.2f) {
			double slideDst = window.Left;
			double slideFrom = slideDst + slideLength;
			window.Left = slideFrom;
			
			IEasingFunction easeFunction = new PowerEase() { EasingMode = EasingMode.EaseOut };
			Duration slideDuration = new Duration(TimeSpan.FromSeconds(durationSec));
			DoubleAnimation slideAnim = new DoubleAnimation() {
				From = slideFrom,
				To = slideDst,
				Duration = slideDuration,
				EasingFunction = easeFunction,
			};
			window.BeginAnimation(Window.LeftProperty, slideAnim);

			DoubleAnimation opacityAnim = new DoubleAnimation() {
				From = 0d,
				To = 1d,
				Duration = slideDuration,
				EasingFunction = easeFunction,
			};
			window.BeginAnimation(Window.OpacityProperty, opacityAnim);
		}
	}
}
