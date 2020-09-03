﻿using GKitForWPF.Graphics;
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
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_UiItemSelector : UserControl, IValueEditor {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static UiFile UiFile => EditingTaleData.UiFile;

		public static readonly DependencyProperty SelectedUiGuidProperty = DependencyProperty.RegisterAttached(nameof(SelectedUiGuid), typeof(string), typeof(ValueEditor_UiItemSelector), new PropertyMetadata(null));

		private const string UnselectedText = "(None)";
		private readonly Dictionary<string, ComboBoxItem> Guid_To_ItemDict;

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return SelectedUiGuid;
			}
			set {
				SelectedUiGuid = (string)value;
			}
		}
		public string SelectedUiGuid {
			get {
				return (string)GetValue(SelectedUiGuidProperty);
			}
			set {
				SetValue(SelectedUiGuidProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		private bool ignoreSelectionChanged;

		public ValueEditor_UiItemSelector() {
			InitializeComponent();

			// Set ItemSource
			Guid_To_ItemDict = new Dictionary<string, ComboBoxItem>();
			IEnumerable<ComboBoxItem> items = UiFile.UiItemList.Select(
				(UiItemBase itemSrc) => {
					ComboBoxItem item = new ComboBoxItem() {
						Content = itemSrc.name,
						Tag = itemSrc.guid
					};
					Guid_To_ItemDict.Add(itemSrc.guid, item);
					return item;
				}
			);

			UiItemComboBox.ItemsSource = new ComboBoxItem[] { new ComboBoxItem() { Content = UnselectedText } }
				.Concat(items).ToArray();

			// Register events
			UiItemComboBox.SelectionChanged += UiComboBox_SelectionChanged;
			EditableValueChanged += OnEditableValueChanged;
		}

		private void UiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (ignoreSelectionChanged)
				return;

			ComboBoxItem selectedItem = UiItemComboBox.SelectedValue as ComboBoxItem;
			if (selectedItem == null ||selectedItem.Tag == null) {
				SelectedUiGuid = null;
				return;
			}

			SelectedUiGuid = selectedItem.Tag as string;
		}

		private void OnEditableValueChanged(object value) {
			ignoreSelectionChanged = true;
			UpdateUI();
			ignoreSelectionChanged = false;
		}

		private void UpdateUI() {
			ComboBoxItem selectedItem = null;
			if(!string.IsNullOrEmpty(SelectedUiGuid)) {
				Guid_To_ItemDict.TryGetValue(SelectedUiGuid, out selectedItem);
			}
			UiItemComboBox.SelectedValue = selectedItem;
		}
	}
}