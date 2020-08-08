using GKitForWPF;
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
using TaleKitEditor.Resources;
using UAnchorPreset = GKitForUnity.AnchorPreset;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_AnchorPreset : UserControl, IValueEditorElement {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(UAnchorPreset), typeof(ValueEditor_AnchorPreset), new PropertyMetadata(UAnchorPreset.StretchAll));

		private static SolidColorBrush DeactiveBackBrush = Colors.Transparent.ToBrush();
		private static SolidColorBrush ActiveBackBrush = AppResourceUtility.GetResource(AppResourceName.SelectedBackground) as SolidColorBrush;

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return Value;
			} set {
				Value = (UAnchorPreset)value;
			}
		}
		public UAnchorPreset Value {
			get {
				return (UAnchorPreset)GetValue(ValueProperty);
			} set {
				SetValue(ValueProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		private Button[] anchorButtons;
		private TwoWayDictionary<UAnchorPreset, Button> presetButtonDict;

		public ValueEditor_AnchorPreset() {
			InitializeComponent();
			InitMembers();
			RegisterEvents();

			UpdateUI();
		}
		private void InitMembers() {
			presetButtonDict = new TwoWayDictionary<UAnchorPreset, Button>();
			presetButtonDict.Add(UAnchorPreset.TopLeft, AnchorTopLeftButton);
			presetButtonDict.Add(UAnchorPreset.TopMid, AnchorTopMidButton);
			presetButtonDict.Add(UAnchorPreset.TopRight, AnchorTopRightButton);
			presetButtonDict.Add(UAnchorPreset.MidLeft, AnchorMidLeftButton);
			presetButtonDict.Add(UAnchorPreset.MidMid, AnchorMidMidButton);
			presetButtonDict.Add(UAnchorPreset.MidRight, AnchorMidRightButton);
			presetButtonDict.Add(UAnchorPreset.BotLeft, AnchorBotLeftButton);
			presetButtonDict.Add(UAnchorPreset.BotMid, AnchorBotMidButton);
			presetButtonDict.Add(UAnchorPreset.BotRight, AnchorBotRightButton);

			presetButtonDict.Add(UAnchorPreset.StretchLeft, AnchorStretchLeftButton);
			presetButtonDict.Add(UAnchorPreset.StretchMid, AnchorStretchMidButton);
			presetButtonDict.Add(UAnchorPreset.StretchRight, AnchorStretchRightButton);
			presetButtonDict.Add(UAnchorPreset.TopStretch, AnchorTopStretchButton);
			presetButtonDict.Add(UAnchorPreset.MidStretch, AnchorMidStretchButton);
			presetButtonDict.Add(UAnchorPreset.BotStretch, AnchorBotStretchButton);

			presetButtonDict.Add(UAnchorPreset.StretchAll, AnchorStretchAllButton);

			anchorButtons = new Button[] {
				AnchorTopLeftButton,
				AnchorTopMidButton,
				AnchorTopRightButton,
				AnchorMidLeftButton,
				AnchorMidMidButton,
				AnchorMidRightButton,
				AnchorBotLeftButton,
				AnchorBotMidButton,
				AnchorBotRightButton,

				AnchorStretchLeftButton,
				AnchorStretchMidButton,
				AnchorStretchRightButton,
				AnchorTopStretchButton,
				AnchorMidStretchButton,
				AnchorBotStretchButton,

				AnchorStretchAllButton,
			};
		}
		private void RegisterEvents() {
			EditableValueChanged += ValueEditorElement_AnchorPreset_EditableValueChanged;

			foreach(Button button in anchorButtons) {
				UAnchorPreset preset = presetButtonDict.Reverse[button];
				button.Click += Button_Click;

				void Button_Click(object sender, RoutedEventArgs e) {
					Value = preset;
				}
			}
		}

		private void ValueEditorElement_AnchorPreset_EditableValueChanged(object value) {
			UpdateUI();
		}

		private void UpdateUI() {
			Button activatedButton = presetButtonDict.Forward[Value];

			foreach (Button button in anchorButtons) {
				button.Background = button == activatedButton ? ActiveBackBrush : DeactiveBackBrush;
			}
		}
	}
}
