using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Reflection;


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

            string[] bg_nelist = new string[] { };
            try
            {
                bg_nelist = GameUty.FileSystemMod.GetList("PhotoBG_NEI", AFileSystemBase.ListType.AllFile);
            }
            catch (Exception) { }


            if (bg_nelist != null && 0 < bg_nelist.Length)
            {
                for (int j = 0; j < bg_nelist.Length; j++)
                {
                    string nei_filename = Path.GetFileName(bg_nelist[j]);

                    if (Path.GetExtension(nei_filename) == ".nei" && nei_filename != "phot_bg_list.nei")
                    {

                        using (AFileBase aFileBase2 = GameUty.FileSystemMod.FileOpen(nei_filename))
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
                                        //   string cellAsString = csvParser.GetCellAsString(num2++, k);
                                        PhotoBGData.bg_data_.Add(photoBGData);

                                    }
                                }
                                else
                                {
                                    Debug.Log($"Skipping invalid file: Mod/{bg_nelist[j]}");
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
                    string nei_filename = Path.GetFileName(BgObj_list[j]);

                    if (Path.GetExtension(nei_filename) == ".nei" && nei_filename != "phot_bg_object_list.nei")
                    {

                        using (AFileBase aFileBase = GameUty.FileSystemMod.FileOpen(nei_filename))
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
                                            photoBGObjectData.create_asset_bundle_name = csvParser.GetCellAsString(num++, i);
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
        // this is no longer required, as game can now use .asset_bg files as photo bg object, but since those loaded from create_asset_bundle_name field
        // removing this would mean loosing compatibility with older nei files.
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
                    string nei_filename = Path.GetFileName(PhotoMotNei[j]);

                    if (Path.GetExtension(nei_filename) == ".nei" && nei_filename != "phot_motion_list.nei")
                    {
                        using (AFileBase aFileBase = GameUty.FileSystem.FileOpen(nei_filename))
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


        public static void PmatHandler()
        {
            FieldInfo m_hashPriorityMaterials = typeof(ImportCM).GetField("m_hashPriorityMaterials",
                                            BindingFlags.Static | BindingFlags.NonPublic);
            var pmadt_dic_value = m_hashPriorityMaterials.GetValue(null);
            Dictionary<int, KeyValuePair<string, float>> pmatlist = new Dictionary<int, KeyValuePair<string, float>> ();
            if (pmadt_dic_value == null)
            {
                
                string[] gamepmat = GameUty.FileSystem.GetList("prioritymaterial", AFileSystemBase.ListType.AllFile);
                string[] modpmat = GameUty.m_ModFileSystem.GetFileListAtExtension(".pmat");
                
                if (modpmat != null && 0 < modpmat.Length)
                {
                    for (int i = 0; i < modpmat.Length; i++)
                    {
                        if (Path.GetExtension(modpmat[i]) == ".pmat")
                        {
                            string text = modpmat[i];
                            using (AFileBase aFileBase = GameUty.FileOpen(Path.GetFileName(text), null))
                            {
                                if (aFileBase.IsValid())
                                {
                                    byte[] buffer = aFileBase.ReadAll();
                                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer), Encoding.UTF8))
                                    {
                                        string a = binaryReader.ReadString();
                                        if (a == "CM3D2_PMATERIAL")
                                        {

                                            int num = binaryReader.ReadInt32();
                                            int key = binaryReader.ReadInt32();
                                            string key2 = binaryReader.ReadString();
                                            float value = binaryReader.ReadSingle();
                                            if (!pmatlist.ContainsKey(key))
                                            {
                                                pmatlist.Add(key, new KeyValuePair<string, float>(key2, value));
                                            }
                                            else
                                            {
                                                Debug.Log($"Skipping {text}  because its target material has already been changed by another Mod .pmat  ");
                                            }
                                        }
                                        else
                                        {

                                            Debug.Log("ヘッダーエラー\n" + text + "File header of Mod .pmat file is invalid! skipping it!");

                                        }
                                    }
                                }
                                else
                                {

                                    Debug.Log(text + "を開けませんでした ( Mod .pmat file is invalid! skipping it)");
                                }
                                
                            }
                        }
                    }
                }


                if (gamepmat != null && 0 < gamepmat.Length)
                {
                    for (int i = 0; i < gamepmat.Length; i++)
                    {
                        if (Path.GetExtension(gamepmat[i]) == ".pmat")
                        {
                            string text = gamepmat[i];
                            using (AFileBase aFileBase = GameUty.FileOpen(text, null))
                            {
                                if (aFileBase.IsValid())
                                {
                                    byte[] buffer = aFileBase.ReadAll();
                                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer), Encoding.UTF8))
                                    {
                                        string a = binaryReader.ReadString();
                                        if (a == "CM3D2_PMATERIAL")
                                        {

                                            int num = binaryReader.ReadInt32();
                                            int key = binaryReader.ReadInt32();
                                            string key2 = binaryReader.ReadString();
                                            float value = binaryReader.ReadSingle();
                                            if (!pmatlist.ContainsKey(key))
                                            {
                                                pmatlist.Add(key, new KeyValuePair<string, float>(key2, value));
                                            }
                                            
                                        }
                                        else
                                        {
                                            
                                            { Debug.Log("ヘッダーエラー\n" + text + "File header of official .pmat file is invalid! skipping it!"); }
                                        }
                                    }
                                }
                                else
                                {
                                 
                                    { Debug.Log(text + "を開けませんでした ( Official .pmat file is invalid! skipping it)"); }
                                }
                            }
                        }
                    }
                }

                m_hashPriorityMaterials.SetValue(null, pmatlist);
            }
        }
    }
           

}

