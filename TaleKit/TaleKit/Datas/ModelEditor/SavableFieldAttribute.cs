using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas.ModelEditor {
	[AttributeUsage(AttributeTargets.Field)]
	public class SavableFieldAttribute : Attribute {
	}
}
