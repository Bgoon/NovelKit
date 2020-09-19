using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GKit.Json;
using Newtonsoft.Json.Linq;
using TaleKit.Datas.ModelEditor;

namespace TaleKit.Datas {
	public class ProjectSetting : EditableModel {

		public readonly TaleData OwnerData;

		[ValueEditor_NumberBox("FPS", GKitForUnity.NumberType.Int)]
		public int defaultFps = 60;
		[ValueEditor_Vector2("Resolution", GKitForUnity.NumberType.Int, 0, 20000)]
		public Vector2 resolution = new Vector2(1920, 1080);

		public ProjectSetting(TaleData ownerData) {
			this.OwnerData = ownerData;

		}

		public JObject ToJObject() {
			JObject jFile = new JObject();

			jFile.AddAttrFields<ValueEditorAttribute>(this);

			return jFile;
		}
	}
}
