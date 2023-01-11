using System;
using System.Collections.Generic;
using System.IO;

namespace AhesselbomGenerator;

public class FileList : List<FileInfo>
{
    public void LoadFromPath(string path)
    {
        Clear();

        var d = new DirectoryInfo(path);

        if (!d.Exists)
            throw new SystemException();
            
        Scan(d);
            
        void Scan(DirectoryInfo p)
        {
            foreach (var f in p.GetFiles())
                Add(f);

            foreach (var dir in p.GetDirectories())
                Scan(dir);
        }
    }
}