using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

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

                for (int i=0;i< GameUty.m_aryModOnlysMenuFiles.Length; i++ )
                {
                
                  GameUty.m_aryModOnlysMenuFiles[i] = Path.GetFileName(GameUty.m_aryModOnlysMenuFiles[i]);
                }

                GameUty.m_aryMenuFiles = GameUty.m_aryMenuFiles.Concat(GameUty.m_aryModOnlysMenuFiles).ToArray();
                
            }
         
            // the rest of the code is taken from UpdateFileSystemPath assembly game ver 1.07 minus KISS' nosense
         
            if (GameUty.rid_menu_dic_.Count == 0)
            {
                
                GameUty.rid_menu_dic_ = new Dictionary<int, string>();
                foreach (string str in GameUty.m_aryMenuFiles)
                {
                    Debug.Log("rid stuff is happening");                 
                    int hashCode = str.GetHashCode();
                    
                    GameUty.rid_menu_dic_[hashCode] = str;
                    
                   
                    
                   // not sure what was the pont of this log
                   // as it goes nowhere, so i decided to cut off
                 //       NDebug.Assert(str == GameUty.rid_menu_dic_[hashCode], string.Concat(new string[]
                 //       {
                 //   "[",
                 //   str,
                 //   "]と[",
                 //   GameUty.rid_menu_dic_[hashCode],
                 //   "]は同じハッシュキーです"
                 //       }));
                    
                }
            }
            return true;
        }

    }
}
