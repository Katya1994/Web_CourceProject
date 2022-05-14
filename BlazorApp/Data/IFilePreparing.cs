namespace BlazorApp.Data;

public interface IFilePreparing
{
    public Dictionary<string, List<string>> FillCheckingDictionary(string path);
}