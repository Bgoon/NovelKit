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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaleKit.Datas.ModelEditor;

namespace TaleKitEditor.UI.ModelEditor {
	[ContentProperty(nameof(Children))]
	public partial class ModelEditorView : UserControl {
		public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(nameof(Children), typeof(UIElementCollection), typeof(ModelEditorView), new PropertyMetadata());
		public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.RegisterAttached(nameof(HeaderText), typeof(string), typeof(ModelEditorView), new PropertyMetadata("Header"));


		public UIElementCollection Children {
			get {
				return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
			}
			private set {
				SetValue(ChildrenProperty, value);
			}
		}
		public string HeaderText {
			get {
				return (string)GetValue(HeaderTextProperty);
			}
			set {
				SetValue(HeaderTextProperty, value);
			}
		}

		public ModelEditorView() {
			InitializeComponent();
			Children = ValueEditorViewContext.Children;
		}
		public ModelEditorView(EditableModel model) : this() {
			ModelEditorUtility.CreateModelEditorView(model, ValueEditorViewContext);
		}
	}
}
