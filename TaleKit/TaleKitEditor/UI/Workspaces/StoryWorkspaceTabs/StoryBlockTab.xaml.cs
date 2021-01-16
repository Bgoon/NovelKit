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
using TaleKitEditor.UI.Workspaces.UIWorkspaceTabs;
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
using TaleKitEditor.UI.Dialogs;

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
		private static UIFile EditingUIFile => EditingTaleData.UIFile;
		private static ViewportTab ViewportTab => MainWindow.ViewportTab;

		public readonly Dictionary<StoryBlockBase, StoryBlockView> Data_To_ViewDict;
		public StoryBlockView SelectedBlockViewSingle {
			get {
				if (StoryBlockTreeView.SelectedItemSet.Count > 0) {
					return (StoryBlockView)StoryBlockTreeView.SelectedItemSet.Last();
				}
				return null;
			}
		}
		public StoryBlock_Item SelectedBlock_ItemSingle {
			get {
				StoryBlockView selectedItemView = SelectedBlockViewSingle;
				return selectedItemView == null ? null : selectedItemView.Data as StoryBlock_Item;
			}
		}

		public StoryClip EditingClip {
			get; private set;
		}

		// Preview
		private readonly Stack<StoryClipState> PreviewClipStack;
		private readonly HashSet<UIRendererMotion> PlayingMotionSet;

		// [ Constructor ]
		public StoryBlockTab() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			// Init members
			Data_To_ViewDict = new Dictionary<StoryBlockBase, StoryBlockView>();
			PlayingMotionSet = new HashSet<UIRendererMotion>();
			PreviewClipStack = new Stack<StoryClipState>();

			// Register events
			LoopEngine.AddLoopAction(OnTick);

			StoryBlockListController.CreateItemButtonClick += StoryBlockListController_CreateItemButtonClick;
			StoryBlockListController.RemoveItemButtonClick += StoryBlockListController_RemoveItemButtonClick;

			StoryBlockTreeView.ItemMoved += MoveBlockPosition;
			StoryBlockTreeView.SelectedItemSet.SelectionAdded += SelectedBlockSet_SelectionAdded;
			StoryBlockTreeView.SelectedItemSet.SelectionRemoved += SelectedBlockSet_SelectionRemoved;
			
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
			EditingStoryFile.BlockCreated += CreateBlockView;
			EditingStoryFile.BlockRemoved += RemoveBlockView;
		}
		private void MainWIndow_ProjectLoaded(TaleData taleData) {
			AttachClip(EditingStoryFile.RootClip);
		}
		private void MainWindow_ProjectUnloaded(TaleData taleData) {
			EditingStoryFile.BlockCreated -= CreateBlockView;
			EditingStoryFile.BlockRemoved -= RemoveBlockView;

			DetachClip();
		}

		private void StoryBlockListController_CreateItemButtonClick() {
			List<Dialogs.MenuItem> menuItemList = new List<Dialogs.MenuItem>();
			foreach(var type in Enum.GetValues(typeof(StoryBlockType))) {
				StoryBlockType blockType = (StoryBlockType)type;

				menuItemList.Add(new Dialogs.MenuItem(blockType.ToString(), () => {
					EditingStoryFile.CreateStoryBlock(EditingClip, blockType);
				}));
			}
			MenuPanel.ShowDialog(menuItemList.ToArray());
		}
		private void StoryBlockListController_RemoveItemButtonClick() {
			foreach (StoryBlockView itemView in StoryBlockTreeView.SelectedItemSet) {
				StoryBlockBase data = itemView.Data;

				EditingStoryFile.RemoveStoryBlock(data);
			}
		}

		private void CreateBlockView(StoryBlockBase block, StoryClip parentClip) {
			if (parentClip == null)
				return;
			if (parentClip != EditingClip)
				return;

			// Create view
			StoryBlockView blockView = new StoryBlockView(block);
			StoryBlockTreeView.ChildItemCollection.Add(blockView);
			blockView.ParentItem = StoryBlockTreeView;

			blockView.ViewContent.UpdatePreviewText();

			Data_To_ViewDict.Add(block, blockView);

			// Register events
			blockView.RegisterRightClickEvent(StoryBlockView_RightClick, true);
		}
		private void RemoveBlockView(StoryBlockBase item, StoryClip parentItem) {
			//Remove view
			Data_To_ViewDict[item].DetachParent();

			Data_To_ViewDict.Remove(item);
		}

		private void MoveBlockPosition(ITreeItem itemView, ITreeFolder oldParentView, ITreeFolder newParentView, int index) {
			//Data에 적용하기
			StoryBlockBase item = ((StoryBlockView)itemView).Data;

			StoryClip editingClip = EditingClip;
			editingClip.RemoveChildItem(item);
			editingClip.InsertChildItem(index, item);
		}

		private void SelectedBlockSet_SelectionAdded(ISelectable item) {
			SelectedBlockSet_SelectionChanged();
		}
		private void SelectedBlockSet_SelectionRemoved(ISelectable item) {
			SelectedBlockSet_SelectionChanged();
		}
		private void SelectedBlockSet_SelectionChanged() {
			ClearPreviewClipStack();

			ApplyBlockToSelectionToRenderer(true);
		}

		private void StoryBlockView_RightClick() {
			List<Dialogs.MenuItem> menuItemList = new List<Dialogs.MenuItem>() {
				new Dialogs.MenuItem("복사", CopySelectedBlocks),
				new Dialogs.MenuItem("붙여넣기", PasteBlocks),
			};

			if(CanMerge()) {
				menuItemList.Add(new Dialogs.MenuItem("Clip으로 합치기", MergeBlocks));
			}

			MenuPanel.ShowDialog(menuItemList.ToArray());

			bool CanMerge() {
				StoryBlockBase prevBlock = null;
				List<int> indexList = new List<int>();
				foreach (var item in StoryBlockTreeView.SelectedItemSet) {
					StoryBlockView blockView = item as StoryBlockView;
					StoryBlockBase block = blockView.Data;

					if(prevBlock == null) {
						prevBlock = block;
						indexList.Add(prevBlock.ParentClip.BlockItemList.IndexOf(prevBlock));
					} else {
						if(prevBlock.ParentClip != block.ParentClip) {
							return false;
						}
						indexList.Add(block.ParentClip.BlockItemList.IndexOf(block));

					}
				}

				indexList.Sort();
				for(int i=1; i<indexList.Count; ++i) {
					int indexDelta = indexList[i] - indexList[i - 1];
					if(Mathf.Abs(indexDelta) > 1) {
						return false;
					}
				}
				return true;
			}
		}

		// [ Block functions ]
		private void CopySelectedBlocks() {

		}
		private void PasteBlocks() {

		}
		private void MergeBlocks() {
		 
		}

		// [ Control ]
		// Attach
		public void AttachClip(StoryClip clip) {
			DetachClip();

			EditingClip = clip;
			foreach(var item in clip.BlockItemList) {
				CreateBlockView(item, clip);
			}
			EditingClipNameRun.Text = clip.name;

			UpdateBlockPreviews();
		}
		public void DetachClip() {
			if(EditingClip != null) {
				foreach(StoryBlockBase block in EditingClip.BlockItemList) {
					block.ClearCache();
				}
			}
			foreach(var viewPair in Data_To_ViewDict) {
				viewPair.Value.Dispose();
			}

			EditingClip = null;

			StoryBlockTreeView.ChildItemCollection.Clear();
			Data_To_ViewDict.Clear();

			EditingClipNameRun.Text = "Root";
		}

		public void UpdateBlockPreviews() {
			foreach(StoryBlockView blockView in Data_To_ViewDict.Values) {
				blockView.ViewContent.UpdatePreviewText();
			}
		}

		// Block selection
		public void SelectNextBlock(bool usePreviewClipStack = true) {
			if (StoryBlockTreeView.SelectedItemSet.Count != 1)
				return;

			for(; ;) {
				StopMotion();

				if(usePreviewClipStack && PreviewClipStack.Count > 0) {
					// 진입한 Clip 기준으로 넘김

					StoryClipState clipState = PreviewClipStack.Peek();
					StoryClip clip = clipState.storyClip;
					int index = ++clipState.selectedIndex;

					if(index >= clip.BlockItemList.Count) {
						PopPreviewClipStack();
						continue;
					}

					StoryBlockBase block = clip.BlockItemList[index];
					if (block.blockType == StoryBlockType.Item) {
						if ((block as StoryBlock_Item).passTrigger == StoryBlockTrigger.None)
							return;
					}

					ApplyBlockToRendererWithMotion(block, true);
					break;
				} else {
					// Edit중인 클립 기준으로 넘김

					StoryClip clip = EditingClip;
					int index = clip.BlockItemList.IndexOf(SelectedBlockViewSingle.Data);

					if (index + 1 >= clip.BlockItemList.Count)
						return;

					StoryBlockTreeView.SelectedItemSet.SetSelectedItem(Data_To_ViewDict[clip.BlockItemList[index + 1]]);
					break;
				}
			}
		}

		// Block applying
		public void ApplyBlockToSelectionToRenderer(bool playMotion = false) {
			if(StoryBlockTreeView.SelectedItemSet.Count == 1) {
				StoryBlockBase selectedBlockBase = (StoryBlockTreeView.SelectedItemSet.First as StoryBlockView).Data;
				int selectedBlockIndex = EditingClip.BlockItemList.IndexOf(selectedBlockBase);

				ApplyBlocksToRenderer(EditingClip, selectedBlockIndex, playMotion);
			} else {
				ApplyBlocksToRenderer(EditingClip, -1, false);
			}
		}
		public void ApplyBlocksToRenderer(StoryClip clip, int targetBlockIndex, bool playMotion = false) {
			StopMotion();

			if (!ViewportTab.PlayStateButton.IsActive || targetBlockIndex < 0) {
				// Render origin UI
				UIRenderer rootRenderer = EditingUIFile.Guid_To_RendererDict[EditingUIFile.UISnapshot.rootUIItem.guid] as UIRenderer;
				rootRenderer.Render(true);
			} else {
				UISnapshot snapshot = GetUISnapshotWithBlockApplied(clip, playMotion ? targetBlockIndex -1 : targetBlockIndex);

				foreach (UIItemBase UIItem in snapshot.GetUIItems()) {
					UIRenderer renderer = EditingUIFile.Guid_To_RendererDict[UIItem.guid] as UIRenderer;
					renderer.RenderFromData(UIItem);
				}

				// Play motion by current block
				if (!playMotion)
					return;
				StoryBlockBase lastBlock = clip.BlockItemList[targetBlockIndex];

				if (!lastBlock.isVisible)
					return;

				ApplyBlockToRendererWithMotion(lastBlock);
			}
		}
		public void ApplyBlockToRendererWithMotion(StoryBlockBase block, bool fromCurrentUIData = false) {
			StopMotion();

			for (; block.blockType == StoryBlockType.Clip;) {
				StoryBlock_Clip clipBlock = block as StoryBlock_Clip;

				if (string.IsNullOrEmpty(clipBlock.targetClipGuid))
					return;
				if (!EditingStoryFile.Guid_To_ClipDict.ContainsKey(clipBlock.targetClipGuid))
					return;
				StoryClip nestedClip = EditingStoryFile.Guid_To_ClipDict[clipBlock.targetClipGuid];
				PushPreviewClipStack(nestedClip);

				block = nestedClip.BlockItemList.FirstOrDefault();

				if (block == null)
					return;
			}

			if (block.blockType == StoryBlockType.Item) {
				UISnapshot prevBlockSnapshot = null;
				if(!fromCurrentUIData) {
					prevBlockSnapshot = GetUISnapshotWithBlockApplied(EditingClip, GetSelectedBlockIndex() - 1);
				}


				foreach (OrderBase order in (block as StoryBlock_Item).OrderList) {
					if (order.orderType == OrderType.UI) {
						Order_UI order_UI = order as Order_UI;

						if (string.IsNullOrEmpty(order_UI.targetUIGuid))
							continue;
						if (!EditingUIFile.Guid_To_RendererDict.ContainsKey(order_UI.targetUIGuid))
							continue;

						UIRenderer renderer = EditingUIFile.Guid_To_RendererDict[order_UI.targetUIGuid] as UIRenderer;
						UIItemBase prevData;
						if (fromCurrentUIData) {
							prevData = renderer.RenderingData.Clone() as UIItemBase;
						} else {
							prevData = prevBlockSnapshot.GetUIItem(order_UI.targetUIGuid);
						}

						ApplyOrderToRendererWithMotion(prevData, renderer, order_UI);
					}
				}
			}
		}

		// TODO : 관련된 코드들 Order 내부로 이동
		// 파라미터들 Interface화 해서 클라이언트와 같이 쓸 것
		// Order
		private void ApplyOrderToRendererWithMotion(UIItemBase prevUIData, UIRenderer UIRenderer, Order_UI order_UI) {
			PlayingMotionSet.Add(new UIRendererMotion(UIRenderer, order_UI, prevUIData, GetUIDataWithOrdersApplied(prevUIData, order_UI)));
		}
		private void ApplyOrderToRenderer(UIRenderer UIRenderer, Order_UI order_UI) {
			UIRenderer.RenderFromData(order_UI.UIKeyData);
		}
		private void ApplyOrderToUIItem(UIItemBase UIItem, Order_UI order_UI) {
			UIItemBase keyData = order_UI.UIKeyData;

			foreach (FieldInfo keyFieldInfo in keyData.GetType().GetFields()) {
				if (keyFieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count() == 0)
					continue;

				if (!keyData.KeyFieldNameHashSet.Contains(keyFieldInfo.Name))
					continue;

				keyFieldInfo.SetValue(UIItem, keyFieldInfo.GetValue(keyData));
			}
		}

		// Manage preview clip
		private void PushPreviewClipStack(StoryClip clip) {
			PreviewClipStack.Push(new StoryClipState(clip, 0));
		}
		private void PopPreviewClipStack() {
			PreviewClipStack.Pop();
		}
		private void ClearPreviewClipStack() {
			PreviewClipStack.Clear();
		}

		// Utility
		public UISnapshot GetUISnapshotWithBlockApplied(StoryClip clip, int targetBlockIndex) {
			// Find last cache
			UISnapshot snapshot = null;
			int cacheIndex = -1;
			for (int blockI = targetBlockIndex - 1; blockI >= 0; --blockI) {
				StoryBlockBase block = clip.BlockItemList[blockI];

				if (block.HasUICache) {
					snapshot = block.UICacheSnapshot.Clone();
					cacheIndex = blockI;
				}
			}
			if (snapshot == null) {
				snapshot = EditingUIFile.UISnapshot.Clone();
			}

			// Apply orders to prev block
			for (int blockI = cacheIndex + 1; blockI <= targetBlockIndex; ++blockI) {
				StoryBlockBase block = clip.BlockItemList[blockI];
				if (!block.isVisible)
					continue;

				snapshot.ApplyStoryBlock(block);
			}

			return snapshot;
		}
		private UIItemBase GetUIDataWithOrdersApplied(UIItemBase UIItem, Order_UI order_UI) {
			UIItemBase newData = UIItem.Clone() as UIItemBase;
			ApplyOrderToUIItem(newData, order_UI);

			return newData;
		}
		private int GetSelectedBlockIndex() {
			if (StoryBlockTreeView.SelectedItemSet.Count == 1) {
				StoryBlockBase selectedBlockBase = (StoryBlockTreeView.SelectedItemSet.First as StoryBlockView).Data;
				return EditingClip.BlockItemList.IndexOf(selectedBlockBase);
			}
			return -1;
		}

		// Motion
		public class UIRendererMotion {
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

			public UIRendererMotion(UIRenderer UIRenderer, Order_UI order_UI, UIItemBase prevKeyData, UIItemBase currentKeyData) {
				this.UIRenderer = UIRenderer;
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
			foreach(UIRendererMotion motion in PlayingMotionSet) {
				if(!motion.IsComplete) {
					motion.AddTime(LoopEngine.DeltaSeconds);
				}
			}
		}
		private void StopMotion() {
			foreach (UIRendererMotion motion in PlayingMotionSet) {
				if (!motion.IsComplete) {
					motion.AddTime(motion.order_UI.TotalSec);
				}
			}

			PlayingMotionSet.Clear();
		}

	}
}
