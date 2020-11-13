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
using TaleKit.Datas.UI.UIItem;
using Mathf = UnityEngine.Mathf;
using UColor = UnityEngine.Color;
using UGRect = GKitForUnity.GRect;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements.UIContents {

	public partial class UIContent_MotionText : UserControl, IUIContent {

		public UIRenderer OwnerRenderer {
			get; private set;
		}

		// [ Constructor ]
		[Obsolete]
		public UIContent_MotionText() {
			InitializeComponent();
		}
		public UIContent_MotionText(UIRenderer ownerRenderer) {
			InitializeComponent();

			this.OwnerRenderer = ownerRenderer;
		}

		public void Render(UIItemBase data) {
			
		}

	}
}
