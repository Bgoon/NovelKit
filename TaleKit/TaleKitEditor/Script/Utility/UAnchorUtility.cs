extern alias GKitForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UAnchorPreset = GKitForUnity.GKit.AnchorPreset;
using UAxisAnchor = GKitForUnity.GKit.AxisAnchor;

namespace TaleKitEditor {
	public static class UAnchorUtility {
		public static HorizontalAlignment ToHorizontalAlignment(this UAxisAnchor axisAnchor) {
			switch(axisAnchor) {
				case UAxisAnchor.Min:
					return HorizontalAlignment.Left;
				case UAxisAnchor.Mid:
					return HorizontalAlignment.Center;
				case UAxisAnchor.Max:
					return HorizontalAlignment.Right;
				default:
				case UAxisAnchor.Stretch:
					return HorizontalAlignment.Stretch;
			}
		}
		public static VerticalAlignment ToVerticalAlignment(this UAxisAnchor axisAnchor) {
			switch (axisAnchor) {
				case UAxisAnchor.Min:
					return VerticalAlignment.Bottom;
				case UAxisAnchor.Mid:
					return VerticalAlignment.Center;
				case UAxisAnchor.Max:
					return VerticalAlignment.Top;
				default:
				case UAxisAnchor.Stretch:
					return VerticalAlignment.Stretch;
			}
		}
	}
}
