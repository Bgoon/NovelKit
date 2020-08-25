﻿using System;
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
using TaleKitEditor.UI.Workspaces.StoryWorkspaceTabs.StoryBoardElements;
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

		private Dictionary<StoryBlockBase, StoryBlockItemView> dataToViewDict;
		public StoryBlockItemView SelectedBlockViewSingle {
			get {
				if (StoryBlockTreeView.SelectedItemSet.Count > 0) {
					return (StoryBlockItemView)StoryBlockTreeView.SelectedItemSet.Last();
				}
				return null;
			}
		}
		public StoryBlock SelectedBlockSingle {
			get {
				StoryBlockItemView selectedItemView = SelectedBlockViewSingle;
				return selectedItemView == null ? null : selectedItemView.Data as StoryBlock;
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
			dataToViewDict = new Dictionary<StoryBlockBase, StoryBlockItemView>();
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
			EditingStoryFile.ItemCreated += StoryFile_ItemCreated;
			EditingStoryFile.ItemRemoved += StoryFile_ItemRemoved;

			EditingClip = EditingStoryFile.RootClip;
		}
		private void MainWindow_DataUnloaded(TaleData obj) {
			EditingStoryFile.ItemCreated -= StoryFile_ItemCreated;
			EditingStoryFile.ItemRemoved -= StoryFile_ItemRemoved;
		}

		private void StoryBlockListController_CreateItemButtonClick() {
			EditingStoryFile.CreateStoryBlockItem(EditingClip);
		}
		private void StoryBlockListController_RemoveItemButtonClick() {
			foreach (StoryBlockItemView itemView in StoryBlockTreeView.SelectedItemSet) {
				StoryBlockBase data = itemView.Data;

				EditingStoryFile.RemoveStoryBlockItem(data);
			}
		}

		private void StoryFile_ItemCreated(StoryBlockBase item, StoryClip parentItem) {
			if (parentItem == null)
				return;
			if (parentItem != EditingClip)
				return;

			//Create view
			StoryBlockItemView itemView = new StoryBlockItemView(item);
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
			StoryBlockBase item = ((StoryBlockItemView)itemView).Data;

			EditingClip.RemoveChildItem(item);
			EditingClip.InsertChildItem(index, item);
		}

		private void SelectedItemSet_SelectionAdded(ISelectable item) {
			OnStoryBlockSelectionChanged();
		}
		private void SelectedItemSet_SelectionRemoved(ISelectable item) {
			OnStoryBlockSelectionChanged();
		}
		private void OnStoryBlockSelectionChanged() {
			ApplyOrderToSelection(true);
		}

		// [ Control ]
		// Apply order
		public void ApplyOrderToSelection(bool playMotion = false) {
			if(StoryBlockTreeView.SelectedItemSet.Count == 1) {
				StoryBlockBase selectedBlockBase = (StoryBlockTreeView.SelectedItemSet.First as StoryBlockItemView).Data;
				int selectedBlockIndex = EditingStoryFile.RootClip.ChildItemList.IndexOf(selectedBlockBase);

				ApplyOrders(selectedBlockIndex, playMotion);
			} else {
				ApplyOrders(-1, false);
			}
		}
		public void ApplyOrders(int lastIndex, bool playMotion = false) {
			StopMotion();

			// Render root renderer
			UiRenderer rootRenderer = EditingUiFile.Item_To_ViewDict[EditingUiFile.RootUiItem] as UiRenderer;
			rootRenderer.Render(true);

			if (!ViewportTab.PlayStateButton.IsActive)
				return;

			RenderedRendererHashSet.Clear();

			for (int i = 0; i <= lastIndex; ++i) {
				StoryBlockBase blockBase = EditingStoryFile.RootClip.ChildItemList[i];
				switch (blockBase.Type) {
					case StoryBlockType.StoryBlock:
						foreach (OrderBase order in (blockBase as StoryBlock).OrderList) {
							if (order.OrderType == OrderType.UI) {
								Order_UI order_UI = order as Order_UI;
								UiItemBase UiItem = EditingUiFile.Guid_To_ItemDict[order_UI.targetUiGuid];

								if (UiItem == null)
									continue;

								UiRenderer renderer = EditingUiFile.Item_To_ViewDict[UiItem] as UiRenderer;

								if(!RenderedRendererHashSet.Contains(renderer)) {
									RenderedRendererHashSet.Add(renderer);
									renderer.Render();
								}


								if(playMotion && i == lastIndex) {
									ApplyOrder(UiItem, renderer, order_UI);
								} else {
									ApplyOrderImmediately(renderer, order_UI);
								}

							}
						}
						break;
					case StoryBlockType.StoryClip:
						break;
				}
			}
		}

		// TODO : 관련된 코드들 Order 내부로 이동
		private void ApplyOrder(UiItemBase UiItem, UiRenderer UiRenderer, Order_UI order_UI) {
			
			// TODO : Test usage
			UiMotionSet.Add(new UiMotion(UiRenderer, order_UI, UiItem, GetOrderAppliedData(UiItem, order_UI)));
		}
		private void ApplyOrderImmediately(UiRenderer UiRenderer, Order_UI order_UI) {
			UiRenderer.RenderFromData(order_UI.UiKeyData);
		}

		private UiItemBase GetOrderAppliedData(UiItemBase UiItem, Order_UI order_UI) {
			UiItemBase newData = UiItem.Clone() as UiItemBase;
			UiItemBase keyData = order_UI.UiKeyData;

			foreach(FieldInfo keyFieldInfo in keyData.GetType().GetFields()) {
				if (keyFieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count() == 0)
					continue;

				if (!keyData.KeyFieldNameHashSet.Contains(keyFieldInfo.Name))
					continue;

				keyFieldInfo.SetValue(newData, keyFieldInfo.GetValue(keyData));
			}

			return newData;
		}

		// Motion
		public class UiMotion {
			private readonly static BindingFlags PublicRuntimeBindingFlags = BindingFlags.Public | BindingFlags.Instance;

			public readonly UiRenderer UiRenderer;
			public readonly Order_UI order_UI;

			public readonly UiItemBase prevKeyData;
			public readonly UiItemBase currentKeyData;

			public float ActualTimeSec => timeSec - order_UI.delaySec;
			public float timeSec;

			public UiMotion(UiRenderer UiRenderer, Order_UI order_UI, UiItemBase prevKeyData, UiItemBase currentKeyData) {
				this.UiRenderer = UiRenderer;
				this.order_UI = order_UI;

				this.prevKeyData = prevKeyData;
				this.currentKeyData = currentKeyData;
			}

			public void AddTime(float deltaSec) {
				this.timeSec += deltaSec;

				Render();
			}
			private void Render() {
				UiItemBase breakDownData = GetBreakDownData();

				UiRenderer.RenderFromData(breakDownData);
			}
			private UiItemBase GetBreakDownData() {
				UiItemBase breakDownData = prevKeyData.Clone() as UiItemBase;

				if (timeSec > order_UI.delaySec) {
					float normalTime = Mathf.Clamp01(ActualTimeSec / order_UI.durationSec);

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
				motion.AddTime(LoopEngine.DeltaSeconds);
				Debug.WriteLine(motion.timeSec);
			}
		}
		private void StopMotion() {
			UiMotionSet.Clear();
		}

	}
}
