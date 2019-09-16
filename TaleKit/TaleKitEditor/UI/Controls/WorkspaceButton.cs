using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TaleKitEditor.UI.Controls {
	public class WorkspaceButton : Button {
		public static readonly DependencyProperty IsActiveWorkspaceProperty = DependencyProperty.RegisterAttached(nameof(IsActiveWorkspace), typeof(bool), typeof(WorkspaceButton), new PropertyMetadata(false));

		public bool IsActiveWorkspace {
			get {
				return (bool)GetValue(IsActiveWorkspaceProperty);
			}
			set {
				SetValue(IsActiveWorkspaceProperty, value);
			}
		}


	}
}
