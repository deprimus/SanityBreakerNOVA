using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

class DifficultAssetCopier
{
    [PostProcessBuildAttribute(0)]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        try
        {
            string destDir = Path.Combine(Directory.GetDirectories(Path.GetDirectoryName(path), "*_data")[0], "Difficulties");

            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.EnumerateFiles(NovaCustomDifficulty.CUSTOM_DIFFICULTIES_PATH, "*.json", SearchOption.AllDirectories))
            {
                try
                {
                    string dest = Path.Combine(destDir, Path.GetFileName(file));
                    File.Copy(file, dest);
                    Debug.Log(string.Format("Copied '{0}' to '{1}'", file, dest));
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
