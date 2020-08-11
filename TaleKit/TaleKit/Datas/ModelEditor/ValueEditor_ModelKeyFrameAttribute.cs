using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.ModelEditor {
	public class ValueEditor_ModelKeyFrameAttribute : ValueEditorAttribute {
		public string connectedProperty;
		public string onConnectedPropertyUpdated;

		public ValueEditor_ModelKeyFrameAttribute(string updateWith = null, string onConnectedPropertyUpdated = null) : base(string.Empty) {
			this.connectedProperty = updateWith;
			this.onConnectedPropertyUpdated = onConnectedPropertyUpdated;
		}
	}
}
