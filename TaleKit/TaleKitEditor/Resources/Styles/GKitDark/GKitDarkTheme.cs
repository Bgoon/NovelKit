using System;
using GKit.WPF.Resources;
using Xceed.Wpf.AvalonDock.Themes;

namespace GKit.AvalonDock.Themes {
	public class GKitDarkTheme : Theme {

		public override Uri GetResourceUri() {
				return new Uri(StyleResource.GetUri("TaleKitEditor", "Resources/Styles/GKitDark/GKitDark.xaml"), UriKind.Absolute);
		}
	}
}
