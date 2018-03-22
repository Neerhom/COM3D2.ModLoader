using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COM3D2.ModMenuAccel.Hook
{
    public class ModmenuAccel
    {

        // this method is executed afer UpdateFileSystemPath assing arc menu filename list to m_aryMenuFiles  
        // and stops further exectuion of UpdateFileSystemPath after finisihng
        public static bool Accel()
        {

            if (GameUty.m_ModFileSystem != null)
            {


                GameUty.m_aryModOnlysMenuFiles = GameUty.m_ModFileSystem.GetFileListAtExtension(".menu"); //  this is where prefrormance gain is, as this only gets paths of menu files
                
                GameUty.m_aryMenuFiles = GameUty.m_aryMenuFiles.Concat(GameUty.m_aryModOnlysMenuFiles).ToArray();

              
            }
            // the rest of the code is taken from UpdateFileSystemPath assembly game ver 1.07
            
            if (GameUty.rid_menu_dic_.Count == 0)
            {
                string[] menuFiles = GameUty.MenuFiles;
                GameUty.rid_menu_dic_ = new Dictionary<int, string>();
                for (int num12 = 0; num12 < menuFiles.Length; num12++)
                {
                    string fileName2 = Path.GetFileName(menuFiles[num12]);
                    int hashCode = fileName2.ToLower().GetHashCode();
                    if (!GameUty.rid_menu_dic_.ContainsKey(hashCode))
                    {
                        GameUty.rid_menu_dic_.Add(hashCode, fileName2);
                    }
                    else
                    {
                        NDebug.Assert(fileName2 == GameUty.rid_menu_dic_[hashCode], string.Concat(new string[]
                        {
                    "[",
                    fileName2,
                    "]と[",
                    GameUty.rid_menu_dic_[hashCode],
                    "]は同じハッシュキーです"
                        }));
                    }
                }
            }
            return true;
        }

    }
}
