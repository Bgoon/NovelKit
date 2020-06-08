using GKit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleKit.Datas.Asset;
using TaleKit.Datas.Motion;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI;

namespace TaleKit.Datas {
	public class TaleData {
		public const string Version = "2019.9";
		public const string FileExt = ".taledata";

		public AssetManager AssetManager {
			get; private set;
		}

		public MotionFile MotionFile {
			get; private set;
		}
		public UiFile UiFile {
			get; private set;
		}
		public StoryFile StoryFile {
			get; private set;
		}

		public string projectName;
		public DateTime exportedTime;

		//EditorData
		public string ProjectDir {
			get; private set;
		}
		public string AssetDir => Path.Combine(ProjectDir, "Assets");
		public string AssetMetaDir => Path.Combine(ProjectDir, ".AssetMeta");
		public bool IsSaved {
			get; private set;
		}
		
		public readonly bool IsEditMode;

		public TaleData(string projectDir, bool isEditMode) {
			this.ProjectDir = IOUtility.NormalizePath(projectDir);
			this.IsEditMode = isEditMode;

			AssetManager = new AssetManager(this);

			MotionFile = new MotionFile();
			UiFile = new UiFile();
			StoryFile = new StoryFile();

			CreateEditorDirectories();

			AssetManager.ReloadAssets();

			if(isEditMode) {
				AssetManager.StartWatchAssetDir();
			}
		}
		public void PostInit() {
		}

		/// <summary>
		/// 프로젝트 경로에 데이터들을 저장합니다.
		/// </summary>
		/// <param name="directoryName"></param>
		public void Save(string directoryName) {

		}

		/// <summary>
		/// 클라이언트에서 플레이할 수 있는 데이터 파일로 내보냅니다.
		/// </summary>
		public void Export(string fileName) {
			JObject jFile = new JObject();

			exportedTime = DateTime.Now;

			//File info
			jFile.Add(nameof(exportedTime), exportedTime.ToOADate());

			jFile.Add("MotionFile", MotionFile.ToJObject());
			jFile.Add("UiFile", UiFile.ToJObject());
			jFile.Add("StoryFile", StoryFile.ToJObject());

			File.WriteAllText(fileName, jFile.ToString(), Encoding.UTF8);
		}

		private void CreateEditorDirectories() {
			Directory.CreateDirectory(AssetDir);
			Directory.CreateDirectory(AssetMetaDir);
		}
	}
}
