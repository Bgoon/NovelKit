using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleKit.Datas {
	public interface ITaleDataFile {
		bool Save(string filename);
		bool Load(string filename);
	}
}
