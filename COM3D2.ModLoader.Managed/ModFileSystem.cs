using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace COM3D2.ModLoader.Managed
{
    /// <summary>
    /// A custom file system for GameUty that gives priority to game's Mod File System.
    /// 
    /// You can modify methods to allow loading from other places as well.
    /// </summary>
    public class ModFileSystem : FileSystemArchive
    {
        /// <summary>
        /// Load a file by the name.
        /// </summary>
        /// <param name="f_strFileName">Name of the file, including possible path.</param>
        /// <returns></returns>
        public override AFileBase FileOpen(string f_strFileName)
        {
            if (GameUty.m_ModFileSystem != null && GameUty.m_ModFileSystem.IsExistentFile(f_strFileName))
                return GameUty.m_ModFileSystem.FileOpen(f_strFileName);

            // If anything else fails, load file from in-game storage
            return base.FileOpen(f_strFileName);
        }

        /// <summary>
        /// Gets a list of files that can be loaded.
        /// </summary>
        /// <param name="f_str_path">Path to a directory.</param>
        /// <param name="type">Type of the list.</param>
        /// <returns>A list of files in the specified path.</returns>
        public override string[] GetList(string f_str_path, ListType type)
        {


            if (GameUty.m_ModFileSystem != null && f_str_path == "prioritymaterial")
            {
                Dictionary<string, string> pmat = new Dictionary<string, string>();
                foreach (string str in base.GetList(f_str_path, type))
                      pmat[Path.GetFileName(str)] = str;
                    

                foreach (string str in GameUty.m_ModFileSystem.GetFileListAtExtension(".pmat"))
                    pmat[Path.GetFileName(str)] = str;

                return pmat.Values.ToArray();
            }
                     
            
                return base.GetList(f_str_path, type);
            
        }

        /// <summary>
        /// Gets all files of the specified extension.
        /// </summary>
        /// <param name="extension">Extension of the file.</param>
        /// <returns>A list of files with the specified extension.</returns>
     //   public override string[] GetFileListAtExtension(string extension)
     //   {
     //       List<string> result = new List<string>(base.GetFileListAtExtension(extension));
     //
     //       if (GameUty.m_ModFileSystem != null)
     //           result.AddRange(GameUty.m_ModFileSystem.GetFileListAtExtension(extension));
     //
     //       return result.ToArray();
     //
     //   }

        /// <summary>
        /// Checks if the file exists in the file system.
        /// </summary>
        /// <param name="f_strFileName">File to check.</param>
        /// <returns>True, if file exists. Otherwise, false.</returns>
        public override bool IsExistentFile(string f_strFileName)
        {
            return base.IsExistentFile(f_strFileName)
                   || GameUty.m_ModFileSystem != null && GameUty.m_ModFileSystem.IsExistentFile(f_strFileName);
        }
    }
}
