namespace TaleKit.Datas {
	public interface ITaleDataFile {
		bool Save(string filename);
		bool Load(string filename);
	}
}
