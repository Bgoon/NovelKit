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

		public static readonly DependencyProperty SelectedAssetKeyProperty = DependencyProperty.RegisterAttached(nameof(SelectedAssetKey), typeof(string), typeof(ValueEditor_UiItemSelector), new PropertyMetadata(null));

		private const string UnselectedText = "(None)";

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return SelectedAssetKey;
			}
			set {
				SelectedAssetKey = (string)value;
			}
		}
		public string SelectedAssetKey {
			get {
				return (string)GetValue(SelectedAssetKeyProperty);
			}
			set {
				SetValue(SelectedAssetKeyProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		private bool ignoreSelectionChanged;

		public ValueEditor_UiItemSelector() {
			InitializeComponent();
			RegisterEvents();

			UiItemComboBox.ItemsSource =
				new string[] { UnselectedText }.Concat(
					UiFile.UiItemList.Select(x => x.name) // TODO : Unique key를 만들어서 사용하게 하기
				).ToArray();
		}
		private void RegisterEvents() {
			UiItemComboBox.SelectionChanged += AssetComboBox_SelectionChanged;
			EditableValueChanged += OnEditableValueChanged;
		}

		private void AssetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (ignoreSelectionChanged)
				return;

			string selectedText = UiItemComboBox.SelectedValue as string;
			if (selectedText == UnselectedText) {
				SelectedAssetKey = null;
				return;
			}

			SelectedAssetKey = selectedText;
		}

		private void OnEditableValueChanged(object value) {
			ignoreSelectionChanged = true;
			UpdateUI();
			ignoreSelectionChanged = false;
		}

		private void UpdateUI() {
			UiItemComboBox.SelectedValue = SelectedAssetKey;
		}
	}
}
