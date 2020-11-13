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
using UnityEngine;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements {
	public delegate void ResolutionChangedDelegate(int width, int height);
	public delegate void ZoomChangedDelegate(double newZoomScale, double oldZoomScale);

	public partial class ResolutionEditor : UserControl {
		public event ResolutionChangedDelegate ResolutionChanged;
		public event ZoomChangedDelegate ZoomChanged;

		private double prevZoomScale = -1d;

		public ResolutionEditor() {
			InitializeComponent();

			// Init members
			foreach(var numberBox in ResolutionNumberEditor.NumberBoxes) {
				numberBox.MinValue = 1;
				numberBox.MaxValue = 70000;
				numberBox.NumberType = GKitForUnity.NumberType.Int;
			}
			ResolutionNumberEditor.ValueTextBox_X.Value = 1920;
			ResolutionNumberEditor.ValueTextBox_Y.Value= 1080;

			ZoomNumberEditor.MinValue = 1;
			ZoomNumberEditor.MaxValue = 100;
			ZoomNumberEditor.Value = 50;

			// Register events
			ResolutionNumberEditor.EditableValueChanged += ResolutionNumberEditor_EditableValueChanged;
			ZoomNumberEditor.ValueChanged += RaiseZoomChanged;
		}



		// [ Event ]
		public void RaiseZoomChanged() {
			double newZoomScale = ZoomNumberEditor.Value * 0.01f;

			if (prevZoomScale < 0d) {
				prevZoomScale = newZoomScale;
			}

			ZoomChanged?.Invoke(newZoomScale, prevZoomScale);

			prevZoomScale = newZoomScale;
		}

		public void OnResolutionChanged() {
			Vector2 resolution = ResolutionNumberEditor.Value;
			ResolutionChanged?.Invoke((int)resolution.x, (int)resolution.y);
		}
		private void ResolutionNumberEditor_EditableValueChanged(object value) {
			OnResolutionChanged();
		}

	}
}
