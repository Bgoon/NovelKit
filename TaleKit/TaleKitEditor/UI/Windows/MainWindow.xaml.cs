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
using GKit.WPF;

namespace TaleKitEditor.UI.Windows {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window {

		public UiEditorLayout UiEditor;
		public MotionEditorLayout motionEditor;
		public Workspaces.StoryEditorLayout storyEditor;
		private UserControl[] workspaces;

		public MainWindow() {
			InitializeComponent();
			if (this.IsDesignMode())
				return;

			Init();
			RegisterEvents();
		}
		private void Init() {
			UiEditor = new UiEditorLayout();
			motionEditor = new MotionEditorLayout();
			storyEditor = new Workspaces.StoryEditorLayout();

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
