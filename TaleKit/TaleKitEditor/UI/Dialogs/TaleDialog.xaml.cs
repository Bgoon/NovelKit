using GKitForWPF;
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
using System.Windows.Shapes;
using TaleKitEditor.UI.Utility;

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// TaleDialog.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TaleDialog : Window {

		private Vector2 talePosition;

		private bool isClosing;

		public static void Show(FrameworkElement content, Vector2 talePosition) {
			TaleDialog dialog = new TaleDialog(talePosition);
			dialog.ContentContext.Children.Add(content);

			dialog.Show();
		}

		[Obsolete]
		internal TaleDialog() {
			InitializeComponent();
		}
		public TaleDialog(Vector2 talePosition) {
			Opacity = 0d;
			InitializeComponent();

			this.talePosition = talePosition;
		}

		private void Window_ContentRendered(object sender, EventArgs e) {
			Vector2 windowPos = talePosition;
			windowPos += -(Vector2)TailShape.TranslatePoint(new Point((float)TailShape.ActualWidth, (float)TailShape.ActualHeight * 0.5f), this);

			Left = windowPos.x;
			Top = windowPos.y;

			this.PlaySlideInAnim();

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
		private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
			ResetFocus();
		}
		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				ResetFocus();
			}
		}

		private void ResetFocus() {
			RootContext.Focus();
		}
	}
}
