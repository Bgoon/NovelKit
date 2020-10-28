using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.UI;
using GKitForUnity;

namespace TaleKit.Datas.Story {
	public class UiCacheManager {
		public readonly TaleData OwnerData;
		private StoryFile StoryFile => OwnerData.StoryFile;
		private UiFile UiFile => OwnerData.UiFile;

		public int CacheInterval {
			get; private set;
		} = 3;

		public StoryClip TargetClipAuto {
			get {
				StoryClip targetClip;
				if (TargetClip == null) {
					targetClip = OwnerData.StoryFile.RootClip;
				} else {
					targetClip = TargetClip;
				}
				return targetClip;
			}
		}
		public StoryClip TargetClip {
			get; private set;
		}

		// [ Constructor ]
		public UiCacheManager(TaleData ownerData) {
			this.OwnerData = ownerData;
		}

		// [ Loop ]
		internal void OnTick() {
			CreateCache(1);
		}

		// [ Control ]
		public void SetActivatedClip(StoryClip clip) {
			TargetClip = clip;
		}

		public void CreateCache(int cacheCount) {
			UiSnapshot cacheSnapshot = null;
			UiSnapshot lastBlockCache = null;

			StoryClip targetClip = TargetClipAuto;

			for (int blockI = 0; blockI < targetClip.ChildItemList.Count; ++blockI) {
				StoryBlockBase block = targetClip.ChildItemList[blockI];

				// Skip block exists cache
				if(block.HasUiCache) {
					lastBlockCache = block.UiCacheSnapshot;
					continue;
				}

				if(cacheSnapshot == null) {
					if(lastBlockCache == null) {
						cacheSnapshot = UiFile.UiSnapshot.Clone();
					} else {
						cacheSnapshot = lastBlockCache.Clone();
					}
				}

				// Apply orders
				cacheSnapshot.ApplyStoryBlockBase(block);

				// Save cache
				if (blockI % CacheInterval == 0) {
					block.SaveCache(cacheSnapshot.Clone());

					Console.WriteLine("CacheSaved");

					if(--cacheCount <= 0) {
						return;
					}
				}
			}
		}
		public void ClearCacheAfterBlock(StoryBlockBase targetBlock) {
			StoryClip targetClip = TargetClipAuto;
			int index = TargetClipAuto.ChildItemList.IndexOf(targetBlock);
			if (index < 0)
				return;

			Console.WriteLine($"CacheCleared index : {index}");
			for (int i = index; i < targetClip.ChildItemList.Count; ++i) {
				StoryBlockBase block = targetClip.ChildItemList[i];

				block.DeleteCache();
			}
		}
	}
}