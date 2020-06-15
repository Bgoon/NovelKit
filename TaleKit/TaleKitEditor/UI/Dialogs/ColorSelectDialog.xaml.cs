using GKitForWPF;
using GKitForWPF.Graphics;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TaleKitEditor.Resources.Shader;
using TaleKitEditor.Resources.VectorImages;
using TaleKitEditor.UI.Utility;
using TaleKitEditor.Utility;
using UColor = UnityEngine.Color;

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// ColorSelectDialog.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ColorSelectDialog : Window {
		private Shader_ColorEditor_SV colorEditor_SV_Shader;

		[FindByTag] private ValueEditors.ValueEditorElement_NumberBox EditText_H;
		[FindByTag] private ValueEditors.ValueEditorElement_NumberBox EditText_S;
		[FindByTag] private ValueEditors.ValueEditorElement_NumberBox EditText_V;

		[FindByTag] private ValueEditors.ValueEditorElement_NumberBox EditText_R;
		[FindByTag] private ValueEditors.ValueEditorElement_NumberBox EditText_G;
		[FindByTag] private ValueEditors.ValueEditorElement_NumberBox EditText_B;

		[FindByTag] private ValueEditors.ValueEditorElement_TextBox EditText_Hex;

		private HSV selectedHsv;
		private UColor oldColor;

		private bool hueDragging;
		private bool svDragging;

		public event Action<UColor> ValueChanged;

		private Vector2 windowTalePosition;

		private bool isClosing;
		private bool onEditTextEvent;


		public ColorSelectDialog() {
			InitializeComponent();
			this.FindControlsByTag();
		}
		public ColorSelectDialog(Vector2 windowTalePosition, UColor currentColor) : this() {
			Color currentWColor = currentColor.ToColor();

			this.windowTalePosition = windowTalePosition;
			this.oldColor = currentColor;
			this.selectedHsv = currentWColor.ToHSV();

			ColorBox_SV.Effect = colorEditor_SV_Shader = new Shader_ColorEditor_SV();
			ColorBox_Hue.Effect = new Shader_ColorEditor_H();

			CurrentColorIndicator.Fill = currentWColor.ToBrush();

			Opacity = 0d;
		}
		private void RegisterEvents() {
			EditText_H.EditableValueChanged += UpdateColorByHSV;
			EditText_S.EditableValueChanged += UpdateColorByHSV;
			EditText_V.EditableValueChanged += UpdateColorByHSV;
			EditText_R.EditableValueChanged += UpdateColorByRGB;
			EditText_G.EditableValueChanged += UpdateColorByRGB;
			EditText_B.EditableValueChanged += UpdateColorByRGB;
			EditText_Hex.EditableValueChanged += UpdateColorByHex;
		}

		private void Window_ContentRendered(object sender, EventArgs e) {
			RegisterEvents();
			OnValueChanged_SelectedHsv();

			Vector2 windowPos = windowTalePosition;
			windowPos += -(Vector2)TailShape.TranslatePoint(new Point((float)TailShape.ActualWidth, (float)-TailShape.ActualHeight * 0.5f), this);

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
			if(e.Key == Key.Return) {
				ResetFocus();
			}
		}

		private void HueEditor_MouseDown(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				hueDragging = true;
				Mouse.Capture((IInputElement)sender);
			}
		}
		private void HueEditor_MouseMove(object sender, MouseEventArgs e) {
			if (!hueDragging)
				return;

			FrameworkElement colorBox = (FrameworkElement)sender;

			float hueValue = Mathf.Clamp01((float)(e.GetPosition(colorBox).Y / colorBox.ActualHeight));
			selectedHsv.hue = hueValue * 360f;

			OnValueChanged_SelectedHsv();
		}
		private void HueEditor_MouseUp(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				hueDragging = false;
				Mouse.Capture(null);
			}	
		}

		private void SvEditor_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Left) {
				svDragging = true;
				Mouse.Capture((IInputElement)sender);
			}
		}
		private void SvEditor_MouseMove(object sender, MouseEventArgs e) {
			if (!svDragging)
				return;

			FrameworkElement colorBox = (FrameworkElement)sender;
			Point cursorPos = e.GetPosition(colorBox);

			selectedHsv.saturation = Mathf.Clamp01((float)(cursorPos.X / colorBox.ActualWidth));
			selectedHsv.value = 1f - Mathf.Clamp01((float)(cursorPos.Y / colorBox.ActualHeight));

			OnValueChanged_SelectedHsv();
		}
		private void SvEditor_MouseUp(object sender, MouseButtonEventArgs e) {
			if(e.ChangedButton == MouseButton.Left) {
				svDragging = false;
				Mouse.Capture(null);
			}
		}

		private void OnValueChanged_SelectedHsv() {
			UpdateTextBoxes();

			//Update indicators
			float hIndicatorOffset = (float)HueIndicator.Height * -0.5f;
			colorEditor_SV_Shader.Hue = selectedHsv.hue;
			HueIndicator.Margin = new Thickness(0f, selectedHsv.hue / 360f * ColorBox_Hue.ActualHeight + hIndicatorOffset, 0f, 0f);

			float svIndicatorOffset = (float)SvIndicator.Width * -0.5f;
			SvIndicator.Margin = new Thickness(selectedHsv.saturation * ColorBox_SV.ActualWidth + svIndicatorOffset, (1f - selectedHsv.value) * ColorBox_SV.ActualHeight + svIndicatorOffset, 0f, 0f);

			NewColorIndicator.Fill = selectedHsv.ToColor().ToBrush();

			ValueChanged?.Invoke(selectedHsv.ToColor().ToUColor());
		}

		private void UpdateTextBoxes() {
			onEditTextEvent = true;

			EditText_H.EditableValue = selectedHsv.hue;
			EditText_S.EditableValue = selectedHsv.saturation;
			EditText_V.EditableValue = selectedHsv.value;

			Color color = selectedHsv.ToColor();
			EditText_R.EditableValue = (int)color.R;
			EditText_G.EditableValue = (int)color.G;
			EditText_B.EditableValue = (int)color.B;

			EditText_Hex.EditableValue = color.ToHex();

			onEditTextEvent = false;
		}

		private void UpdateColorByHSV(object value) {
			if (onEditTextEvent)
				return;

			try {
				selectedHsv = new HSV(EditText_H.Value, EditText_S.Value, EditText_V.Value);
			} catch {

			}
			OnValueChanged_SelectedHsv();
		}
		private void UpdateColorByRGB(object value) {
			if (onEditTextEvent)
				return;

			try {
				Color color = Color.FromArgb(255, 
					(byte)Mathf.Clamp(EditText_R.IntValue, 0, 255), 
					(byte)Mathf.Clamp(EditText_G.IntValue, 0, 255), 
					(byte)Mathf.Clamp(EditText_B.IntValue, 0, 255));
				selectedHsv = color.ToHSV();
			} catch {

			}
			OnValueChanged_SelectedHsv();
		}
		private void UpdateColorByHex(object value) {
			if (onEditTextEvent)
				return;

			string hex = (string)EditText_Hex.EditableValue;
			try {
				Color color = hex.ToColor();
				selectedHsv = color.ToHSV();
			} catch {

			}
			OnValueChanged_SelectedHsv();
		}

		private void ResetFocus() {
			RootContext.Focus();
		}
	}
}
