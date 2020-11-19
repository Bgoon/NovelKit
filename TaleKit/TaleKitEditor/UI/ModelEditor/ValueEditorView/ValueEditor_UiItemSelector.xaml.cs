using GKitForWPF.Graphics;
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
using TaleKit.Datas;
using TaleKit.Datas.Asset;
using TaleKit.Datas.ModelEditor;
using TaleKit.Datas.UI;
using TaleKit.Datas.UI.UIItem;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.ModelEditor {
	public partial class ValueEditor_UIItemSelector : UserControl, IValueEditor {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static UIFile UIFile => EditingTaleData.UIFile;

		public static readonly DependencyProperty SelectedUIGuidProperty = DependencyProperty.RegisterAttached(nameof(SelectedUIGuid), typeof(string), typeof(ValueEditor_UIItemSelector), new PropertyMetadata(null));

		private const string UnselectedText = "(None)";
		private readonly Dictionary<string, ComboBoxItem> Guid_To_ItemDict;

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return SelectedUIGuid;
			}
			set {
				SelectedUIGuid = (string)value;
			}
		}
		public string SelectedUIGuid {
			get {
				return (string)GetValue(SelectedUIGuidProperty);
			}
			set {
				SetValue(SelectedUIGuidProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		private bool ignoreSelectionChanged;

		public ValueEditor_UIItemSelector() {
			InitializeComponent();

			// Set ItemSource
			Guid_To_ItemDict = new Dictionary<string, ComboBoxItem>();
			IEnumerable<ComboBoxItem> items = UIFile.UIItemList.Select(
				(UIItemBase itemSrc) => {
					ComboBoxItem item = new ComboBoxItem() {
						Content = itemSrc.name,
						Tag = itemSrc.guid
					};
					Guid_To_ItemDict.Add(itemSrc.guid, item);
					return item;
				}
			);

			UIItemComboBox.ItemsSource = new ComboBoxItem[] { new ComboBoxItem() { Content = UnselectedText } }
				.Concat(items).ToArray();

			// Register events
			UIItemComboBox.SelectionChanged += UIComboBox_SelectionChanged;
			EditableValueChanged += OnEditableValueChanged;
		}

		private void UIComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (ignoreSelectionChanged)
				return;

			ComboBoxItem selectedItem = UIItemComboBox.SelectedValue as ComboBoxItem;
			if (selectedItem == null ||selectedItem.Tag == null) {
				SelectedUIGuid = null;
				return;
			}

			SelectedUIGuid = selectedItem.Tag as string;
		}

		private void OnEditableValueChanged(object value) {
			ignoreSelectionChanged = true;
			UpdateUI();
			ignoreSelectionChanged = false;
		}

		private void UpdateUI() {
			ComboBoxItem selectedItem = null;
			if(!string.IsNullOrEmpty(SelectedUIGuid)) {
				Guid_To_ItemDict.TryGetValue(SelectedUIGuid, out selectedItem);
			}
			UIItemComboBox.SelectedValue = selectedItem;
		}
	}
}
