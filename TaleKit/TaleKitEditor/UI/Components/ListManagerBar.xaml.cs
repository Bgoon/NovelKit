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
using GKit;

namespace PendulumMotionEditor.Views.Components {
	/// <summary>
	/// ListManagerBar.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ListManagerBar : UserControl {

		public event Action OnClick_CreateItemButton;
		public event Action OnClick_CreateFolderButton;
		public event Action OnClick_CopyButton;
		public event Action OnClick_RemvoeButton;

		public ListManagerBar() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			RegisterEvents();
		}
		private void RegisterEvents() {
			Grid[] buttons = new Grid[] {
				CreateItemButton,
				CreateFolderButton,
				CopyButton,
				RemoveButton,
			};

			for (int i = 0; i < buttons.Length; ++i) {
				Grid button = buttons[i];
				button.SetBtnColor(button.Children[button.Children.Count-1] as Border);
			}

			CreateItemButton.SetOnClick(() => { OnClick_CreateItemButton(); });
			CreateFolderButton.SetOnClick(() => { OnClick_CreateFolderButton(); });
			CopyButton.SetOnClick(() => { OnClick_CopyButton(); });
			RemoveButton.SetOnClick(() => { OnClick_RemvoeButton(); });
		}
	}
}
