using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GKit;
using GKit.Unity;

namespace TaleKit {
	public class Option {
		protected static TaleKitClient Client => TaleKitClient.Instance;

		//Graphic
		public int FPS {
			get {
				return Application.targetFrameRate;
			} set {
				Application.targetFrameRate = value;
			}
		}

		public Option() {
			Load();
		}
		private void Load() {
			SetDefault();
		}
		private void SetDefault() {
			FPS = 60;
		}
	}
}
