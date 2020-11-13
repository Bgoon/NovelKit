using GKitForUnity.IO;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using TaleKit.Datas.Asset;
using TaleKit.Datas.Motion;
using TaleKit.Datas.Story;
using TaleKit.Datas.UI;
using Ionic.Zip;

namespace TaleKit.Datas {
	public class TaleData : IDisposable {
		public const string Version = "2019.9";
		public const string FileExt = ".taledata";

		public AssetManager AssetManager {
			get; private set;
		}

		public MotionFile MotionFile {
			get; private set;
		}
		public UIFile UiFile {
			get; private set;
		}
		public StoryFile StoryFile {
			get; private set;
		}
		public ProjectSetting ProjectSetting {
			get; private set;
		}

		public string projectName;
		public DateTime exportedTime;

		public event Action Tick;

		// EditorData
		public string ProjectDir {
			get; private set;
		}
		public string AssetDir => Path.Combine(ProjectDir, "Assets");
		public string AssetMetaDir => Path.Combine(ProjectDir, ".AssetMeta");
		public string DataFilename => Path.Combine(ProjectDir, "Data.json");

		public bool IsSaved {
			get; private set;
		}

		public readonly bool IsEditMode;

		public static TaleData FromTaleFile(string filename, TaleDataInitArgs initArgs) {
			string projectDir = Path.Combine(Path.GetDirectoryName(filename), "TaleData");
			Directory.CreateDirectory(projectDir);

			ReadOptions option = new ReadOptions() {
				Encoding = Encoding.UTF8,
			};
			using (ZipFile zip = ZipFile.Read(filename, option)) {
				zip.ExtractAll(projectDir);
			}

			return FromProjectDir(projectDir, initArgs);
		}
		public static TaleData FromProjectDir(string projectDir, TaleDataInitArgs initArgs) {
			initArgs.createRootUIItem = false;
			initArgs.isEditMode = true;

			TaleData taleData = new TaleData(projectDir, initArgs);

			initArgs.projectLoadedTask?.Invoke(taleData);

			JObject jDataRoot = JObject.Parse(File.ReadAllText(taleData.DataFilename, Encoding.UTF8));

			taleData.ProjectSetting.LoadFromJson((JObject)jDataRoot["ProjectSetting"]);
			taleData.UiFile.LoadFromJson((JObject)jDataRoot["UiFile"]);
			taleData.MotionFile.LoadFromJson((JObject)jDataRoot["MotionFile"]);
			taleData.StoryFile.LoadFromJson((JObject)jDataRoot["StoryFile"]);

			return taleData;
		}

		// [ Constructor ]
		public TaleData(string projectDir, TaleDataInitArgs initArgs) {
			this.ProjectDir = IOUtility.NormalizePath(projectDir);
			this.IsEditMode = initArgs.isEditMode;

			AssetManager = new AssetManager(this);

			MotionFile = new MotionFile(this, initArgs.targetMotionData);
			UiFile = new UIFile(this, initArgs.createRootUIItem);
			StoryFile = new StoryFile(this);
			ProjectSetting = new ProjectSetting(this);

			CreateEditorDirectories();

			AssetManager.ReloadMetas();

			if (initArgs.isEditMode) {
				AssetManager.StartWatchAssetDir();
			}
		}
		public void Dispose() {
			UiFile.Clear();
			
		}
		public void PostInit() {
			UiFile.Init();
		}

		// [ Loop ]
		/// <summary>
		/// API 사용자가 게임 루프에서 호출해 주어야 합니다.
		/// </summary>
		public void OnTick() {
			Tick?.Invoke();
		}

		// [ Save / Load ]
		/// <summary>
		/// 프로젝트 경로에 데이터들을 저장합니다.
		/// </summary>
		/// <param name="directory"></param>
		public void Save() {
			if (string.IsNullOrEmpty(ProjectDir))
				throw new Exception("ProjectDir이 지정되지 않았습니다.");

			Directory.CreateDirectory(ProjectDir);

			// Asset
			AssetManager.SaveMetas();

			// Data
			string dataString = GetDataJsonString();
			string dataFilename = DataFilename;

			Directory.CreateDirectory(Path.GetDirectoryName(dataFilename));
			File.WriteAllText(dataFilename, dataString, Encoding.UTF8);
		}

		/// <summary>
		/// 클라이언트에서 플레이할 수 있는 데이터 파일로 내보냅니다.
		/// </summary>
		public void Export(string filename) {
			//ProjectDir을 압축해서 파일로 Export

			Save();

			using (ZipFile zip = new ZipFile(Encoding.UTF8)) {
				zip.AddDirectory(ProjectDir);

				zip.Save(filename);
			}
		}

		public void SetProjectDir(string directory) {
			ProjectDir = directory;

			CreateEditorDirectories();
		}

		private void CreateEditorDirectories() {
			Directory.CreateDirectory(AssetDir);
			Directory.CreateDirectory(AssetMetaDir);
		}

		public string GetDataJsonString() {
			JObject jFile = new JObject();

			exportedTime = DateTime.Now;

			//File info
			jFile.Add(nameof(exportedTime), exportedTime.ToOADate());

			jFile.Add("MotionFile", MotionFile.ToJObject());
			jFile.Add("UiFile", UiFile.ToJObject());
			jFile.Add("StoryFile", StoryFile.ToJObject());
			jFile.Add("ProjectSetting", ProjectSetting.ToJObject());

			return jFile.ToString();
		}
	}
}
