using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit;
using TaleKitEditor.UI.Windows;


namespace TaleKitEditor {
	public class Root {

		public GLoopEngine LoopEngine {
			get; private set;
		}
		public MainWindow MainWindow {
			get; private set;
		}

		public Root() {
			InitModules();
			StartModules();
		}
		private void InitModules() {

			MainWindow = new MainWindow();
			LoopEngine = new GLoopEngine();

		}
		private void StartModules() {
			LoopEngine.StartLoop();
			MainWindow.Show();
		}
	}
}
