using GKitForWPF;
using GKitForWPF.UI.Controls;
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
		public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.RegisterAttached(nameof(DisplayName), typeof(string), typeof(UiItemView), new PropertyMetadata("Item"));
		public static readonly DependencyProperty ItemTypeNameProperty = DependencyProperty.RegisterAttached(nameof(ItemTypeName), typeof(string), typeof(UiItemView), new PropertyMetadata("ItemType"));

		public readonly UiItemBase Data;

		// ITreeFolder interface
		public ITreeFolder ParentItem {
			get; set;
		}
		public FrameworkElement ItemContext => ItemPanel;
		public UIElementCollection ChildItemCollection => ChildStackPanel.Children;

		// Item member
		public string DisplayName {
			get {
				return (string)GetValue(DisplayNameProperty);
			}
			set {
				SetValue(DisplayNameProperty, value);
			}
		}
		public string ItemTypeName {
			get {
				return (string)GetValue(ItemTypeNameProperty);
			}
			set {
				SetValue(ItemTypeNameProperty, value);
			}
		}

		// [ Constructor ]
		public UiItemView() {
			InitializeComponent();
		}
		public UiItemView(UiItemBase data) : this() {
			this.Data = data;
			data.View = this;

			// Register events
			NameEditText.TextEdited += NameEditText_TextEdited;
			data.ModelUpdated += Data_ModelUpdated;
		}


		// [ Event ]
		private void Data_ModelUpdated() {
			DisplayName = Data.name;
		}
		private void NameEditText_TextEdited(string oldText, string newText, ref bool cancelEdit) {
			Data.name = newText;
		}

		// [ Control ]
		public void SetRootItem() {
			ItemPanel.Visibility = Visibility.Collapsed;
		}
		public void SetDisplayName(string name) {
			DisplayName = name;
		}
		public void SetSelected(bool isSelected) {
			ItemPanel.Background = GResourceUtility.GetAppResource<Brush>(isSelected ? "ItemBackground_Selected" : "ItemBackground");
		}
	}
}
