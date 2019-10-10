using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaleKit.Datas.Story;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	public static class OrderControlFactory {
		public static UserControl CreateControl(OrderBase order) {
			if (order is EventOrder) {
				return new EventOrderControl((EventOrder)order);
			} else if (order is LogicOrder) {
				return new LogicOrderControl((LogicOrder)order);
			} else if (order is MessageOrder) {
				return new MessageOrderControl((MessageOrder)order);
			} else if (order is UiOrder) {
				return new UiOrderControl((UiOrder)order);
			}
			throw new TypeAccessException("지원하지 않는 타입입니다.");
		}
	}
}
