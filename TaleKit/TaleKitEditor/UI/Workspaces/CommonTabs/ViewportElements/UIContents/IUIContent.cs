using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.UI.UIItem;

namespace TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements.UIContents {
	public interface IUIContent {
		UIRenderer OwnerRenderer {
			get;
		}

		void Render(UIItemBase data);
	}
}
