using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GKit;
using GKit.WPF;
using GKit.WPF.Resources;
using TaleKitEditor.UI.Windows;

namespace TaleKitEditor {
	/// <summary>
	/// App.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class App : Application {
		private void OnStartup(object sender, StartupEventArgs e) {
			LoadResources();

			CreateRoot();
		}
		private void LoadResources() {
			StyleResource.Apply(Resources, ThemeType.FlatTheme);
			StyleResource.ApplyCustom(Resources, "PendulumMotionEditor", "Resources/Style/EditorStyle.xaml");
		}
		private void CreateRoot() {
			new Root();
		}
	}
}
