using System;
using System.IO;

public static class UniqueFileNameFinder {

    public static string Find(Func<string, string> getPath, string fileName, string extension)
    {
        int i = 0;

        //loop until we find a unique file name
        while (File.Exists(getPath(fileName + i + extension)))
        {
            i++;
        }

        return fileName + i + extension;
    }
}
