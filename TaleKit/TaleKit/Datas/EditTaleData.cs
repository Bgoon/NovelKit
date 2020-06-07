using GKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas;

namespace TaleKit.Datas {
	public class EditTaleData : TaleData {

		//EditorData
		public string ProjectDir {
			get; private set;
		}
		public string AssetDir => Path.Combine(ProjectDir, "Assets");
		public string AssetMetaDir => Path.Combine(ProjectDir, ".AssetMeta");
		public bool IsSaved {
			get; private set;
		}

		public EditTaleData(string projectDir) : base() {
			this.ProjectDir = IOUtility.NormalizePath(projectDir);

			CreateEditorDirectories();
		}


		private void CreateEditorDirectories() {
			Directory.CreateDirectory(AssetDir);
			Directory.CreateDirectory(AssetMetaDir);
		}
	}
}
