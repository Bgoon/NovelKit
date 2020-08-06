using System;

namespace TaleKit.Datas.Editor {
	public interface IEditableModel {
		event Action ModelUpdated;

		void UpdateModel();
	}
}
