using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TaleKit.Datas.ModelEditor;
using TaleKitEditor.UI.Utility;

namespace TaleKitEditor.UI.ModelEditor {
	/// <summary>
	/// CheckBoxValueEditor.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ValueEditor_TextBlockViewer : UserControl, IValueEditor {
		public event EditableValueChangedDelegate EditableValueChanged;

		public object EditableValue {
			get => ValueTextBlock.Text;
			set {
				string text = value.ToString();

				if (text == null) {
					text = string.Empty;
				}

				ValueTextBlock.Text = text;
				//EditableValueChanged?.Invoke(text);
			}
		}

		public ValueEditor_TextBlockViewer() {
			InitializeComponent();
		}
	}
}
