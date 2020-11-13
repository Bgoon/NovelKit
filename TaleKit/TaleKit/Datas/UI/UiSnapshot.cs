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

namespace TaleKit.Datas.UI {
	public class UISnapshot {
		public UIItemBase rootUiItem;
		private Dictionary<string, UIItemBase> guid_To_UiItemDict;

		// [ Constructor ]
		public UISnapshot() {
			guid_To_UiItemDict = new Dictionary<string, UIItemBase>();
		}

		// [ Data Managing ]
		public void Clear() {
			rootUiItem = null;
			guid_To_UiItemDict.Clear();
		}
		public UISnapshot Clone() {
			UISnapshot cloneSnapshot = new UISnapshot();

			foreach (var srcUiItemPair in guid_To_UiItemDict) {
				UIItemBase srcItem = srcUiItemPair.Value;
				UIItemBase cloneItem = srcItem.Clone() as UIItemBase;

				if (srcItem.ParentItem == null) {
					cloneSnapshot.rootUiItem = cloneItem;
				}
				cloneSnapshot.guid_To_UiItemDict.Add(cloneItem.guid, cloneItem);
			}
			foreach (var cloneUiItemPair in cloneSnapshot.guid_To_UiItemDict) {
				UIItemBase cloneItem = cloneUiItemPair.Value;

				if (cloneItem.ParentItem != null) {
					cloneItem.ParentItem = guid_To_UiItemDict[cloneItem.ParentItem.guid];
				}
				foreach (var cloneUiItemChild in cloneItem.ChildItemList.ToArray()) {
					cloneItem.ChildItemList.Remove(cloneUiItemChild);
					cloneItem.ChildItemList.Add(guid_To_UiItemDict[cloneUiItemChild.guid]);
				}
			}

			return cloneSnapshot;
		}
		public void CopyDataFrom(UISnapshot srcSnapshot) {
			foreach(var srcUiItemPair in srcSnapshot.guid_To_UiItemDict) {
				UIItemBase srcItem = srcUiItemPair.Value;
				UIItemBase dstItem = guid_To_UiItemDict[srcItem.guid];

				// Class 제외하고 Struct만 복사
				FieldInfo[] fieldInfos = srcItem.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach(FieldInfo fieldInfo in fieldInfos) {
					if(fieldInfo.FieldType.IsClass)
						continue;

					fieldInfo.SetValue(dstItem, fieldInfo.GetValue(srcItem));
				}
			}
		}

		// Control Collection
		public UIItemBase[] GetUiItems() {
			return guid_To_UiItemDict.Values.ToArray();
		}

		public void RegisterUiItem(string guid, UIItemBase UiItem) {
			guid_To_UiItemDict.Add(guid, UiItem);
		}
		public void RemoveUiItem(string guid) {
			guid_To_UiItemDict.Remove(guid);
		}
		public UIItemBase GetUiItem(string guid) {
			return guid_To_UiItemDict[guid];
		}

		public void ApplyStoryBlockBase(StoryBlockBase storyBlockBase) {
			if(storyBlockBase is StoryBlock_Item) {
				StoryBlock_Item storyBlock = storyBlockBase as StoryBlock_Item;
				foreach (OrderBase order in storyBlock.OrderList) {
					Order_UI UiOrder = order as Order_UI;

					if (UiOrder == null || string.IsNullOrEmpty(UiOrder.targetUiGuid) || !guid_To_UiItemDict.ContainsKey(UiOrder.targetUiGuid))
						continue;

					UIItemBase targetUiItem = guid_To_UiItemDict[UiOrder.targetUiGuid];

					foreach(FieldInfo fieldInfo in targetUiItem.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
						if(UiOrder.UiKeyData.KeyFieldNameHashSet.Contains(fieldInfo.Name)) {
							fieldInfo.SetValue(targetUiItem, fieldInfo.GetValue(UiOrder.UiKeyData));
						}
					}
				}
			} else {
				// TODO : StoryClip 적용 구현
			}
		}

		public JObject ToJObject() {
			return rootUiItem.ToJObject();
		}
	}
}
