using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GKit;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI.UIItem;
using TaleKit.Datas.ModelEditor;

namespace TaleKit.Datas.UI {
	public class UISnapshot {
		public readonly TaleData OwnerTaleData;

		public UIItemBase rootUiItem;
		private Dictionary<string, UIItemBase> guid_To_UIItemDict;

		// [ Constructor ]
		public UISnapshot(TaleData ownerTaleData) {
			this.OwnerTaleData = ownerTaleData;

			guid_To_UIItemDict = new Dictionary<string, UIItemBase>();
		}

		// [ Data Managing ]
		public void Clear() {
			rootUiItem = null;
			guid_To_UIItemDict.Clear();
		}
		public UISnapshot Clone() {
			UISnapshot cloneSnapshot = new UISnapshot(OwnerTaleData);

			foreach (var srcUIItemPair in guid_To_UIItemDict) {
				UIItemBase srcItem = srcUIItemPair.Value;
				UIItemBase cloneItem = srcItem.Clone() as UIItemBase;

				if (cloneItem.ParentItem == null) {
					cloneSnapshot.rootUiItem = cloneItem;
				}
				cloneSnapshot.guid_To_UIItemDict.Add(cloneItem.guid, cloneItem);
			}
			foreach (var cloneUIItemPair in cloneSnapshot.guid_To_UIItemDict) {
				UIItemBase cloneItem = cloneUIItemPair.Value;

				if (cloneItem.ParentItem != null) {
					cloneItem.ParentItem = guid_To_UIItemDict[cloneItem.ParentItem.guid];
				}
				UIItemBase[] childItems = cloneItem.ChildItemList.ToArray();
				cloneItem.InitializeClone();
				foreach (var srcUIItemChild in childItems) {
					cloneItem.ChildItemList.Add(guid_To_UIItemDict[srcUIItemChild.guid]);
				}
			}

			return cloneSnapshot;
		}
		public void CopyDataFrom(UISnapshot srcSnapshot) {
			foreach(var srcUiItemPair in srcSnapshot.guid_To_UIItemDict) {
				UIItemBase srcItem = srcUiItemPair.Value;
				UIItemBase dstItem = guid_To_UIItemDict[srcItem.guid];

				// Class 제외하고 Struct만 복사
				FieldInfo[] fieldInfos = srcItem.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach(FieldInfo fieldInfo in fieldInfos) {
					if(fieldInfo.FieldType.IsClass)
						continue;
					if (fieldInfo.GetCustomAttributes<SavableFieldAttribute>() == null)
						continue;

					fieldInfo.SetValue(dstItem, fieldInfo.GetValue(srcItem));
				}
			}
		}

		// Control Collection
		public UIItemBase[] GetUiItems() {
			return guid_To_UIItemDict.Values.ToArray();
		}

		public void RegisterUiItem(string guid, UIItemBase UiItem) {
			guid_To_UIItemDict.Add(guid, UiItem);
		}
		public void RemoveUiItem(string guid) {
			guid_To_UIItemDict.Remove(guid);
		}
		public UIItemBase GetUiItem(string guid) {
			return guid_To_UIItemDict[guid];
		}

		public void ApplyStoryBlock(StoryBlockBase storyBlockBase) {
			if(storyBlockBase.blockType == StoryBlockType.Item) {
				StoryBlock_Item itemBlock = storyBlockBase as StoryBlock_Item;
				foreach (OrderBase order in itemBlock.OrderList) {
					Order_UI UIOrder = order as Order_UI;

					if (UIOrder == null || string.IsNullOrEmpty(UIOrder.targetUIGuid) || !guid_To_UIItemDict.ContainsKey(UIOrder.targetUIGuid))
						continue;

					UIItemBase targetUiItem = guid_To_UIItemDict[UIOrder.targetUIGuid];

					foreach(FieldInfo fieldInfo in targetUiItem.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
						if(UIOrder.UIKeyData.KeyFieldNameHashSet.Contains(fieldInfo.Name)) {
							fieldInfo.SetValue(targetUiItem, fieldInfo.GetValue(UIOrder.UIKeyData));
						}
					}
				}
			} else if(storyBlockBase.blockType == StoryBlockType.Clip) {
				StoryBlock_Clip clipBlock = storyBlockBase as StoryBlock_Clip;
				if (string.IsNullOrEmpty(clipBlock.targetClipGuid))
					return;
				if (!OwnerTaleData.StoryFile.Guid_To_ClipDict.ContainsKey(clipBlock.targetClipGuid))
					return;

				StoryClip clip = OwnerTaleData.StoryFile.Guid_To_ClipDict[clipBlock.targetClipGuid];

				ApplyStoryClip(clip);
			}
		}
		public void ApplyStoryClip(StoryClip clip) {
			foreach(StoryBlockBase block in clip.BlockItemList) {
				ApplyStoryBlock(block);
			}
		}

		public JObject ToJObject() {
			return rootUiItem.ToJObject();
		}
	}
}
