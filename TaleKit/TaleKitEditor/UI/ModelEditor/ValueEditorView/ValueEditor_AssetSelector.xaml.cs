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
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_AssetSelector : UserControl, IValueEditorElement {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static AssetManager AssetManager => EditingTaleData.AssetManager;

		public static readonly DependencyProperty SelectedAssetKeyProperty = DependencyProperty.RegisterAttached(nameof(SelectedAssetKey), typeof(string), typeof(ValueEditor_AssetSelector), new PropertyMetadata(null));

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

		[Obsolete]
		internal ValueEditor_AssetSelector() {
			InitializeComponent();
		}
		public ValueEditor_AssetSelector(ValueEditor_AssetSelectorAttribute attr) {
			InitializeComponent();
			RegisterEvents();

			AssetComboBox.ItemsSource =
				new string[] { UnselectedText }.Concat(
					AssetManager.GetAssets(attr.assetType, true).Select(x => x.Key)
				).ToArray();
		}
		private void RegisterEvents() {
			AssetComboBox.SelectionChanged += AssetComboBox_SelectionChanged;
			EditableValueChanged += OnEditableValueChanged;
		}

		private void AssetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (ignoreSelectionChanged)
				return;

			string selectedText = AssetComboBox.SelectedValue as string;
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
			AssetComboBox.SelectedValue = SelectedAssetKey;
		}
	}
}
