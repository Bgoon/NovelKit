using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GKit;
using GKit.Unity;
using GKit.Data;

namespace TaleKit.Datas.UI {
	public class UiItem {

		public event NodeItemInsertedDelegate<UiItem> ChildInserted;

		public event ChildRemovedDelegate<UiItem> ChildRemoved;

		public readonly UiFile OwnerFile;
		public UiItem ParentItem {
			get; private set;
		}
		public List<UiItem> ChildItemList {
			get; private set;
		}

		//Datas
		public readonly GameObject GameObject;
		public readonly RectTransform RectTransform;
		public readonly UiTransform UiTransform;
		public readonly CanvasRenderer Renderer;
		public float Alpha {
			get {
				return Renderer.GetAlpha();
			} set {
				Renderer.SetAlpha(value);
			}
		}

		public UiItem() {
			ChildItemList = new List<UiItem>();

			ParentItem = new UiItem();

			GameObject = new GameObject();
			RectTransform = GameObject.AddComponent<RectTransform>();
			UiTransform = GameObject.AddComponent<UiTransform>();
			Renderer = GameObject.AddComponent<CanvasRenderer>();
		}

		public void AddChildItem(UiItem item) {
			InsertChildItem(ChildItemList.Count, item);
		}
		public void InsertChildItem(int index, UiItem item) {
			ChildItemList.Insert(index, item);

			ChildInserted?.Invoke(index, item);
		}
		public void RemoveChildItem(UiItem item) {
			ChildItemList.Remove(item);

			ChildRemoved?.Invoke(item);
		}
	}
}
