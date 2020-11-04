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
using TaleKit.Datas.Motion;
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_EasingSelector : UserControl, IValueEditor {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static MotionFile MotionFile => EditingTaleData.MotionFile;

		public static readonly DependencyProperty SelectedEasingKeyProperty = DependencyProperty.RegisterAttached(nameof(SelectedEasingKey), typeof(string), typeof(ValueEditor_EasingSelector), new PropertyMetadata(null));

		private const string UnselectedText = "(None)";

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return SelectedEasingKey;
			}
			set {
				SelectedEasingKey = (string)value;
			}
		}
		public string SelectedEasingKey {
			get {
				return (string)GetValue(SelectedEasingKeyProperty);
			}
			set {
				SetValue(SelectedEasingKeyProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		private bool ignoreSelectionChanged;

		public ValueEditor_EasingSelector() {
			InitializeComponent();

			// Set ItemSource
			List<string> itemList = new List<string>();
			foreach(var motionPair in MotionFile.motionData.itemDict) {
				if(motionPair.Value.Type == PenMotion.Datas.Items.MotionItemType.Motion) {
					itemList.Add(motionPair.Key);
				}
			}
			itemList.Sort();
			itemList.Insert(0, UnselectedText);

			EasingComboBox.ItemsSource = itemList;

			// Register events
			EasingComboBox.SelectionChanged += AssetComboBox_SelectionChanged;
			EditableValueChanged += OnEditableValueChanged;
		}

		private void AssetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (ignoreSelectionChanged)
				return;

			string selectedText = EasingComboBox.SelectedValue as string;
			if (selectedText == UnselectedText) {
				SelectedEasingKey = null;
				return;
			}

			SelectedEasingKey = selectedText;
		}

		private void OnEditableValueChanged(object value) {
			ignoreSelectionChanged = true;
			UpdateUI();
			ignoreSelectionChanged = false;
		}

		private void UpdateUI() {
			EasingComboBox.SelectedValue = SelectedEasingKey;
		}
	}
}
