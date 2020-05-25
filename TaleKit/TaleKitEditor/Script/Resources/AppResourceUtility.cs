using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaleKitEditor.Resources {
	public enum AppResourceName {
		SelectedBackground,
	}
	public static class AppResourceUtility {
		public static object GetResource(AppResourceName resourceName) {
			return Application.Current.FindResource(resourceName.ToString());
		}
	}
}
