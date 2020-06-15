using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
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

			AssetManager.ReloadMetas();

			if (isEditMode) {
				AssetManager.StartWatchAssetDir();
			}
		}
		public void PostInit() {
		}

		/// <summary>
		/// 프로젝트 경로에 데이터들을 저장합니다.
		/// </summary>
		/// <param name="directory"></param>
		public void Save() {
			if (string.IsNullOrEmpty(ProjectDir))
				throw new Exception("ProjectDir이 지정되지 않았습니다.");

			Directory.CreateDirectory(ProjectDir);

			//Asset
			AssetManager.SaveMetas();

			//
		}

		/// <summary>
		/// 클라이언트에서 플레이할 수 있는 데이터 파일로 내보냅니다.
		/// </summary>
		public void Export(string filename) {
			string dataString = GetDataString();

			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			File.WriteAllText(filename, dataString, Encoding.UTF8);
		}

		public void SetProjectDir(string directory) {
			ProjectDir = directory;

			CreateEditorDirectories();
		}

		private void CreateEditorDirectories() {
			Directory.CreateDirectory(AssetDir);
			Directory.CreateDirectory(AssetMetaDir);
		}

		public string GetDataString() {
			JObject jFile = new JObject();

			exportedTime = DateTime.Now;

			//File info
			jFile.Add(nameof(exportedTime), exportedTime.ToOADate());

			jFile.Add("MotionFile", MotionFile.ToJObject());
			jFile.Add("UiFile", UiFile.ToJObject());
			jFile.Add("StoryFile", StoryFile.ToJObject());

			return jFile.ToString();
		}
	}
}
