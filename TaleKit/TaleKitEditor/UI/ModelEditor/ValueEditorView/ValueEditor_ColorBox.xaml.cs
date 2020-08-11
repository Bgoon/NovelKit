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
using GKitForWPF;
using GKitForWPF.Graphics;
using TaleKitEditor.UI.Dialogs;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.Utility;
using UColor = UnityEngine.Color;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// ValueEditorElement_Color.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_ColorBox : UserControl, IValueEditor {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(UColor), typeof(ValueEditor_ColorBox), new PropertyMetadata(UColor.black));

		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;

		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get {
				return Value;
			}
			set {
				Value = (UColor)value;
			}
		}
		public UColor Value {
			get {
				return (UColor)GetValue(ValueProperty);
			}
			set {
				SetValue(ValueProperty, value);
				EditableValueChanged?.Invoke(value);
			}
		}

		public ValueEditor_ColorBox() {
			InitializeComponent();
			Init();
			RegisterEvents();
		}
		private void Init() {
			UpdateUI();
		}
		private void RegisterEvents() {
			ButtonContext.RegisterButtonReaction();
			ButtonContext.RegisterClickEvent(ButtonContext_OnClick);
			EditableValueChanged += OnEditableValueChanged;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			Init();
			RegisterEvents();
		}
		private void OnEditableValueChanged(object obj) {
			UpdateUI();
		}
		private void ButtonContext_OnClick() {
			Vector2 windowTailPos = (Vector2)this.TranslatePoint(new Point(5f, (float)ActualHeight * 0.5f), MainWindow);

			ColorPickerPanel colorPicker = ColorPickerPanel.ShowDialog(Value, windowTailPos);
			colorPicker.ValueChanged += Dialog_ValueChanged;
		}
		private void Dialog_ValueChanged(UColor value) {
			Value = value;
		}

		private void UpdateUI() {
			ColorIndicator.Fill = Value.ToColor().ToBrush();
		}
	}
}
