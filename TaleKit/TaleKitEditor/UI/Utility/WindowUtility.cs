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

		public static void PlayEaseInAnim(this FrameworkElement element, DependencyProperty animProperty, double fromValue, double toValue, float durationSec = 0.3f) {
			IEasingFunction easeFunction = new PowerEase() { EasingMode = EasingMode.EaseOut };
			DoubleAnimation anim = new DoubleAnimation() {
				From = fromValue,
				To = toValue,
				Duration = TimeSpan.FromSeconds(durationSec),
				EasingFunction = easeFunction,
			};
			Timeline.SetDesiredFrameRate(anim, 144);
			element.BeginAnimation(animProperty, anim);
		}

		public static void PlayMaskInAnim(this Window window, float durationSec = 0.3f) {
			IEasingFunction easeFunction = new PowerEase() { EasingMode = EasingMode.EaseOut };
			DoubleAnimation anim = new DoubleAnimation() {
				From = 0d,
				To = window.ActualWidth,
				Duration = TimeSpan.FromSeconds(durationSec),
				EasingFunction = easeFunction,
			};
			Timeline.SetDesiredFrameRate(anim, 144);
			window.BeginAnimation(Window.WidthProperty, anim);
		}

		public static void PlaySlideInAnim(this Window window, float slideLength = 50f, float durationSec = 0.2f) {
			double slideDst = window.Left;
			double slideFrom = slideDst + slideLength;
			window.Left = slideFrom;
			
			IEasingFunction easeFunction = new PowerEase() { EasingMode = EasingMode.EaseOut };
			DoubleAnimation anim = new DoubleAnimation() {
				From = slideFrom,
				To = slideDst,
				Duration = TimeSpan.FromSeconds(durationSec),
				EasingFunction = easeFunction,
			};
			Timeline.SetDesiredFrameRate(anim, 144);
			window.BeginAnimation(Window.LeftProperty, anim);
		}
	}
}
