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

        // add all existing .asset_bg files from Mod to GameUty.BgFiles dictionary
        public static void addbundlestobg()
        {
            string[] array = GameUty.m_ModFileSystem.GetFileListAtExtension(".asset_bg"); ;
            for (int num9 = 0; num9 < array.Length; num9++)
            {

                string fileName = Path.GetFileName(array[num9]);

                GameUty.BgFiles[fileName] = GameUty.m_ModFileSystem;

            }
        }




        // add data from *phot_bg_list*.nei files to PhotoBGData.bg_data_

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

        // add data from *phot_bg_object_list*.nei files to PhotoBGObjectData.bg_data_
        public static void PhotoBGobjext()
        {


            string[] BgObj_list = new string[] { };
            try
            {
                BgObj_list = GameUty.FileSystemMod.GetList("PhotoBG_OBJ_NEI", AFileSystemBase.ListType.AllFile);
            }
            catch (Exception) { }

            if (BgObj_list != null && 0 < BgObj_list.Length)
            {


                for (int j = 0; j < BgObj_list.Length; j++)
                {

                    if (BgObj_list[j].Contains("phot_bg_object_list") && Path.GetExtension(BgObj_list[j]) == ".nei" && Path.GetFileName(BgObj_list[j]) != "phot_bg_object_list.nei")
                    {

                        using (AFileBase aFileBase = GameUty.FileSystemMod.FileOpen(BgObj_list[j]))
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
                                    Debug.Log($"Skipping invalid file: Mod/{BgObj_list[j]}");

                                }

                            }
                        }
                    }
                }
            }
        }



        //if game's base code fails to create GameObject from Resource files, then this method would load it from .asset_bg
        // this allows use of .asset_bg files as Photo Mode Bacground objects
        public static void PhotBGObj_Instantiate_Ext(PhotoBGObjectData self, ref UnityEngine.Object @object)
        {

            if (@object == null)
            {
                @object = GameMain.Instance.BgMgr.CreateAssetBundle(self.create_prefab_name);
            }

        }


        // add data from *phot_motion_list*.nei files to PhotoMotionData.motion_data_
        public static void PhotMotExt()
        {

            string[] PhotoMotNei = new string[] { };

            try
            {
                PhotoMotNei = GameUty.FileSystemMod.GetList("PhotMot_NEI", AFileSystemBase.ListType.AllFile);
            }
            catch (Exception) { }
            if (PhotoMotNei != null && PhotoMotNei.Length > 0)
            {
                for (int j = 0; j < PhotoMotNei.Length; j++)
                {
                    if (PhotoMotNei[j].Contains("phot_motion_list") && Path.GetExtension(PhotoMotNei[j]) == ".nei" && Path.GetFileName(PhotoMotNei[j]) != "phot_motion_list.nei")
                    {
                        using (AFileBase aFileBase = GameUty.FileSystem.FileOpen(PhotoMotNei[j]))
                        {
                            using (CsvParser csvParser = new CsvParser())
                            {
                                if (csvParser.Open(aFileBase))
                                {
                                    for (int i = 1; i < csvParser.max_cell_y; i++)
                                    {

                                        int num = 0;
                                        PhotoMotionData photoMotionData = new PhotoMotionData();
                                        photoMotionData.id = (long)csvParser.GetCellAsInteger(num++, i);
                                        photoMotionData.category = csvParser.GetCellAsString(num++, i);
                                        photoMotionData.name = csvParser.GetCellAsString(num++, i);
                                        photoMotionData.direct_file = csvParser.GetCellAsString(num++, i);
                                        photoMotionData.is_loop = (csvParser.GetCellAsString(num++, i) == "○");
                                        photoMotionData.call_script_fil = csvParser.GetCellAsString(num++, i);
                                        photoMotionData.call_script_label = csvParser.GetCellAsString(num++, i);
                                        photoMotionData.is_mod = false;
                                        string cellAsString = csvParser.GetCellAsString(num++, i);
                                        bool flag = csvParser.GetCellAsString(num++, i) == "○";
                                        photoMotionData.use_animekey_mune_l = (photoMotionData.use_animekey_mune_r = flag);
                                        photoMotionData.is_man_pose = (csvParser.GetCellAsString(num++, i) == "○");
                                        PhotoMotionData.motion_data_.Add(photoMotionData);


                                    }
                                }
                                else
                                { Debug.Log($"Skipping invalid file: Mod/{PhotoMotNei[j]}"); }


                            }
                        }
                    }



                }
            }
        }



           

    }
}
