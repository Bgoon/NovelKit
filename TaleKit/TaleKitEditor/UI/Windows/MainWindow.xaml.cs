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
using TaleKitEditor.UI.Workspaces;
using GKit;

namespace TaleKitEditor.UI {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window {

		public UIEditor UiEditor;
		public MotionEditor motionEditor;
		public StoryEditor storyEditor;
		private UserControl[] workspaces;

		public MainWindow() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			Init();
			RegisterEvents();
		}
		private void Init() {
			UiEditor = new UIEditor();
			motionEditor = new MotionEditor();
			storyEditor = new StoryEditor();

			workspaces = new UserControl[] {
				UiEditor,
				motionEditor,
				storyEditor,
			};

			ActiveWorkspace(WorkspaceType.UiEditor);
		}
		private void RegisterEvents() {

		}

		public void ActiveWorkspace(WorkspaceType type) {
			for(int i=0; i< workspaces.Length; ++i) {
				workspaces[i].DetachParent();
			}

			UserControl workspace;
			switch(type) {
				default:
				case WorkspaceType.UiEditor:
					workspace = UiEditor;
					break;
				case WorkspaceType.MotionEditor:
					workspace = motionEditor;
					break;
				case WorkspaceType.StoryEditor:
					workspace = storyEditor;
					break;
			}
			workspace.SetParent(WorkspaceContext);
		}
	}
}
