using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.UI;
using GKit;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;
using TaleKit.Datas.Editor;

namespace TaleKit.Datas.Story {
	/// <summary>
	/// StoryBlock에 붙는 컴포넌트이다. 편집기를 통해서 데이터를 수정할 수 있다.
	/// 이것을 상속받는 클래스는 생성자에서 부모만을 정해줘야 한다.
	/// </summary>
	public abstract class OrderBase : IEditableModel, IComparer {
		protected static TaleKitClient Client => TaleKitClient.Instance;
		protected static UiManager UiManager => Client.UiManager;
		protected static GLoopEngine LoopEngine => Client.LoopEngine;
		

		public StoryBlock OwnerBlock {
			get; private set;
		}

		public bool IsComplete {
			get; protected set;
		}

		public abstract OrderType OrderType {
			get;
		}

		public OrderBase(StoryBlock ownerBlock) {
			this.OwnerBlock = ownerBlock;
		}

		public virtual void OnStart() {

		}
		public virtual void OnTick() {

		}
		public virtual void OnComplete() {

		}

		public virtual void Skip() {

		}

		public virtual JObject ToJObject() {
			JObject jOrder = new JObject();
			jOrder.Add("Type", OrderType.ToString());

			JObject jAttributes = new JObject();
			jOrder.Add("Attributes", jAttributes);

			Type modelType = GetType();
			FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (FieldInfo field in fields) {
				ValueEditorAttribute editorAttribute = field.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;
				
				if (editorAttribute == null)
					continue;

				jAttributes.Add(field.Name, field.GetValue(this).ToString());
			}

			return jOrder;
		}

		public int Compare(object x, object y) {
			return x.GetHashCode();
		}
	}
}
