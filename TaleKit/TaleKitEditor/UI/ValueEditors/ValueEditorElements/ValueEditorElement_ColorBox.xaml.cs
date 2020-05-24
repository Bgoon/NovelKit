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
using GKit;
using GKit.WPF;
using TaleKitEditor.UI.Dialogs;
using TaleKitEditor.Utility;
using UColor = UnityEngine.Color;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// ValueEditorElement_Color.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditorElement_ColorBox : UserControl, IValueEditorElement {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(nameof(Value), typeof(UColor), typeof(ValueEditorElement_ColorBox), new PropertyMetadata(UColor.black));

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

		public ValueEditorElement_ColorBox() {
			InitializeComponent();
			Init();
			RegisterEvents();
		}
		private void Init() {
			UpdateUI();
		}
		private void RegisterEvents() {
			ButtonContext.RegisterButtonReaction();
			ButtonContext.RegisterOnClick(ButtonContext_OnClick);
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
			Vector2 windowTailPos = this.GetAbsolutePosition(new Vector2(10f, (float)ActualHeight * 0.5f));

			ColorSelectDialog dialog = new ColorSelectDialog(windowTailPos, Value);
			dialog.Show();

			dialog.ValueChanged += Dialog_ValueChanged;
		}
		private void Dialog_ValueChanged(UColor value) {
			Value = value;
		}

		private void UpdateUI() {
			ColorIndicator.Fill = Value.ToColor().ToBrush();
		}
	}
}
