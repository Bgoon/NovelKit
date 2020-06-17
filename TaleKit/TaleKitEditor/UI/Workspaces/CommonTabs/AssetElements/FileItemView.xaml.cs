using GKitForUnity.IO;
using GKitForWPF;
using GKitForWPF;
using GKitForWPF.Graphics;
using GKitForWPF.UI.Controls;
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
using TaleKitEditor.Resources.VectorImages;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.AssetElements {
	/// <summary>
	/// FileItemView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class FileItemView : UserControl, ISelectable {
		public static readonly DependencyProperty FilenameProperty = DependencyProperty.RegisterAttached(nameof(Filename), typeof(string), typeof(FileItemView), new PropertyMetadata("Filename"));

		private static SolidColorBrush SelectedBrush = "9E6A49".ToBrush();
		private static SolidColorBrush UnselectedBrush = "4F4F4F".ToBrush();

		public string AssetRelPath {
			get; private set;
		}
		public string FullFilename {
			get; private set;
		}
		public string Filename {
			get {
				return (string)GetValue(FilenameProperty);
			}
			set {
				SetValue(FilenameProperty, value);
			}
		}

		public event Action Click;

		[Obsolete]
		internal FileItemView() {
			InitializeComponent();
		}
		public FileItemView(string fullFilename, string assetRelPath, FileIconType fileType) {
			InitializeComponent();

			this.FullFilename = fullFilename;
			this.Filename = Path.GetFileName(fullFilename);
			this.AssetRelPath = assetRelPath;

			UserControl icon = null;
			switch(fileType) {
				case FileIconType.Directory:
					icon = new DirectoryIcon();
					break;
				case FileIconType.File:
					icon = new FileIcon();
					break;
			}
			if(icon != null) {
				IconContext.Children.Add(icon);
			}

			RegisterEvents();
			SetSelected(false);
		}
		private void RegisterEvents() {
			ButtonContext.RegisterButtonReaction(0.05f);
			ButtonContext.RegisterClickEvent(() => {
				Click?.Invoke();
			});
		}

		public void SetSelected(bool isSelected) {
			RootPanel.Background = GResourceUtility.GetAppResource<Brush>(isSelected ? "ItemBackground_Selected" : "ItemBackground");
		}
	}
}
