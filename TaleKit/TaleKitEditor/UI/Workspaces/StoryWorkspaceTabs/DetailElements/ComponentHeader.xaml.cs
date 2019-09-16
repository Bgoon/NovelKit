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
using GKit.WPF;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.DetailElements {
	/// <summary>
	/// ComponentHeader.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ComponentHeader : UserControl {
		public ComponentHeader() {
			InitializeComponent();
			Init();
		}
		private void Init() {
			Indicator.DetachParent();
		}
	}
}
