using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.Asset {
	[AttributeUsage(AttributeTargets.Field)]
	public class AssetMetaAttribute : Attribute {

		public AssetMetaAttribute() {

		}
	}
}
