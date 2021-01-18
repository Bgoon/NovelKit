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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.Utility;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using UColor = UnityEngine.Color;
using UGRect = GKitForUnity.GRect;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements.UIContents {

	public partial class UIContent_ScriptText : UserControl, IUIContent {
		public delegate void SetCharAttribute(TextBlock textBlock);	

		public UIRenderer OwnerRenderer {
			get; private set;
		}

		// Text data
		public string Text => (OwnerRenderer.Data as UIItem_ScriptText).text;

		// Motion data
		public bool IsMotionComplete => string.IsNullOrEmpty(Text) || motionElapsedSec >= totalMotionSec;
		public float charMotionSec = 0.5f; // 글자 하나의 모션 시간
		public float charMotionDelaySec = 0.04f; // 앞 글자와의 모션 시작 딜레이
		public float totalMotionSec;

		public float motionStartAlpha;
		public Vector2 motionStartOffset;

		private float motionElapsedSec;



		// Constructor
		[Obsolete]
		public UIContent_ScriptText() {
			InitializeComponent();
		}
		public UIContent_ScriptText(UIRenderer ownerRenderer) {
			InitializeComponent();

			this.OwnerRenderer = ownerRenderer;
		}

		// Render
		public void Render(UIItemBase data) {
			UIItem_ScriptText scriptData = data as UIItem_ScriptText;

			OwnerRenderer.SetProperty(scriptData, nameof(scriptData.text), (object value) => {
				CreateChars((string)value, scriptData);
			});

			OwnerRenderer.SetProperty(scriptData, nameof(scriptData.fontFamily), (object value) => {
				SetTextAttribute((x) => {
					string fontFamilyName = (string)value;
					FontFamily fontFamily;
					if (!string.IsNullOrEmpty(fontFamilyName)) {
						fontFamily = new FontFamily(fontFamilyName);
					} else {
						fontFamily = new FontFamily();
					}
					x.FontFamily = fontFamily;
				});
			});

			OwnerRenderer.SetProperty(scriptData, nameof(scriptData.fontSize), (object value) => {
				SetTextAttribute(x => x.FontSize = Mathf.Max(1, (int)value)); 
			});

			OwnerRenderer.SetProperty(scriptData, nameof(scriptData.fontColor), (object value) => {
				SetTextAttribute(x => x.Foreground = ((UColor)value).ToColor().ToBrush()); 
			});

			OwnerRenderer.SetProperty(scriptData, nameof(scriptData.textAnchor), (object value) => {
				TextAnchor textAnchor = (TextAnchor)value;
				switch (textAnchor) {
					case TextAnchor.UpperLeft:
					case TextAnchor.MiddleLeft:
					case TextAnchor.LowerLeft:
						CharWrapPanel.HorizontalContentAlignment = HorizontalAlignment.Left;
						break;
					case TextAnchor.UpperCenter:
					case TextAnchor.MiddleCenter:
					case TextAnchor.LowerCenter:
						CharWrapPanel.HorizontalContentAlignment = HorizontalAlignment.Center;
						break;
					case TextAnchor.UpperRight:
					case TextAnchor.MiddleRight:
					case TextAnchor.LowerRight:
						CharWrapPanel.HorizontalContentAlignment = HorizontalAlignment.Right;
						break;
				}

				switch (textAnchor) {
					case TextAnchor.UpperLeft:
					case TextAnchor.UpperCenter:
					case TextAnchor.UpperRight:
						CharWrapPanel.VerticalAlignment = VerticalAlignment.Top;
						break;
					case TextAnchor.MiddleLeft:
					case TextAnchor.MiddleCenter:
					case TextAnchor.MiddleRight:
						CharWrapPanel.VerticalAlignment = VerticalAlignment.Center;
						break;
					case TextAnchor.LowerLeft:
					case TextAnchor.LowerCenter:
					case TextAnchor.LowerRight:
						CharWrapPanel.VerticalAlignment = VerticalAlignment.Bottom;
						break;
				}
			});
		}

		// Motion
		public void ResetMotion() {
			UpdateTotalMotionSec();
			motionElapsedSec = 0f;

			UpdateMotion(0f, true);
		}
		public void SkipMotion() {
			motionElapsedSec = totalMotionSec;

			UpdateMotion(0f, true);
		}
		public void UpdateMotion(float deltaTime, bool force = false) {
			if (IsMotionComplete && !force)
				return;

			motionElapsedSec += deltaTime;

			for (int i = 0; i < CharWrapPanel.Children.Count; ++i) {
				TextBlock character = CharWrapPanel.Children[i] as TextBlock;

				float indexTime = (motionElapsedSec - i * charMotionDelaySec) / charMotionSec;
				float motionTime = Mathf.Pow(Mathf.Clamp01(indexTime), 0.5f);
				float motionTimeInvert = 1f - motionTime;

				Vector2 offset = motionStartOffset * motionTimeInvert;
				character.RenderTransform = new TranslateTransform(offset.x, offset.y);
				character.Opacity = motionStartAlpha + (1f - motionStartAlpha) * motionTime;
			}
		}

		private void UpdateTotalMotionSec() {
			if(string.IsNullOrEmpty(Text)) {
				totalMotionSec = 0f;
				return;
			}

			totalMotionSec = Text.Length * charMotionDelaySec + charMotionSec;
		}

		// Manage Char
		private void ClearChars() {
			CharWrapPanel.Children.Clear();
		}
		private void CreateChars(string text, UIItem_ScriptText scriptData) {
			ClearChars();

			if (string.IsNullOrEmpty(text))
				return;
			foreach(char character in text) {
				TextBlock textBlock = new TextBlock();
				textBlock.Text = character.ToString();

				FontFamily fontFamily;
				if (!string.IsNullOrEmpty(scriptData.fontFamily)) {
					fontFamily = new FontFamily(scriptData.fontFamily);
				} else {
					fontFamily = new FontFamily();
				}
				textBlock.FontFamily = fontFamily;
				textBlock.FontSize = Mathf.Max(1, scriptData.fontSize);
				textBlock.Foreground = ((UColor)scriptData.fontColor).ToColor().ToBrush();

				CharWrapPanel.Children.Add(textBlock);
			}

			ResetMotion();
		}

		private void SetTextAttribute(SetCharAttribute setCharAttributeDelegate) {
			foreach(TextBlock character in CharWrapPanel.Children) {
				setCharAttributeDelegate?.Invoke(character);
			}
		}
	}
}
