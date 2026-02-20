namespace ReportApi.Storage;

public interface IStorage
{
    public void SaveFile(string path, byte[] content);
    public void DeleteFile(string path);
    public byte[] ReadAllContent(string path);
    public bool Exists(string path);
}