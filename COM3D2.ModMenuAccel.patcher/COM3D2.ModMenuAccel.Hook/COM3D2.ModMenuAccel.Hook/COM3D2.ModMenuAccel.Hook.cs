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
        
        // executed right before game fetches list of files in mod filesystem

        public static void Accel()
        {
            // initialize m_ModFileSystem if for some reason it has not been to avoid potential crash
            // also return as there  is nothing to parse in empty filesystem
            if (GameUty.m_ModFileSystem == null)
            {
                GameUty.m_ModFileSystem = new FileSystemWindows();
                return;
            }


            GameUty.m_aryModOnlysMenuFiles = GameUty.m_ModFileSystem.GetFileListAtExtension(".menu"); //  this is where prefrormance gain is, as this only gets paths of menu files

            for (int i=0;i< GameUty.m_aryModOnlysMenuFiles.Length; i++ )
            {
                
                GameUty.m_aryModOnlysMenuFiles[i] = Path.GetFileName(GameUty.m_aryModOnlysMenuFiles[i]);
            }
                
      

        }

    }
}
