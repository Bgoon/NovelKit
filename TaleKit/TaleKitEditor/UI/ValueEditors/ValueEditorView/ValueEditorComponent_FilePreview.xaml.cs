using GKitForWPF;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace TaleKitEditor.UI.ValueEditors {
	public partial class ValueEditorComponent_FilePreview : UserControl, IValueEditorComponent {
		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(nameof(Source), typeof(BitmapImage), typeof(ValueEditorComponent_FilePreview), new PropertyMetadata(null));

		public BitmapImage Source {
			get {
				return (BitmapImage)GetValue(SourceProperty);
			}
			set {
				SetValue(SourceProperty, value);
			}
		}

		[Obsolete]
		internal ValueEditorComponent_FilePreview() {
			InitializeComponent();
		}
		public ValueEditorComponent_FilePreview(string fileUri) {
			InitializeComponent();

			HideAll();
			try {
				string fileExt = Path.GetExtension(fileUri).ToLower();
				switch (fileExt) {
					case ".bmp":
					case ".jpg":
					case ".jpeg":
					case ".gif":
					case ".tiff":
					case ".png":
						ShowImagePreview(fileUri);
						break;
					default:
						ShowTextPreview(fileUri);
						break;
				}
			} catch (Exception ex) {
			}
		}

		private void ShowImagePreview(string fileUri) {
			Source = new BitmapImage(new Uri(fileUri));

			PreviewImageContext.Visibility = Visibility.Visible;
		}
		private void ShowTextPreview(string fileUri) {
			FileInfo fileInfo = new FileInfo(fileUri);

			if (fileInfo.Length > 1024 * 100)
				return;

			string text = File.ReadAllText(fileUri, Encoding.UTF8);
			PreviewTextBox.Text = text;

			PreviewTextBox.Visibility = Visibility.Visible;
		}

		private void HideAll() {
			PreviewImageContext.Visibility = Visibility.Collapsed;
			PreviewTextBox.Visibility = Visibility.Collapsed;
		}
	}
}
