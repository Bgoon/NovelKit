using GKitForUnity;
using System;

namespace TaleKit.Datas.ModelEditor {
	public delegate void ModelUpdatedDelegate(string fieldName);

	public class EditableModel {
		public event ModelUpdatedDelegate ModelUpdated;

		public virtual void NotifyModelUpdated(string fieldName) {
			ModelUpdated?.Invoke(fieldName);
		}
	}
}
