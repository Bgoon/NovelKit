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
using TaleKit.Datas.Story;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_StoryClipSelector : UserControl, IValueEditor {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static AssetManager AssetManager => EditingTaleData.AssetManager;

		public static readonly DependencyProperty SelectedAssetKeyProperty = DependencyProperty.RegisterAttached(nameof(SelectedStoryClip), typeof(string), typeof(ValueEditor_AssetSelector), new PropertyMetadata(null));

		private const string UnselectedText = "(None)";

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return SelectedStoryClip;
			}
			set {
				SelectedStoryClip = (string)value;
			}
		}
		public string SelectedStoryClip {
			get {
				return (string)GetValue(SelectedAssetKeyProperty);
			}
			set {
				SetValue(SelectedAssetKeyProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		private bool ignoreSelectionChanged;

		private Dictionary<string, ComboBoxItem> Guid_To_ItemDict;

		public ValueEditor_StoryClipSelector() {
			InitializeComponent();

			// Set ItemSource
			Guid_To_ItemDict = new Dictionary<string, ComboBoxItem>();
			IEnumerable<ComboBoxItem> items = EditingTaleData.StoryFile.Guid_To_ClipDict.Values.Select(
				(StoryClip clip) => {
					ComboBoxItem item = new ComboBoxItem() {
						Content = clip.name,
						Tag = clip.guid,
					};
					Guid_To_ItemDict.Add(clip.guid, item);
					return item;
				}
			);

			StoryClipComboBox.ItemsSource = new ComboBoxItem[] { new ComboBoxItem() { Content = UnselectedText } }
				.Concat(items).ToArray();

			// Register events
			StoryClipComboBox.SelectionChanged += StoryClipComboBox_SelectionChanged;
			EditableValueChanged += OnEditableValueChanged;
		}

		private void StoryClipComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (ignoreSelectionChanged)
				return;

			ComboBoxItem selectedItem = StoryClipComboBox.SelectedValue as ComboBoxItem;
			if (selectedItem == null || selectedItem.Tag == null) {
				SelectedStoryClip = null;
				return;
			}

			SelectedStoryClip = selectedItem.Tag as string;
		}

		private void OnEditableValueChanged(object value) {
			ignoreSelectionChanged = true;
			UpdateUI();
			ignoreSelectionChanged = false;
		}

		private void UpdateUI() {
			ComboBoxItem selectedItem = null;
			if (!string.IsNullOrEmpty(SelectedStoryClip)) {
				Guid_To_ItemDict.TryGetValue(SelectedStoryClip, out selectedItem);
			}
			StoryClipComboBox.SelectedValue = selectedItem;
		}
	}
}
