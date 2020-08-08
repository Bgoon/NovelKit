using GKitForWPF.Graphics;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaleKitEditor.UI.ValueEditors {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_EnumComboBox : UserControl, IValueEditorElement {
		public event EditableValueChangedDelegate EditableValueChanged;

		private Type enumType;
		public object EditableValue {
			get {
				return ValueComboBox.SelectedItem;
			} set {
				ValueComboBox.SelectedItem = value;
			}
		}

		public ValueEditor_EnumComboBox(Type enumType) {
			this.enumType = enumType;

			InitializeComponent();
			ValueComboBox.ItemsSource = Enum.GetValues(enumType);
			ValueComboBox.SelectionChanged += ValueComboBox_SelectionChanged;
		}

		private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			EditableValueChanged?.Invoke(EditableValue);
		}
	}
}
