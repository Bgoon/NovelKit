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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// ColorSelectDialog.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ColorSelectDialog : Window {
		
		private bool isClosing;

		public ColorSelectDialog() {
			InitializeComponent();
		}

		private void Window_ContentRendered(object sender, EventArgs e) {
			//Vector2 windowPos = dstPosition;
			//windowPos += -(Vector2)TailShape.TranslatePoint(new Point((float)TailShape.ActualWidth, (float)-TailShape.ActualHeight * 0.5f), this);

			//Left = windowPos.x;
			//Top = windowPos.y;

			//PlayShowingAnimation();
			//Opacity = 1d;
		}
		private void Window_Deactivated(object sender, EventArgs e) {
			if (!isClosing) {
				Close();
			}
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			isClosing = true;
		}
	}
}
