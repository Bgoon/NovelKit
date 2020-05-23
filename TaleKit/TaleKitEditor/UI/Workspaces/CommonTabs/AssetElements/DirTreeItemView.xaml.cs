using GKit;
using GKit.WPF;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.CommonTabs;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.AssetElements {
	/// <summary>
	/// DirTreeItemView.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DirTreeItemView : UserControl, IDisposable {
		private static Root Root => Root.Instance;
		private static MainWindow MainWindow => Root.MainWindow;
		private static AssetTab AssetTab => MainWindow.AssetTab;

		public static readonly DependencyProperty DirNameProperty = DependencyProperty.RegisterAttached(nameof(DirName), typeof(string), typeof(DirTreeItemView), new PropertyMetadata("DirName"));
		public static readonly DependencyProperty TreeOpenArrowAngleProperty = DependencyProperty.RegisterAttached(nameof(TreeOpenArrowAngle), typeof(double), typeof(DirTreeItemView), new PropertyMetadata(0d));
		
		private static SolidColorBrush SelectedBrush = "535E69".ToBrush();


		public DirTreeItemView ParentView {
			get; private set;
		}
		public string DirFullName {
			get; private set;
		}
		public string DirName {
			get {
				return (string)GetValue(DirNameProperty);
			}
			set {
				SetValue(DirNameProperty, value);
			}
		}
		public bool IsTreeOpen {
			get; private set;
		}
		private bool isChildLoaded;

		public double TreeOpenArrowAngle {
			get {
				return (double)GetValue(TreeOpenArrowAngleProperty);
			}
			set {
				SetValue(TreeOpenArrowAngleProperty, value);
			}
		}

		private FileSystemWatcher watcher;

		[Obsolete]
		internal DirTreeItemView() {
			//For designer
			InitializeComponent();
		}
		public DirTreeItemView(string dirFullName) {
			InitializeComponent();
			RegisterEvents();
			this.DirFullName = dirFullName;
			this.DirName = Path.GetFileName(dirFullName);

			SetTreeOpen(false);
		}
		private void RegisterEvents() {
			PathContext.RegisterButtonReaction(0.05f);
			PathContext.RegisterOnClick(OnClick);
			TreeRightArrow.RegisterButtonReaction(0.1f);
			TreeRightArrow.RegisterOnClick(TreeRightArrow_MouseDown);
		}
		public void Dispose() {
			if(isChildLoaded) {
				if(watcher != null) {
					watcher.Dispose();
					watcher = null;
				}
			}
		}

		private void TreeRightArrow_MouseDown() {
			SetTreeOpen(!IsTreeOpen);
		}
		private void OnClick() {
			AssetTab.ExploreDir(DirFullName);
		}

		public void SetTreeOpen(bool isTreeOpen) {
			this.IsTreeOpen = isTreeOpen;

			//TreeArrow anim
			DoubleAnimation anim = new DoubleAnimation() {
				To = isTreeOpen ? 90d : 0d,
				EasingFunction = new PowerEase() {
					EasingMode = EasingMode.EaseOut,
				},
				Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
			};
			BeginAnimation(TreeOpenArrowAngleProperty, anim);

			//Show/Hide childs
			ChildItemContext.Visibility = isTreeOpen ? Visibility.Visible : Visibility.Collapsed;

			if(isTreeOpen && !isChildLoaded) {
				UpdateTree();
			}
		}

		private void UpdateTree() {
			if(!isChildLoaded) {
				isChildLoaded = true;

				RegisterWatcher();
			}

			string[] subDirs = Directory.GetDirectories(DirFullName).Where((string subDir) => {
				return !AssetTab.ExcludeDirNames.Contains(subDir);
			}).ToArray();

			ChildItemContext.Children.Clear();
			foreach (string subDir in subDirs) {
				DirTreeItemView itemView = new DirTreeItemView(subDir);

				ChildItemContext.Children.Add(itemView);
			}
		}
		private void RegisterWatcher() {

		}

	}
}
