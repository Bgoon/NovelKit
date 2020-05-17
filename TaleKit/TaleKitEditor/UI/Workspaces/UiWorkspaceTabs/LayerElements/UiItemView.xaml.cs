using GKit.WPF.UI.Controls;
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
using TaleKit.Datas.UI;

namespace TaleKitEditor.UI.Workspaces.UiWorkspaceTabs {
	/// <summary>
	/// UiItem.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class UiItemView : UserControl, ITreeFolder {
		public readonly UiItem Data;

		//ITreeFolder interface
		public string DisplayName => NameEditText.Text;
		public ITreeFolder ParentItem {
			get; set;
		}
		public FrameworkElement ItemContext => ItemPanel;
		public UIElementCollection ChildItemCollection => ChildStackPanel.Children;


		public UiItemView() {
			InitializeComponent();
		}
		public UiItemView(UiItem data) : this() {
			this.Data = data;
		}

		public void SetRootItem() {
			ItemPanel.Visibility = Visibility.Collapsed;
		}
		public void SetDisplayName(string name) {
			NameEditText.Text = name;
		}
		public void SetDisplaySelected(bool isSelected) {
			ItemPanel.Background = (Brush)Application.Current.Resources[isSelected ? "ItemBackground_Selected" : "ItemBackground"];
		}
	}
}
