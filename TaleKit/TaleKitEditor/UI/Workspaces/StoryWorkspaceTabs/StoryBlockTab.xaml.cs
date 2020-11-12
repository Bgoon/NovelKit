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
		private static UiFile EditingUiFile => EditingTaleData.UiFile;
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

		public StoryClip EditingClipAuto {
			get {
				if (EditingClip == null) {
					return EditingStoryFile.RootClip;
				} else {
					return EditingClip;
				}
			}
		}
		public StoryClip EditingClip {
			get; private set;
		}

		// Collection
		private readonly HashSet<UiRenderer> RenderedRendererHashSet;
		private readonly HashSet<UiMotion> UiMotionSet;

		// [ Constructor ]
		public StoryBlockTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			// Init members
			dataToViewDict = new Dictionary<StoryBlockBase, StoryBlockView>();
			RenderedRendererHashSet = new HashSet<UiRenderer>();
			UiMotionSet = new HashSet<UiMotion>();

			// Register events
			LoopEngine.AddLoopAction(OnTick);

			StoryBlockListController.CreateItemButtonClick += StoryBlockListController_CreateItemButtonClick;
			StoryBlockListController.RemoveItemButtonClick += StoryBlockListController_RemoveItemButtonClick;

			StoryBlockTreeView.ItemMoved += StoryBlockListView_ItemMoved;
			StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedItemSet_SelectionAdded;
			StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedItemSet_SelectionRemoved;
			
			MainWindow.ProjectLoaded += MainWindow_DataLoaded;
			MainWindow.ProjectUnloaded += MainWindow_DataUnloaded;
		}

		// [ Event ]
		private void OnTick() {
			UpdateMotion();
		}

		private void MainWindow_DataLoaded(TaleData obj) {
			EditingStoryFile.BlockItemCreated += StoryFile_ItemCreated;
			EditingStoryFile.BlockItemRemoved += StoryFile_ItemRemoved;
		}
		private void MainWindow_DataUnloaded(TaleData obj) {
			EditingStoryFile.BlockItemCreated -= StoryFile_ItemCreated;
			EditingStoryFile.BlockItemRemoved -= StoryFile_ItemRemoved;

			DetachClip();
		}

		private void StoryBlockListController_CreateItemButtonClick() {
			EditingStoryFile.CreateStoryBlock_Item(EditingClipAuto);
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
			if (parentItem != EditingClipAuto)
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
			//StoryFile.RemoveStoryBlockItem(item);

			dataToViewDict.Remove(item);
		}

		private void StoryBlockListView_ItemMoved(ITreeItem itemView, ITreeFolder oldParentView, ITreeFolder newParentView, int index) {
			//Data에 적용하기
			StoryBlockBase item = ((StoryBlockView)itemView).Data;

			StoryClip editingClip = EditingClipAuto;
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
		// Block
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

			// TODO : Cache 사용하도록 수정
			if (!ViewportTab.PlayStateButton.IsActive || lastBlockIndex < 0) {
				UiRenderer rootRenderer = EditingUiFile.Guid_To_RendererDict[EditingUiFile.UiSnapshot.rootUiItem.guid] as UiRenderer;
				rootRenderer.Render(true);
			} else {
				StoryClip editingClip = EditingClipAuto;

				// Find last cache
				UiSnapshot snapshot = null;
				int cacheIndex = -1;
				for(int blockI=lastBlockIndex-1; blockI>=0; --blockI) {
					StoryBlockBase block = editingClip.BlockItemList[blockI];

					if(block.HasUiCache) {
						snapshot = block.UiCacheSnapshot.Clone();
						cacheIndex = blockI;
					}
				}
				if(snapshot == null) {
					snapshot = EditingUiFile.UiSnapshot.Clone();
				}

				// Apply orders
				int applyTargetBlockIndex = playMotion ? lastBlockIndex - 1 : lastBlockIndex;
				for (int blockI = cacheIndex + 1; blockI <= applyTargetBlockIndex; ++blockI) {
					StoryBlockBase block = editingClip.BlockItemList[blockI];
					if (!block.isVisible)
						continue;

					snapshot.ApplyStoryBlockBase(block);
				}
				foreach(UiItemBase UiItem in snapshot.GetUiItems()) {
					UiRenderer renderer = EditingUiFile.Guid_To_RendererDict[UiItem.guid] as UiRenderer;
					renderer.RenderFromData(UiItem);
				}

				// Play the motion
				if(playMotion) {
					StoryBlockBase lastBlock = EditingStoryFile.RootClip.BlockItemList[lastBlockIndex];

					if (lastBlock.isVisible) {
						switch (lastBlock.Type) {
							case StoryBlockType.StoryBlock:
								foreach (OrderBase order in (lastBlock as StoryBlock_Item).OrderList) {
									if (order.OrderType == OrderType.UI) {
										Order_UI order_UI = order as Order_UI;

										if (string.IsNullOrEmpty(order_UI.targetUiGuid))
											continue;

										UiItemBase UiItem = EditingUiFile.UiSnapshot.GetUiItem(order_UI.targetUiGuid);

										if (UiItem == null)
											continue;

										UiRenderer renderer = EditingUiFile.Guid_To_RendererDict[UiItem.guid] as UiRenderer;

										StartMotion(UiItem, renderer, order_UI);
									}
								}
								break;
							case StoryBlockType.StoryClipBlock:
								break;
						}
					}
				}
			}
		}

		// TODO : 관련된 코드들 Order 내부로 이동
		// 파라미터들 Interface화 해서 클라이언트와 같이 쓸 것
		// Order
		private void StartMotion(UiItemBase UiItem, UiRenderer UiRenderer, Order_UI order_UI) {
			UiItemBase prevData = GetOrdersAppliedData(UiItem, GetSelectedBlockIndex() - 1);

			UiMotionSet.Add(new UiMotion(UiRenderer, order_UI, prevData, GetOrderAppliedData(prevData, order_UI)));
		}
		private void ApplyOrderToRendererImmediately(UiRenderer UiRenderer, Order_UI order_UI) {
			UiRenderer.RenderFromData(order_UI.UiKeyData);
		}

		private UiItemBase GetOrdersAppliedData(UiItemBase UiItem, int lastBlockIndex) {
			UiItemBase newItem = UiItem.Clone() as UiItemBase;

			for(int i=0; i<=lastBlockIndex; ++i) {
				StoryBlockBase blockBase = EditingStoryFile.RootClip.BlockItemList[i];
				switch (blockBase.Type) {
					case StoryBlockType.StoryBlock:
						foreach (OrderBase order in (blockBase as StoryBlock_Item).OrderList) {
							if (order.OrderType == OrderType.UI) {
								Order_UI order_UI = order as Order_UI;
								if (order_UI.targetUiGuid != newItem.guid)
									continue;

								ApplyOrder(newItem, order_UI);
							}
						}
						break;
					case StoryBlockType.StoryClipBlock:
						break;
				}
			}
			return newItem;
		}
		private UiItemBase GetOrderAppliedData(UiItemBase UiItem, Order_UI order_UI) {
			UiItemBase newData = UiItem.Clone() as UiItemBase;
			ApplyOrder(newData, order_UI);

			return newData;
		}
		private void ApplyOrder(UiItemBase UiItem, Order_UI order_UI) {
			UiItemBase keyData = order_UI.UiKeyData;

			foreach (FieldInfo keyFieldInfo in keyData.GetType().GetFields()) {
				if (keyFieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count() == 0)
					continue;

				if (!keyData.KeyFieldNameHashSet.Contains(keyFieldInfo.Name))
					continue;

				keyFieldInfo.SetValue(UiItem, keyFieldInfo.GetValue(keyData));
			}
		}

		// Motion
		public class UiMotion {
			private TaleData EditingData => order_UI.OwnerBlock.OwnerFile.OwnerTaleData;
			private MotionFile EditingMotionFile => EditingData.MotionFile;
		
			private readonly static BindingFlags PublicRuntimeBindingFlags = BindingFlags.Public | BindingFlags.Instance;

			public readonly UiRenderer UiRenderer;
			public readonly Order_UI order_UI;

			public readonly UiItemBase prevKeyData;
			public readonly UiItemBase currentKeyData;

			public float ActualTimeSec => timeSec - order_UI.delaySec;
			public float timeSec;

			public bool IsComplete {
				get; private set;
			}
			public bool IsOverTime => ActualTimeSec > order_UI.durationSec;

			public UiMotion(UiRenderer UiRenderer, Order_UI order_UI, UiItemBase prevKeyData, UiItemBase currentKeyData) {
				this.UiRenderer = UiRenderer;
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
				UiItemBase breakDownData = GetBreakDownData();

				UiRenderer.RenderFromData(breakDownData);
			}
			private UiItemBase GetBreakDownData() {
				UiItemBase breakDownData = prevKeyData.Clone() as UiItemBase;

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
			foreach(UiMotion motion in UiMotionSet) {
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
			EditingClip = clip;
		}
		public void DetachClip() {
			EditingClip = null;
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
