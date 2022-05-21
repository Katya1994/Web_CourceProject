using System.Diagnostics;

namespace BlazorApp.Data;

public class ByteCodePreparing : TextPreparing
{
    private const string _pathToBuild = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe";
    private const string _pathToIldasm = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\ildasm.exe";
    
    public string SlnPath { get; private set; }
    public string SavePath { get; private set;}

    // public ILCodePreparing(string slnPath, string savePath)
    // {
    //     SlnPath = slnPath;
    //     SavePath = savePath;
    // }

    private bool BuildProject()
    {
        Process process = null;
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(_pathToBuild);
            startInfo.Arguments = $"\"{SlnPath}\" /t:Rebuild /p:Configuration=Debug /p:TargetFrameworkVersion=v4.7.2";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            
            process = Process.Start(startInfo);
            process.WaitForExit();
            return true;
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            process.Kill();
            return false;
        }
    }

    private bool Disassemble(string personFolder, string key)
    {
        Process process = null;
        try
        {
            List<string> projects = new List<string>();

            var slndir = new FileInfo(SlnPath).Directory;
            var list = slndir.GetDirectories();

            foreach (var dir in slndir.GetDirectories())
            {
                if(dir.Name == ".vs")
                    continue;
                var file = $@"{slndir.FullName}\{dir.Name}\bin\Debug\{dir.Name}.exe";
                projects.Add(file);
            }
            
            foreach (var project in projects)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(_pathToIldasm);
                startInfo.Arguments = $"\"{project}\" /\"output:{SavePath}\\file_{key}_{projects.IndexOf(project)}.il\"";
                //не получается сделать, чтобы окно ildasm.exe не появлялось
                startInfo.UseShellExecute = false;
                startInfo.UserName = null;
                startInfo.CreateNoWindow = true;

                process = Process.Start(startInfo);
                process.WaitForExit();
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            process.Kill();
            return false;
        }
    }

    public Dictionary<string, List<string>> FillCheckingDictionary(string rootPath, string rootSavePath)
    {
        var dirs = new DirectoryInfo(rootPath).GetDirectories();
        
        foreach (var dir in dirs)
        {
            SavePath = Path.Combine(rootSavePath, dir.Name);

            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
            else if (new DirectoryInfo(SavePath).GetFiles().Length > 0)
                continue;
            
            SlnPath = dir.GetFiles()[0].FullName;
            BuildProject();
            Disassemble(SavePath, dir.Name);
        }

        return base.FillCheckingDictionary(rootSavePath);
    }
}