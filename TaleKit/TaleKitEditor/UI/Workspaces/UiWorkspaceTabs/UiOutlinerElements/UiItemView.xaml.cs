using GKitForWPF;
using GKitForWPF.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UIItem;

namespace TaleKitEditor.UI.Workspaces.UIWorkspaceTabs {
	public partial class UIItemView : UserControl, ITreeFolder {
		public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.RegisterAttached(nameof(DisplayName), typeof(string), typeof(UIItemView), new PropertyMetadata("Item"));
		public static readonly DependencyProperty ItemTypeNameProperty = DependencyProperty.RegisterAttached(nameof(ItemTypeName), typeof(string), typeof(UIItemView), new PropertyMetadata("ItemType"));

		public readonly UIItemBase Data;

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
		public UIItemView() {
			InitializeComponent();
		}
		public UIItemView(UIItemBase data) : this() {
			this.Data = data;
			data.View = this;

			// Register events
			NameEditText.TextEdited += NameEditText_TextEdited;
			data.ModelUpdated += Data_ModelUpdated;
		}

		// [ Event ]
		private void Data_ModelUpdated(EditableModel updatedModel, FieldInfo fieldInfo, object editorView) {
			if(fieldInfo.Name == nameof(Data.name)) {
				DisplayName = Data.name;
			}
		}
		private void NameEditText_TextEdited(string oldText, string newText, ref bool cancelEdit) {
			if(string.IsNullOrEmpty(newText)) {
				cancelEdit = true;
				return;
			}
				
			Data.name = newText;
			Data.NotifyModelUpdated(Data, Data.GetType().GetField(nameof(Data.name)), null);
		}

		// [ Control ]
		public void SetRootItem() {
			ItemPanel.Visibility = Visibility.Collapsed;
			ChildStackPanel.Margin = new Thickness(0);
		}
		public void SetDisplayName(string name) {
			DisplayName = name;
		}
		public void SetSelected(bool isSelected) {
			ItemPanel.Background = GResourceUtility.GetAppResource<Brush>(isSelected ? "ItemBackground_Selected" : "ItemBackground");
		}
	}
}
