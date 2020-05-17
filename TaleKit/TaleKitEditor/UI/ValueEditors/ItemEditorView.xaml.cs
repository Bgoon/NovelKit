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
using TaleKit.Datas.Editor;

namespace TaleKitEditor.UI.ValueEditors {
	[ContentProperty(nameof(Children))]
	public partial class ItemEditorView : UserControl {
		public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(nameof(Children), typeof(UIElementCollection), typeof(ItemEditorView), new PropertyMetadata());
		public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.RegisterAttached(nameof(HeaderText), typeof(string), typeof(ItemEditorView), new PropertyMetadata("Header"));


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

		public ItemEditorView() {
			InitializeComponent();
			Children = ValueEditorViewContext.Children;
		}
		public ItemEditorView(IEditableModel model) : this() {
			ValueEditorUtility.CreateValueEditorViews(model, ValueEditorViewContext);
		}
	}
}
