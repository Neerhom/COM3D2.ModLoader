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
                if (!(Path.GetExtension(fileName) != ".asset_bg") && !GameUty.BgFiles.ContainsKey(fileName))
                {
                    GameUty.BgFiles.Add(fileName, GameUty.m_ModFileSystem);
                }
            }
        }
        





        public static void PhotoBGext()
        {
           
            string[] bgneilist = new string[] { };
            try
            {
                bgneilist = GameUty.FileSystemMod.GetList("PhotoBG_NEI", AFileSystemBase.ListType.AllFile);
            }
            catch (Exception) { }

         
            if (bgneilist != null && 0 < bgneilist.Length)
            {
                for (int j = 0; j < bgneilist.Length; j++)
                {
                     if (bgneilist[j].Contains("phot_bg_list") && Path.GetExtension(bgneilist[j]) == ".nei" && Path.GetFileName(bgneilist[j]) != "phot_bg_list.nei")
                    {
                        //string f_strFileName = bgneilist[j];
                        using (AFileBase aFileBase2 = GameUty.FileSystemMod.FileOpen(bgneilist[j]))
                        {
                            using (CsvParser csvParser = new CsvParser())
                            {

                                if (csvParser.Open(aFileBase2))
                                {


                                    for (int k = 1; k < csvParser.max_cell_y; k++)
                                    {
                                            int num2 = 0;
                                            PhotoBGData photoBGData = new PhotoBGData();
                                            photoBGData.id = csvParser.GetCellAsInteger(num2++, k).ToString();
                                            photoBGData.category = csvParser.GetCellAsString(num2++, k);
                                            photoBGData.name = csvParser.GetCellAsString(num2++, k);
                                            photoBGData.create_prefab_name = csvParser.GetCellAsString(num2++, k);
                                            string cellAsString = csvParser.GetCellAsString(num2++, k);
                                            PhotoBGData.bg_data_.Add(photoBGData);
                                        
                                    }
                                }
                                else
                                {
                                    Debug.Log($"Skipping invalid file: Mod/{bgneilist[j]}");
                                }
                            }
                        }
                    }

                }

            }
        }
        
        public static void PhotoBGobjext()
        {
           
            
            string[] BgObj_enabled = new string[] { };
            try
            {
                BgObj_enabled = GameUty.FileSystemMod.GetList("PhotoBG_OBJ_NEI", AFileSystemBase.ListType.AllFile);
            }
            catch (Exception) { }
           
            if (BgObj_enabled != null && 0 < BgObj_enabled.Length)
            {
               
                
                for (int j = 0; j < BgObj_enabled.Length; j++)
                {
                    Debug.Log(BgObj_enabled[j]);
                      if (BgObj_enabled[j].Contains("phot_bg_object_list") && Path.GetExtension(BgObj_enabled[j]) == ".nei" && Path.GetFileName(BgObj_enabled[j]) != "phot_bg_object_list.nei")
                    {
                       
                        using (AFileBase aFileBase = GameUty.FileSystemMod.FileOpen(BgObj_enabled[j]))
                        {
                            using (CsvParser csvParser = new CsvParser())
                            {

                                if (csvParser.Open(aFileBase))
                                {
                                    for (int i = 1; i < csvParser.max_cell_y; i++)
                                    {
                                 
                                        {
                                            int num = 0;
                                            PhotoBGObjectData photoBGObjectData = new PhotoBGObjectData();
                                            photoBGObjectData.id = (long)csvParser.GetCellAsInteger(num++, i);
                                            photoBGObjectData.category = csvParser.GetCellAsString(num++, i);
                                            photoBGObjectData.name = csvParser.GetCellAsString(num++, i);
                                            photoBGObjectData.create_prefab_name = csvParser.GetCellAsString(num++, i);
                                            PhotoBGObjectData.bg_data_.Add(photoBGObjectData);
                                    
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.Log($"Skipping invalid file: Mod/{BgObj_enabled[j]}");

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

 

    }
}
