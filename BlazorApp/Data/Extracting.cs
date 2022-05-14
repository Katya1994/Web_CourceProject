using System.Diagnostics;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace BlazorApp.Data;

public static class Extracting
{
    //для проектов в архиве
    public static void ExtractAll(string rootPath)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            var dirs = directoryInfo.GetDirectories();

            foreach (var dir in dirs)
            {
                //extract zip
                var tempList = dir.GetFiles();
                foreach (var file in tempList)
                {
                    if (file.Extension == ".zip")
                    {
                        ExtractZipContent(file.FullName, null, $"{file.DirectoryName}");
                        file.Delete();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
        }
    }
    
    private static void ExtractZipContent(string FileZipPath, string password, string OutputFolder)
    {
        ZipFile file = null;
        try
        {
            FileStream fs = File.OpenRead(FileZipPath);
            file = new ZipFile(fs);

            if (!String.IsNullOrEmpty(password))
            {
                file.Password = password;
            }

            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                    {
                        // Ignore directories
                        continue;           
                    }

                String entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                // 4K is optimum
                byte[] buffer = new byte[4096];     
                Stream zipStream = file.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                String fullZipToPath = Path.Combine(OutputFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);

                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }
        finally
        {
            if (file != null)
            {
                file.IsStreamOwner = true; // Makes close also shut the underlying stream
                file.Close(); // Ensure we release resources
            }
        }
    }
}