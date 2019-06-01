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
	/// FileManager.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class FileManagerBar : UserControl {
		public event Action OnClick_CreateFileButton;
		public event Action OnClick_OpenFileButton;
		public event Action OnClick_SaveFileButton;

		public FileManagerBar() {
			InitializeComponent();

			if(this.IsDesignMode())
				return;

			RegisterEvents();
		}
		private void RegisterEvents() {
			Grid[] buttons = new Grid[] {
				CreateFileButton,
				OpenFileButton,
				SaveFileButton,
			};

			for(int i=0; i<buttons.Length; ++i) {
				buttons[i].SetBtnColor();
			}

			CreateFileButton.SetOnClick(()=> { OnClick_CreateFileButton(); });
			OpenFileButton.SetOnClick(()=> { OnClick_OpenFileButton(); });
			SaveFileButton.SetOnClick(()=> { OnClick_SaveFileButton(); });
		}

	}
}
