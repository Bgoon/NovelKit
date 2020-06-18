using GKitForWPF;
using GKitForWPF.Graphics;
using System;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs;

namespace TaleKitEditor.UI.Dialogs {
	public partial class AddOrderPanel : UserControl {
		public StoryBlock OwnerBlock {
			get; private set;
		}

		public event Action Close;

		private OrderTypeItemView[] itemViews;

		public static AddOrderPanel Show(StoryBlock ownerBlock, Vector2 talePosition) {
			AddOrderPanel panel = new AddOrderPanel(ownerBlock);
			TaleDialog.Show(panel, talePosition);

			return panel;
		}

		[Obsolete]
		internal AddOrderPanel() {
			InitializeComponent();
		}
		public AddOrderPanel(StoryBlock ownerBlock) {
			InitializeComponent();
			this.OwnerBlock = ownerBlock;

			CreateTypeItems();
		}

		private void TypeItemView_Click(OrderType obj) {
			OwnerBlock.AddOrder(obj);

			Close?.Invoke();
		}

		private void CreateTypeItems() {
			string[] orderTypeTexts = Enum.GetNames(typeof(OrderType));
			itemViews = new OrderTypeItemView[orderTypeTexts.Length];

			for (int i = 0; i < orderTypeTexts.Length; ++i) {
				string orderTypeText = orderTypeTexts[i];

				OrderTypeItemView typeItemView = itemViews[i] = new OrderTypeItemView();
				typeItemView.TypeNameText = orderTypeText;
				typeItemView.OrderType = (OrderType)Enum.Parse(typeof(OrderType), orderTypeText);
				typeItemView.Click += TypeItemView_Click;

				if (i == orderTypeTexts.Length - 1) {
					typeItemView.IsSeparatorVisible = false;
				}

				TypeItemStackPanel.Children.Add(typeItemView);
			}
		}
	}
}
