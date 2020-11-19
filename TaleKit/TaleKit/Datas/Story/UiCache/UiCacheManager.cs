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
		public readonly StoryFile OwnerFile;
		public TaleData OwnerTaleData => OwnerFile.OwnerTaleData;
		public UIFile UIFile => OwnerTaleData.UiFile;

		public int CacheInterval {
			get; private set;
		} = 3;

		public StoryClip TargetClipAuto {
			get {
				StoryClip targetClip;
				if (TargetClip == null) {
					targetClip = OwnerFile.RootClip;
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
		public UiCacheManager(StoryFile ownerFile) {
			this.OwnerFile = ownerFile;
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
			UISnapshot cacheSnapshot = null;
			int lastCacheBlockIndex = -1;

			StoryClip targetClip = TargetClipAuto;

			// Find last cache
			for(int blockI = targetClip.BlockItemList.Count-1; blockI>=0; --blockI) {
				StoryBlockBase block = targetClip.BlockItemList[blockI];

				if (block.HasUICache) {
					cacheSnapshot = block.UICacheSnapshot.Clone();
					lastCacheBlockIndex = blockI;
					return;
				}
			}

			if(cacheSnapshot == null) {
				cacheSnapshot = UIFile.UISnapshot.Clone();
			}

			// Simulate blocks and save cache
			for (int blockI = lastCacheBlockIndex+1; blockI < targetClip.BlockItemList.Count; ++blockI) {
				StoryBlockBase block = targetClip.BlockItemList[blockI];

				// Apply orders
				if(block.isVisible) {
					cacheSnapshot.ApplyStoryBlock(block);
				}

				// Save cache
				if (blockI % CacheInterval == 0) {
					block.SaveCache(cacheSnapshot.Clone());

					if(--cacheCount <= 0) {
						return;
					}
				}
			}
		}
		public void ClearCacheAll() {
			StoryClip targetClip = TargetClipAuto;

			if (targetClip == null)
				return;

			for (int i = 0; i < targetClip.BlockItemList.Count; ++i) {
				StoryBlockBase block = targetClip.BlockItemList[i];

				block.ClearCache();
			}
		}
		public void ClearCacheAfterBlock(StoryBlockBase targetBlock) {
			StoryClip targetClip = TargetClipAuto;
			int index = targetClip.BlockItemList.IndexOf(targetBlock);
			if (index < 0)
				return;

			ClearCacheAfterBlock(index);
		}
		public void ClearCacheAfterBlock(int targetBlockIndex) {
			StoryClip targetClip = TargetClipAuto;

			for (int i = targetBlockIndex; i < targetClip.BlockItemList.Count; ++i) {
				StoryBlockBase block = targetClip.BlockItemList[i];

				block.ClearCache();
			}
		}
	}
}