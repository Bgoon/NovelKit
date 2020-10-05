using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.UI;

namespace TaleKit.Datas.Story {
	public class UiCacheManager {
		public readonly TaleData OwnerData;
		private StoryFile StoryFile => OwnerData.StoryFile;
		private UiFile UiFile => OwnerData.UiFile;

		public int CacheInterval {
			get; private set;
		} = 10;

		public StoryClip ActivatedClip {
			get; private set;
		}

		// [ Constructor ]
		public UiCacheManager(TaleData ownerData) {
			this.OwnerData = ownerData;
		}

		// [ State ]
		public void SetActivatedClip(StoryClip clip) {
			ActivatedClip = clip;
		}

		// [ Control ]
		public void CreateCache() {
			if (ActivatedClip == null)
				return;

			UiSnapshot cacheSnapshot = UiFile.UiSnapshot.Clone();	

			for(int blockI = 0; blockI < ActivatedClip.ChildItemList.Count; ++blockI) {
				StoryBlockBase block = ActivatedClip.ChildItemList[blockI];

				// Apply orders
				if(block.Type == StoryBlockType.StoryBlock) {

					cacheSnapshot.ApplyStoryBlock(block as StoryBlock);

				} else if(block.Type == StoryBlockType.StoryClip) {
					// TODO : StoryClip 적용 구현하기
				}

				// Save cache
				if(blockI % CacheInterval == 0) {
					block.SaveCache(cacheSnapshot.Clone());
				}
			}
		}
	}
}


//
for (int i = 0; i <= lastBlockIndex; ++i) {
				StoryBlockBase blockBase = EditingStoryFile.RootClip.ChildItemList[i];
				switch (blockBase.Type) {
					case StoryBlockType.StoryBlock:
						foreach (OrderBase order in (blockBase as StoryBlock).OrderList) {
							if (order.OrderType == OrderType.UI) {
								Order_UI order_UI = order as Order_UI;

								if (string.IsNullOrEmpty(order_UI.targetUiGuid))
									continue;

								UiItemBase UiItem = EditingUiFile.UiSnapshot.GetUiItem(order_UI.targetUiGuid);

								if (UiItem == null)
									continue;

								UiRenderer renderer = EditingUiFile.Guid_To_RendererDict[UiItem.guid] as UiRenderer;

								if(!RenderedRendererHashSet.Contains(renderer)) {
									RenderedRendererHashSet.Add(renderer);
									renderer.Render();
								}


								if(playMotion && i == lastBlockIndex) {
									StartMotion(UiItem, renderer, order_UI);
								} else {
									ApplyOrderToRendererImmediately(renderer, order_UI);
								}

							}
						}
						break;
					case StoryBlockType.StoryClip:
						break;
				}
			}