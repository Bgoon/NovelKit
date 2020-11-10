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
			int lastCacheBlockIndex = 0;

			StoryClip targetClip = TargetClipAuto;


			// Find last cache
			for(int blockI = targetClip.BlockItemList.Count-1; blockI>=0; --blockI) {
				StoryBlockBase block = targetClip.BlockItemList[blockI];

				if (block.HasUiCache) {
					cacheSnapshot = block.UiCacheSnapshot.Clone();
					lastCacheBlockIndex = blockI;
					return;
				}
			}
			if(cacheSnapshot == null) {
				cacheSnapshot = UiFile.UiSnapshot.Clone();
			}

			// Simulate blocks and save cache
			for (int blockI = lastCacheBlockIndex; blockI < targetClip.BlockItemList.Count; ++blockI) {
				StoryBlockBase block = targetClip.BlockItemList[blockI];

				// Apply orders
				if(block.isVisible) {
					cacheSnapshot.ApplyStoryBlockBase(block);
				}

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
		public void ClearCacheAll() {
			StoryClip targetClip = TargetClipAuto;
			for (int i = 0; i < targetClip.BlockItemList.Count; ++i) {
				StoryBlockBase block = targetClip.BlockItemList[i];

				block.DeleteCache();
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

			Console.WriteLine($"CacheCleared index : {targetBlockIndex}");
			for (int i = targetBlockIndex; i < targetClip.BlockItemList.Count; ++i) {
				StoryBlockBase block = targetClip.BlockItemList[i];

				block.DeleteCache();
			}
		}
	}
}