using GKitForWPF;
using GKitForWPF.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
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
	public partial class PlayStateButton : UserControl {
		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(nameof(IsActive), typeof(bool), typeof(PlayStateButton), new PropertyMetadata(false));
		private static SolidColorBrush BackPanelActiveBackground = "#1A73E0".ToBrush();

		public bool IsActive {
			get {
				return (bool)GetValue(IsActiveProperty);
			}
			set {
				SetValue(IsActiveProperty, value);
				UpdateUI();
			}
		}

		public event Action Click;

		public PlayStateButton() {
			InitializeComponent();

			// Register events
			BackPanel.RegisterButtonReaction();
			BackPanel.RegisterClickEvent(OnBackPanelClick);

			UpdateUI();
		}
		private void OnBackPanelClick() {
			Click?.Invoke();
			IsActive = !IsActive;
		}

		private void UpdateUI() {
			Background = IsActive ? BackPanelActiveBackground : null;
		}

	}
}
