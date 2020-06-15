using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKitForWPF;
using TaleKitEditor.UI.Windows;


namespace TaleKitEditor {
	public class Root {
		public static Root Instance {
			get; private set;
		}

		public GLoopEngine LoopEngine {
			get; private set;
		}
		public MainWindow MainWindow {
			get; private set;
		}

		public Root() {
			Instance = this;

			InitModules();
			StartModules();
		}
		private void InitModules() {
			MainWindow = new MainWindow();
			LoopEngine = new GLoopEngine();

			MainWindow.Initialize();

		}
		private void StartModules() {
			LoopEngine.StartLoop();
			MainWindow.Show();
		}
	}
}
