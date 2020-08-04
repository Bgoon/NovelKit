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
using GKitForWPF;

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// MenuItem.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MenuItem : UserControl {
		public static readonly DependencyProperty ItemNameProperty = DependencyProperty.RegisterAttached(nameof(ItemName), typeof(string), typeof(MenuItem), new PropertyMetadata());

		public event Action Click;
		public string ItemName {
			get {
				return (string)GetValue(ItemNameProperty);
			}
			set {
				SetValue(ItemNameProperty, value);
			}
		}

		public MenuItem() {
			InitializeComponent();
		}
		public MenuItem(string itemName, Action action) : this() {
			this.ItemName = itemName;

			if(action != null) {
				Click += action;
			}

			// Register events
			ButtonPanel.RegisterButtonReaction();
			ButtonPanel.RegisterClickEvent(OnClick);
		}

		private void OnClick() {
			Click?.Invoke();
		}
	}
}
