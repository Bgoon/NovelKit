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
using TaleKit.Datas;
using TaleKit.Datas.Story;
using TaleKitEditor.UI.Windows;
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.Views;
using TaleKitEditor.UI.Workspaces.UiWorkspaceTabs;
using GKitForWPF;
using GKitForWPF.UI.Controls;
using TaleKit.Datas.UI;
using TaleKitEditor.UI.Workspaces.CommonTabs.ViewportElements;
using TaleKitEditor.UI.Workspaces.CommonTabs;
using System.Globalization;
using TaleKit.Datas.ModelEditor;
using System.Reflection;
using System.Diagnostics;
using TaleKit.Datas.Motion;
using TaleKit.Datas.UI.UIItem;
using TaleKit.Datas.Story.StoryBlock;

namespace TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs {
	/// <summary>
	/// StoryBoard.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class StoryBlockTab : UserControl {
		private static Root Root => Root.Instance;
		private static GLoopEngine LoopEngine => Root.LoopEngine;
		private static MainWindow MainWindow => Root.MainWindow;
		private static TaleData EditingTaleData => MainWindow.EditingTaleData;
		private static StoryFile EditingStoryFile => EditingTaleData.StoryFile;
		private static UIFile EditingUiFile => EditingTaleData.UiFile;
		private static ViewportTab ViewportTab => MainWindow.ViewportTab;

		private readonly Dictionary<StoryBlockBase, StoryBlockView> dataToViewDict;
		public StoryBlockView SelectedBlockViewSingle {
			get {
				if (StoryBlockTreeView.SelectedItemSet.Count > 0) {
					return (StoryBlockView)StoryBlockTreeView.SelectedItemSet.Last();
				}
				return null;
			}
		}
		public StoryBlock_Item SelectedBlockSingle {
			get {
				StoryBlockView selectedItemView = SelectedBlockViewSingle;
				return selectedItemView == null ? null : selectedItemView.Data as StoryBlock_Item;
			}
		}

		public StoryClip EditingClip {
			get; private set;
		}

		// Collection
		private readonly HashSet<UIRenderer> RenderedRendererHashSet;
		private readonly HashSet<UIMotionPlayer> UiMotionSet;

		// [ Constructor ]
		public StoryBlockTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			// Init members
			dataToViewDict = new Dictionary<StoryBlockBase, StoryBlockView>();
			RenderedRendererHashSet = new HashSet<UIRenderer>();
			UiMotionSet = new HashSet<UIMotionPlayer>();

			// Register events
			LoopEngine.AddLoopAction(OnTick);

			StoryBlockListController.CreateItemButtonClick += StoryBlockListController_CreateItemButtonClick;
			StoryBlockListController.RemoveItemButtonClick += StoryBlockListController_RemoveItemButtonClick;

			StoryBlockTreeView.ItemMoved += StoryBlockListView_ItemMoved;
			StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
			
			MainWindow.ProjectPreloaded += MainWindow_ProjectPreloaded;
			MainWindow.ProjectLoaded += MainWIndow_ProjectLoaded;
			MainWindow.ProjectUnloaded += MainWindow_ProjectUnloaded;

			DetachClip();
		}

		// [ Event ]
		private void OnTick() {
			UpdateMotion();
		}

		private void MainWindow_ProjectPreloaded(TaleData taleData) {
			EditingStoryFile.BlockCreated += StoryFile_ItemCreated;
			EditingStoryFile.BlockRemoved += StoryFile_ItemRemoved;
		}
		private void MainWIndow_ProjectLoaded(TaleData taleData) {
			AttachClip(EditingStoryFile.RootClip);
		}
		private void MainWindow_ProjectUnloaded(TaleData taleData) {
			EditingStoryFile.BlockCreated -= StoryFile_ItemCreated;
			EditingStoryFile.BlockRemoved -= StoryFile_ItemRemoved;

			DetachClip();
		}

		private void StoryBlockListController_CreateItemButtonClick() {
			EditingStoryFile.CreateStoryBlock(EditingClip, StoryBlockType.Item);
		}
		private void StoryBlockListController_RemoveItemButtonClick() {
			foreach (StoryBlockView itemView in StoryBlockTreeView.SelectedItemSet) {
				StoryBlockBase data = itemView.Data;

				EditingStoryFile.RemoveStoryBlock(data);
			}
		}

		private void StoryFile_ItemCreated(StoryBlockBase item, StoryClip parentItem) {
			if (parentItem == null)
				return;
			if (parentItem != EditingClip)
				return;

			//Create view
			StoryBlockView itemView = new StoryBlockView(item);
			StoryBlockTreeView.ChildItemCollection.Add(itemView);
			itemView.ParentItem = StoryBlockTreeView;

			dataToViewDict.Add(item, itemView);
		}
		private void StoryFile_ItemRemoved(StoryBlockBase item, StoryClip parentItem) {
			//Remove view
			dataToViewDict[item].DetachParent();

			dataToViewDict.Remove(item);
		}

		private void StoryBlockListView_ItemMoved(ITreeItem itemView, ITreeFolder oldParentView, ITreeFolder newParentView, int index) {
			//Data에 적용하기
			StoryBlockBase item = ((StoryBlockView)itemView).Data;

			StoryClip editingClip = EditingClip;
			editingClip.RemoveChildItem(item);
			editingClip.InsertChildItem(index, item);
		}

		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			OnStoryBlockSelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			OnStoryBlockSelectionChanged();
		}
		private void OnStoryBlockSelectionChanged() {
			ApplyBlockToSelectionToRenderer(true);
		}

		// [ Control ]
		// Block selection
		public void SelectAroundBlock(int indexOffset) {
			if (StoryBlockTreeView.SelectedItemSet.Count != 1)
				return;

			StoryClip editingClip = EditingClip;
			int index = editingClip.BlockItemList.IndexOf(SelectedBlockSingle);
			
			if (index + indexOffset < 0 || index + indexOffset >= editingClip.BlockItemList.Count)
				return;

			StoryBlockTreeView.SelectedItemSet.SetSelectedItem(dataToViewDict[editingClip.BlockItemList[index + indexOffset]]);
		}

		// Block applying
		public void ApplyBlockToSelectionToRenderer(bool playMotion = false) {
			if(StoryBlockTreeView.SelectedItemSet.Count == 1) {
				StoryBlockBase selectedBlockBase = (StoryBlockTreeView.SelectedItemSet.First as StoryBlockView).Data;
				int selectedBlockIndex = EditingStoryFile.RootClip.BlockItemList.IndexOf(selectedBlockBase);

				ApplyBlocksToRenderer(selectedBlockIndex, playMotion);
			} else {
				ApplyBlocksToRenderer(-1, false);
			}
		}
		public void ApplyBlocksToRenderer(int lastBlockIndex, bool playMotion = false) {
			StopMotion();

			if (!ViewportTab.PlayStateButton.IsActive || lastBlockIndex < 0) {
				UIRenderer rootRenderer = EditingUiFile.Guid_To_RendererDict[EditingUiFile.UISnapshot.rootUiItem.guid] as UIRenderer;
				rootRenderer.Render(true);
			} else {
				StoryClip editingClip = EditingClip;

				// Find last cache
				UISnapshot snapshot = null;
				int cacheIndex = -1;
				for(int blockI=lastBlockIndex-1; blockI>=0; --blockI) {
					StoryBlockBase block = editingClip.BlockItemList[blockI];

					if(block.HasUiCache) {
						snapshot = block.UICacheSnapshot.Clone();
						cacheIndex = blockI;
					}
				}
				if (snapshot == null) {
					snapshot = EditingUiFile.UISnapshot.Clone();
				}

				// Apply orders
				int applyTargetBlockIndex = playMotion ? lastBlockIndex - 1 : lastBlockIndex;
				for (int blockI = cacheIndex + 1; blockI <= applyTargetBlockIndex; ++blockI) {
					StoryBlockBase block = editingClip.BlockItemList[blockI];
					if (!block.isVisible)
						continue;

					snapshot.ApplyStoryBlock(block);
				}
				foreach(UIItemBase UiItem in snapshot.GetUiItems()) {
					UIRenderer renderer = EditingUiFile.Guid_To_RendererDict[UiItem.guid] as UIRenderer;
					renderer.RenderFromData(UiItem);
				}

				// Play the motion
				if(playMotion) {
					StoryBlockBase lastBlock = EditingStoryFile.RootClip.BlockItemList[lastBlockIndex];

					if (lastBlock.isVisible) {
						switch (lastBlock.blockType) {
							case StoryBlockType.Item:
								foreach (OrderBase order in (lastBlock as StoryBlock_Item).OrderList) {
									if (order.orderType == OrderType.UI) {
										Order_UI order_UI = order as Order_UI;

										if (string.IsNullOrEmpty(order_UI.targetUIGuid))
											continue;

										UIItemBase UiItem = EditingUiFile.UISnapshot.GetUiItem(order_UI.targetUIGuid);

										if (UiItem == null)
											continue;

										UIRenderer renderer = EditingUiFile.Guid_To_RendererDict[UiItem.guid] as UIRenderer;

										StartMotion(UiItem, renderer, order_UI);
									}
								}
								break;
							case StoryBlockType.Clip:
								break;
						}
					}
				}
			}
		}

		// TODO : 관련된 코드들 Order 내부로 이동
		// 파라미터들 Interface화 해서 클라이언트와 같이 쓸 것
		// Order
		private void StartMotion(UIItemBase UiItem, UIRenderer UiRenderer, Order_UI order_UI) {
			UIItemBase prevData = GetOrdersAppliedData(UiItem, GetSelectedBlockIndex() - 1);

			UiMotionSet.Add(new UIMotionPlayer(UiRenderer, order_UI, prevData, GetOrderAppliedData(prevData, order_UI)));
		}
		private void ApplyOrderToRendererImmediately(UIRenderer UiRenderer, Order_UI order_UI) {
			UiRenderer.RenderFromData(order_UI.UIKeyData);
		}

		private UIItemBase GetOrdersAppliedData(UIItemBase UiItem, int lastBlockIndex) {
			UIItemBase newItem = UiItem.Clone() as UIItemBase;

			for(int i=0; i<=lastBlockIndex; ++i) {
				StoryBlockBase blockBase = EditingStoryFile.RootClip.BlockItemList[i];
				switch (blockBase.blockType) {
					case StoryBlockType.Item:
						foreach (OrderBase order in (blockBase as StoryBlock_Item).OrderList) {
							if (order.orderType == OrderType.UI) {
								Order_UI order_UI = order as Order_UI;
								if (order_UI.targetUIGuid != newItem.guid)
									continue;

								ApplyOrder(newItem, order_UI);
							}
						}
						break;
					case StoryBlockType.Clip:
						break;
				}
			}
			return newItem;
		}
		private UIItemBase GetOrderAppliedData(UIItemBase UiItem, Order_UI order_UI) {
			UIItemBase newData = UiItem.Clone() as UIItemBase;
			ApplyOrder(newData, order_UI);

			return newData;
		}
		private void ApplyOrder(UIItemBase UiItem, Order_UI order_UI) {
			UIItemBase keyData = order_UI.UIKeyData;

			foreach (FieldInfo keyFieldInfo in keyData.GetType().GetFields()) {
				if (keyFieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count() == 0)
					continue;

				if (!keyData.KeyFieldNameHashSet.Contains(keyFieldInfo.Name))
					continue;

				keyFieldInfo.SetValue(UiItem, keyFieldInfo.GetValue(keyData));
			}
		}

		// Motion
		public class UIMotionPlayer {
			private TaleData EditingData => order_UI.OwnerBlock.OwnerFile.OwnerTaleData;
			private MotionFile EditingMotionFile => EditingData.MotionFile;
		
			private readonly static BindingFlags PublicRuntimeBindingFlags = BindingFlags.Public | BindingFlags.Instance;

			// TODO : UIRenderer 를 인터페이스로 변경해서 참조
			public readonly UIRenderer UIRenderer;
			public readonly Order_UI order_UI;

			public readonly UIItemBase prevKeyData;
			public readonly UIItemBase currentKeyData;

			public float ActualTimeSec => timeSec - order_UI.delaySec;
			public float timeSec;

			public bool IsComplete {
				get; private set;
			}
			public bool IsOverTime => ActualTimeSec > order_UI.durationSec;

			public UIMotionPlayer(UIRenderer UiRenderer, Order_UI order_UI, UIItemBase prevKeyData, UIItemBase currentKeyData) {
				this.UIRenderer = UiRenderer;
				this.order_UI = order_UI;

				this.prevKeyData = prevKeyData;
				this.currentKeyData = currentKeyData;
			}

			public void AddTime(float deltaSec) {
				this.timeSec += deltaSec;

				Render();

				if(IsOverTime) {
					IsComplete = true;
				}
			}
			private void Render() {
				UIItemBase breakDownData = GetBreakDownData();

				UIRenderer.RenderFromData(breakDownData);
			}
			private UIItemBase GetBreakDownData() {
				UIItemBase breakDownData = prevKeyData.Clone() as UIItemBase;

				if (timeSec > order_UI.delaySec) {
					float normalTime = Mathf.Clamp01(ActualTimeSec / order_UI.durationSec);
					if(!string.IsNullOrEmpty(order_UI.easingKey)) {
						if(EditingMotionFile.motionData.itemDict.ContainsKey(order_UI.easingKey)) {
							normalTime = EditingMotionFile.motionData.GetMotionValue(order_UI.easingKey, normalTime);
						}
					}

					foreach (var fieldInfo in currentKeyData.GetType().GetFields(PublicRuntimeBindingFlags)) {
						if (fieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count() == 0)
							continue;

						object prevValue = fieldInfo.GetValue(prevKeyData);
						object currentValue = fieldInfo.GetValue(currentKeyData);

						object breakDownValue = GetBreakDownField(prevValue, currentValue, normalTime, fieldInfo);

						fieldInfo.SetValue(breakDownData, breakDownValue);
					}
				}

				return breakDownData;
			}

			private object GetBreakDownField(object prevField, object currentField, float normalTime, FieldInfo fieldInfo) {
				if (fieldInfo.FieldType.IsNumericType()) {
					// Numeric
					object breakDownValue = GetBreakDownNumericValue(prevField, currentField, normalTime, fieldInfo.FieldType);

					if (breakDownValue != null) {
						return breakDownValue;
					}
				} else if (fieldInfo.FieldType.IsStruct()) {
					// Struct
					object prevStructValue = fieldInfo.GetValue(prevKeyData);
					object currentStructValue = fieldInfo.GetValue(currentKeyData);
					object breakDownStructValue = currentStructValue;

					foreach (FieldInfo structFieldInfo in currentStructValue.GetType().GetFields(PublicRuntimeBindingFlags)) {
						object prevStructFieldValue = structFieldInfo.GetValue(prevStructValue);
						object currentStructFieldValue = structFieldInfo.GetValue(currentStructValue);

						object breakDownValue = GetBreakDownField(prevStructFieldValue, currentStructFieldValue, normalTime, structFieldInfo);
						if (breakDownValue != null) {
							structFieldInfo.SetValue(breakDownStructValue, breakDownValue);
						}
					}
					return breakDownStructValue;
				}
				// Other
				return currentField;
			}
			private object GetBreakDownNumericValue(object prevValue, object currentValue, float normalTime, Type fieldType) {
				double? prevNumericValue = GetNumber(prevValue);
				double? currentNumericValue = GetNumber(currentValue);

				if (currentNumericValue.HasValue && prevNumericValue.HasValue) {
					double breakDownNumericValue = prevNumericValue.Value + (currentNumericValue.Value - prevNumericValue.Value) * normalTime;
					return Convert.ChangeType(breakDownNumericValue, fieldType);
				} else {
					return null;
				}
			}
			private double? GetNumber(object value) {
				double result;
				if(Double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result)) {
					return result;
				}
				return null;
			}
		}

		private void UpdateMotion() {
			foreach(UIMotionPlayer motion in UiMotionSet) {
				if(!motion.IsComplete) {
					motion.AddTime(LoopEngine.DeltaSeconds);
				}
			}
		}
		private void StopMotion() {
			UiMotionSet.Clear();
		}

		// Attach
		public void AttachClip(StoryClip clip) {
			DetachClip();

			EditingClip = clip;

			foreach(var item in clip.BlockItemList) {
				StoryFile_ItemCreated(item, clip);
			}

			EditingClipNameRun.Text = clip.name;
		}
		public void DetachClip() {
			EditingClip = null;

			StoryBlockTreeView.ChildItemCollection.Clear();
			dataToViewDict.Clear();

			EditingClipNameRun.Text = "Root";
		}

		// Utility
		private int GetSelectedBlockIndex() {
			if (StoryBlockTreeView.SelectedItemSet.Count == 1) {
				StoryBlockBase selectedBlockBase = (StoryBlockTreeView.SelectedItemSet.First as StoryBlockView).Data;
				return EditingStoryFile.RootClip.BlockItemList.IndexOf(selectedBlockBase);
			}
			return -1;
		}
	}
}
