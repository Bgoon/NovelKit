using GKit;
using GKit.WPF;
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
using TaleKitEditor.Resources.Shader;
using TaleKitEditor.Resources.VectorImages;

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// ColorSelectDialog.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ColorSelectDialog : Window {
		private Shader_ColorEditor_SV colorEditor_SV_Shader;
		private HSV selectedHsv;
		
		private bool isClosing;



		public ColorSelectDialog() {
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			ColorBox_SV.Effect = colorEditor_SV_Shader = new Shader_ColorEditor_SV();
			ColorBox_H.Effect = new Shader_ColorEditor_H();
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
			//if (!isClosing) {
			//	Close();
			//}
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			isClosing = true;
		}

		private void UpdateUI() {
			float hIndicatorOffset = (float)HueIndicator.Height * -0.5f;
			colorEditor_SV_Shader.Hue = selectedHsv.hue;
			HueIndicator.Margin = new Thickness(0f, selectedHsv.hue * ColorBox_H.ActualHeight + hIndicatorOffset, 0f, 0f);

			float svIndicatorOffset = (float)SvIndicator.Width * -0.5f;
			SvIndicator.Margin = new Thickness(selectedHsv.saturation * ColorBox_SV.ActualWidth + svIndicatorOffset, selectedHsv.value * ColorBox_SV.ActualHeight + svIndicatorOffset, 0f, 0f);
		}

		private void HueEditor_MouseDown(object sender, MouseButtonEventArgs e) {

		}
		private void HueEditor_MouseMove(object sender, MouseEventArgs e) {
			FrameworkElement colorBox = (FrameworkElement)sender;

			float hueValue = Mathf.Clamp01((float)(e.GetPosition(colorBox).Y / colorBox.ActualHeight));
			selectedHsv.hue = hueValue;

			UpdateUI();
		}
		private void HueEditor_MouseUp(object sender, MouseButtonEventArgs e) {

		}

		private void SvEditor_MouseDown(object sender, MouseButtonEventArgs e) {

		}
		private void SvEditor_MouseMove(object sender, MouseEventArgs e) {
			FrameworkElement colorBox = (FrameworkElement)sender;
			Point cursorPos = e.GetPosition(colorBox);

			selectedHsv.saturation = Mathf.Clamp01((float)(cursorPos.X / colorBox.ActualWidth));
			selectedHsv.value = Mathf.Clamp01((float)(cursorPos.Y / colorBox.ActualHeight));

			UpdateUI();
		}
		private void SvEditor_MouseUp(object sender, MouseButtonEventArgs e) {

		}

		
	}
}
