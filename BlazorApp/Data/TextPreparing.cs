using System.Diagnostics;

namespace BlazorApp.Data;

public class TextPreparing: IFilePreparing
{
    private List<FileInfo> GetRecursFiles(DirectoryInfo rootDir)
    {
        List<FileInfo> fileList = new List<FileInfo>();
        try
        {
            var dirs = rootDir.GetDirectories();
            foreach (var dir in dirs)
            {
                fileList.AddRange(GetRecursFiles(dir));
            }
            var files = rootDir.GetFiles();
            foreach (var file in files)
            {
                if(file.Extension == ".cache" || file.Extension == ".config")
                    continue;
                
                fileList.Add(file);
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
        }
        return fileList;
    }
    
    public List<string> GetCleanText(DirectoryInfo dir)
    {
        List<string> list = new List<string>();

        List<FileInfo> fileList = GetRecursFiles(dir);

        foreach (var file in fileList)
        {
            foreach (string line in File.ReadAllLines(file.FullName))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    foreach (var ch in line)
                    {
                        if (char.IsLetterOrDigit(ch))
                        {
                            list.Add(line.Trim().ToLower());
                            break;
                        }
                    }
                }
            }
        }
        
        return list;
    }

    //для текстовых файлов
    public Dictionary<string, List<string>> FillCheckingDictionary(string path)
    {
        Dictionary<string, List<string>> _dictionary = new Dictionary<string, List<string>>();
        
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        var dirs = directoryInfo.GetDirectories();
        foreach (var dir in dirs)
        {           
            var textList = GetCleanText(dir);
            _dictionary.Add(dir.Name, textList);
        }

        return _dictionary;
    }
}