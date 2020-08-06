using GKitForWPF;
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

namespace TaleKitEditor.UI.Dialogs {
	/// <summary>
	/// 선택 가능한 텍스트 메뉴
	/// </summary>
	public partial class MenuPanel : UserControl {

		private MenuItem[] items;

		public event Action ItemClick;

		public static MenuPanel ShowDialog(params MenuItem[] items) {
			MenuPanel panel = new MenuPanel(items);
			TaleDialog dialog = TaleDialog.Show(panel, MouseInput.AbsolutePosition);
			panel.ItemClick += dialog.Close;

			return panel;
		}
		public MenuPanel() {
			InitializeComponent();
		}
		public MenuPanel(params MenuItem[] items) : this() {
			this.items = items;

			foreach(MenuItem item in items) {
				ItemStackPanel.Children.Add(item);

				item.Click += OnItemClick;
			}
		}

		private void OnItemClick() {
			ItemClick?.Invoke();
		}
	}
}
