using GKitForWPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// TaleDialog.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TaleDialog : Window {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;

		private Vector2 talePosition;

		private bool isClosing;

		// [ Static function ]
		public static TaleDialog Show(FrameworkElement content) {
			return Show(content, MouseInput.GetPositionFromWindow(MainWindow) / DPIUtility.GetDPIScale(MainWindow));
		}
		public static TaleDialog Show(FrameworkElement content, Vector2 talePosition) {
			TaleDialog dialog = new TaleDialog(talePosition);
			dialog.ContentContext.Children.Add(content);
			dialog.UpdateWindowPos();

			dialog.Show();

			return dialog;
		}

		// [ Constructor ]
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
			UpdateWindowPos();

			Opacity = 1d;

			this.PlayEaseInAnim(OpacityProperty, 0d, 1d, 0.2f);
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

		public void UpdateWindowPos() {
			PresentationSource source = PresentationSource.FromVisual(MainWindow);
			Vector2 dpiScale = new Vector2(1f, 1f);
			if(source != null) {
				dpiScale.x = (float)source.CompositionTarget.TransformToDevice.M11;
				dpiScale.y = (float)source.CompositionTarget.TransformToDevice.M22;
			}
			
			Vector2 mainWindowPos = new Vector2((float)MainWindow.Left, (float)(MainWindow.Top + SystemParameters.CaptionHeight * dpiScale.y));
			Vector2 windowPos = mainWindowPos + talePosition;// / dpiScale;
			windowPos -= (Vector2)TailShape.TranslatePoint(new System.Windows.Point((float)TailShape.ActualWidth, (float)TailShape.ActualHeight * 0.5f), this);

			Left = windowPos.x;
			Top = windowPos.y;
		}

		private void ResetFocus() {
			RootContext.Focus();
		}
	}
}
