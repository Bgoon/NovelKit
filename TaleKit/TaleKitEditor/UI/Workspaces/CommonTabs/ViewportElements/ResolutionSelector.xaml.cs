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

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements {
	public delegate void ResolutionChangedDelegate(int width, int height);
	public delegate void ZoomChangedDelegate(double zoom);
	public partial class ResolutionSelector : UserControl {
		public event ResolutionChangedDelegate ResolutionChanged;
		public event ZoomChangedDelegate ZoomChanged;

		public ResolutionSelector() {
			InitializeComponent();

			// Init members
			WidthEditText.Text = "1920";
			HeightEditText.Text = "1080";
			ZoomNumberEditor.MinValue = 0;
			ZoomNumberEditor.MaxValue = 100;
			ZoomNumberEditor.Value = 50;

			// Register events
			WidthEditText.TextEdited += OnResolutionEditTextEdited;
			HeightEditText.TextEdited += OnResolutionEditTextEdited;
			ZoomNumberEditor.ValueChanged += RaiseZoomChanged;
		}


		// [ Event ]
		public void RaiseZoomChanged() {
			ZoomChanged?.Invoke(ZoomNumberEditor.Value * 0.01f);
		}
		public void RaiseResolutionChanged() {
			int width, height;

			if (!int.TryParse(WidthEditText.Text, out width) || !int.TryParse(HeightEditText.Text, out height))
				return;

			ResolutionChanged?.Invoke(width, height);
		}

		private void OnResolutionEditTextEdited(string oldText, string newText, ref bool cancelEdit) {
			int value;
			if(!int.TryParse(newText, out value)) {
				cancelEdit = true;
				return;
			}

			RaiseResolutionChanged();
		}

	}
}
