using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace COM3D2.ModLoader.Managed
{
    public static class Hooks
    {

        public static void SwapFileSystems()
        {
            // Create a new mod file system
            ModFileSystem mod = new ModFileSystem();

            // Destroy the previously created file system
            GameUty.m_FileSystem.Dispose();
            // Swap the original file systems with the custom one
            GameUty.m_FileSystem = mod;

        }


        public static void addbundlestobg()
        {
            string[] array = GameUty.m_ModFileSystem.GetFileListAtExtension(".asset_bg"); ;
            for (int num9 = 0; num9 < array.Length; num9++)
            {
                string path = array[num9];
                string fileName = Path.GetFileName(path);
                if (!(Path.GetExtension(fileName) != ".asset_bg") && !global::GameUty.BgFiles.ContainsKey(fileName))
                {
                    global::GameUty.BgFiles.Add(fileName, global::GameUty.m_ModFileSystem);
                }
            }
        }
        





        public static void PhotoBGext()
        {
            HashSet<int> hashSet = new HashSet<int>();
            string[] bgneilist = GameUty.FileSystemMod.GetList("PhotoBG_NEI", AFileSystemBase.ListType.AllFile);
            
            if (bgneilist != null && 0< bgneilist.Length)
            { 
            for (int j = 0; j < bgneilist.Length; j++)
            {
                if (bgneilist[j].Contains("phot_bg_enabled_list") && bgneilist[j].Contains(".nei"))
                {
                    string fileName = bgneilist[j].Replace(".nei", "");
                    wf.CsvCommonIdManager.ReadEnabledIdList(wf.CsvCommonIdManager.FileSystemType.Normal, true, fileName, ref hashSet);
                }
          if (bgneilist[j].Contains("phot_bg_list") &&  bgneilist[j].Contains(".nei"))
                {
                    string f_strFileName = bgneilist[j];
                    using (AFileBase aFileBase2 = GameUty.FileSystemMod.FileOpen(f_strFileName))
                    {
                        using (CsvParser csvParser2 = new CsvParser())
                        {
                            NDebug.Assert(csvParser2.Open(aFileBase2), $"{f_strFileName} is invalid!");
                            for (int k = 1; k < csvParser2.max_cell_y; k++)
                            {
                                if (csvParser2.IsCellToExistData(0, k) && hashSet.Contains(csvParser2.GetCellAsInteger(0, k)))
                                {
                                    int num2 = 0;
                                    PhotoBGData photoBGData2 = new PhotoBGData();
                                    photoBGData2.id = csvParser2.GetCellAsInteger(num2++, k).ToString();
                                    photoBGData2.category = csvParser2.GetCellAsString(num2++, k);
                                    photoBGData2.name = csvParser2.GetCellAsString(num2++, k);
                                    photoBGData2.create_prefab_name = csvParser2.GetCellAsString(num2++, k);
                                    string cellAsString2 = csvParser2.GetCellAsString(num2++, k);
                                    if (string.IsNullOrEmpty(cellAsString2) || PluginData.IsEnabled(cellAsString2))
                                    {
                                        PhotoBGData.bg_data_.Add(photoBGData2);
                                    }
                                }
                            }
                        }
                    }
                }

            }

        }
        }

        public static void PhotoBGobjext()
        {
           
            HashSet<int> hashSet = new HashSet<int>();
            string[] BgObj_enabled = GameUty.FileSystemMod.GetList("PhotoBG_OBJ_NEI", AFileSystemBase.ListType.AllFile);
            if (BgObj_enabled != null && BgObj_enabled.Length > 0)
            {
                for (int j = 0; j < BgObj_enabled.Length; j++)
                {
                    if (BgObj_enabled[j].Contains("phot_bg_object_enabled_list") && BgObj_enabled[j].Contains(".nei"))
                    {
                        string filename = BgObj_enabled[j].Replace(".nei", "");

                        wf.CsvCommonIdManager.ReadEnabledIdList(wf.CsvCommonIdManager.FileSystemType.Normal, true, filename, ref hashSet);
                    }
                     if (BgObj_enabled[j].Contains("phot_bg_object_list") && BgObj_enabled[j].Contains(".nei"))
                    {
                        string filename_full = BgObj_enabled[j];

                        using (AFileBase aFileBase = GameUty.FileSystem.FileOpen(filename_full))
                        {
                            using (CsvParser csvParser = new CsvParser())
                            {
                                bool condition = csvParser.Open(aFileBase);
                                NDebug.Assert(condition, $"{aFileBase} is invalid!");
                                for (int i = 1; i < csvParser.max_cell_y; i++)
                                {
                                    if (csvParser.IsCellToExistData(0, i) && hashSet.Contains(csvParser.GetCellAsInteger(0, i)))
                                    {
                                        int num = 0;
                                        PhotoBGObjectData photoBGObjectData = new PhotoBGObjectData();
                                        photoBGObjectData.id = (long)csvParser.GetCellAsInteger(num++, i);
                                        photoBGObjectData.category = csvParser.GetCellAsString(num++, i);
                                        photoBGObjectData.name = csvParser.GetCellAsString(num++, i);
                                        photoBGObjectData.create_prefab_name = csvParser.GetCellAsString(num++, i);
                                        string cellAsString = csvParser.GetCellAsString(num++, i);
                                        if (string.IsNullOrEmpty(cellAsString) || PluginData.IsEnabled(cellAsString))
                                        {
                                            PhotoBGObjectData.bg_data_.Add(photoBGObjectData);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void PhotBGObj_Instantiate_Ext (PhotoBGObjectData self, ref UnityEngine.Object @object)
        {

            if (@object == null)
            {
                @object = GameMain.Instance.BgMgr.CreateAssetBundle(self.create_prefab_name);
            }
           
        }


        public static void ModPmat(ref string[] list)
        {
           

            string[] listmod = GameUty.m_ModFileSystem.GetFileListAtExtension(".pmat");

           
            if (list != null && list.Length > 0)
            {
                List<string> convertlist = list.ToList();
                for (int i = 0; i < listmod.Length; i++)
                {
                    convertlist.Add(listmod[i]);
                }

                list = convertlist.ToArray();
            }

           

        }

    }
}
