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

      


        // add data from *phot_bg_list*.nei files to PhotoBGData.bg_data_

        public static void PhotoBGext()
        {

            string[] bg_nelist = null;
            bg_nelist = GameUty.FileSystemMod.GetList("PhotoBG_NEI", AFileSystemBase.ListType.AllFile);
            

            if (bg_nelist == null || 0 == bg_nelist.Length)
            { return; }
            foreach (string str in bg_nelist)
            {
                string nei_filename = Path.GetFileName(str);

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
                                    int num2 = 1;
                                    PhotoBGData photoBGData = new PhotoBGData(); // this requires prepatched assembly to compile
                                    photoBGData.id = "";
                                    photoBGData.category = csvParser.GetCellAsString(num2++, k);
                                    photoBGData.name = csvParser.GetCellAsString(num2++, k);
                                    photoBGData.create_prefab_name = csvParser.GetCellAsString(num2++, k);
                                    //assign id from prefab/bundles string
                                    //this is done because save/load of objects in photomode is based on id
                                    if (!string.IsNullOrEmpty(photoBGData.create_prefab_name))
                                    {
                                        photoBGData.id = photoBGData.create_prefab_name.GetHashCode().ToString();
                                        // this feels wrong, but i suspect the KISS might convert id to int at some point
                                        // so i'd rather be on safe side
                                    }

                                    string check = csvParser.GetCellAsString(num2++, k);
                                    if (String.IsNullOrEmpty(check) || GameUty.BgFiles.ContainsKey(photoBGData.create_prefab_name.ToLower() + ".asset_bg"))
                                    {
                                        PhotoBGData.bg_data_.Add(photoBGData);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log($"Skipping invalid file: Mod/{str}");
                            }
                        }
                    }
                }

            }

            
        }

        // add data from *phot_bg_object_list*.nei files to PhotoBGObjectData.bg_data_
        public static void PhotoBGobjext()
        {


            string[] BgObj_list = null;
            
                BgObj_list = GameUty.FileSystemMod.GetList("PhotoBG_OBJ_NEI", AFileSystemBase.ListType.AllFile);
           

            if (BgObj_list == null || 0 == BgObj_list.Length)
            { return; }


            foreach (string str in BgObj_list)
            {
                string nei_filename = Path.GetFileName(str);
               

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
                                    int num = 1;
                                    PhotoBGObjectData photoBGObjectData = new PhotoBGObjectData(); // this requires prepatched assembly to compile
                                    photoBGObjectData.id = 0; // not sure if't necessary for id to actually have a value
                                    photoBGObjectData.category = csvParser.GetCellAsString(num++, i);
                                    photoBGObjectData.name = csvParser.GetCellAsString(num++, i);
                                    photoBGObjectData.create_prefab_name = csvParser.GetCellAsString(num++, i);
                                    photoBGObjectData.create_asset_bundle_name = csvParser.GetCellAsString(num++, i);
                                    //assign id from rpefab/bundles string
                                    //this is done because save/load of objects in photomode is based on id
                                    if (!string.IsNullOrEmpty(photoBGObjectData.create_prefab_name))
                                    {
                                        photoBGObjectData.id = photoBGObjectData.create_prefab_name.GetHashCode();
                                    }
                                    else if (!string.IsNullOrEmpty(photoBGObjectData.create_asset_bundle_name))
                                    {
                                        photoBGObjectData.id = photoBGObjectData.create_asset_bundle_name.GetHashCode();
                                    }
                                    string check = csvParser.GetCellAsString(num++, i);
                                    if (String.IsNullOrEmpty(check) || GameUty.BgFiles.ContainsKey(photoBGObjectData.create_asset_bundle_name.ToLower() + ".asset_bg"))
                                    {
                                        PhotoBGObjectData.bg_data_.Add(photoBGObjectData);
                                    }

                                }
                            }
                            else
                            {
                                Debug.Log($"Skipping invalid file: Mod/{str}");

                            }

                        }
                    }
                }
            }
            
        }



        


        // add data from *phot_motion_list*.nei files to PhotoMotionData.motion_data_
        public static void PhotMotExt()
        {

            string[] PhotoMotNei = null;

            PhotoMotNei = GameUty.FileSystemMod.GetList("PhotMot_NEI", AFileSystemBase.ListType.AllFile);
            
            
         //   if (PhotoMotNei == null || PhotoMotNei.Length == 0)
         //   { return; }

            foreach (string str in PhotoMotNei)
            {
                string nei_filename = Path.GetFileName(str);

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
                                    PhotoMotionData photoMotionData = new PhotoMotionData(); // this requires prepatched assembly to compile
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
                            { Debug.Log($"Skipping invalid file: Mod/{str}"); }


                        }
                    }
                }

            }
            
        }


        // add data to desk_item_detail.nei and desk_item_category.nei 
        public static void DeskData_Ext ()
        {
#if DEBUG
            Console.WriteLine("Console.Writeline says hi");
            Debug.Log("Debug.Log says hi"); 
#endif

            string[] DeskItemList = null;
             DeskItemList = GameUty.FileSystemMod.GetList("DeskItem_NEI", AFileSystemBase.ListType.AllFile);
            if (DeskItemList == null || DeskItemList.Length ==0)
            { return; }

            foreach (string str in  DeskItemList)
            {
                string neifile = Path.GetFileName(str);
                string extension = Path.GetExtension(neifile);

                // check if file is indeed a .nei file and it snot mennt to overwrite base files
                // also check if the file is category type or detail type to determine the processing
                // only files that contain category in their name are assumed to be category files all others nei files are assumbed to be detail files

                if (neifile.Contains("category") && extension == ".nei" && neifile != "desk_item_category.nei")
                {
                    using (AFileBase afileBase = GameUty.FileSystem.FileOpen(neifile))
                    {
                        using (CsvParser csvParser = new CsvParser())
                        {
                            bool condition = csvParser.Open(afileBase);
                            for (int k = 1; k < csvParser.max_cell_y; k++)
                            {
                                if (!csvParser.IsCellToExistData(0, k))
                                {
                                    break;
                                }
                                int cellAsInteger = csvParser.GetCellAsInteger(0, k);
                                string cellAsString = csvParser.GetCellAsString(1, k);
                                if (!DeskManager.item_category_data_dic_.ContainsKey(cellAsInteger))
                                {
                                    DeskManager.item_category_data_dic_.Add(cellAsInteger, cellAsString);
                                }
                            }
                        }
                    }
                }
                else if (neifile != "desk_item_detail.nei" && extension == ".nei")
                {
                    using (AFileBase afileBase = GameUty.FileSystem.FileOpen(neifile))
                    {
                        using (CsvParser csvParser = new CsvParser())
                        {

                            if (csvParser.Open(afileBase))
                            {
                                for (int j = 1; j < csvParser.max_cell_y; j++)
                                {
                                    if (csvParser.IsCellToExistData(0, j))
                                    {
                                        int cellAsInteger2 = csvParser.GetCellAsInteger(0, j);
                                        DeskManager.ItemData itemData = new DeskManager.ItemData(csvParser, j);


                                        // check if it's a prefab data and add it if it is
                                        // is impossible to check if prefab exists in resources files
                                        // so if referenced prefab doesn't exist i ngame files, the entry isn't going to work properly in game
                                        if (!string.IsNullOrEmpty(itemData.prefab_name))
                                        {
                                            itemData.id = itemData.prefab_name.GetHashCode();// override the id from file with hash of prefab string to minimize id conflicts
                                            DeskManager.item_detail_data_dic_.Add(itemData.id, itemData);
                                        }

                                        // check if entry refers to asset bundle, and if it is, check if it exists before addding the data
                                        else if (!string.IsNullOrEmpty(itemData.asset_name) && GameUty.BgFiles.ContainsKey(itemData.asset_name + ".asset_bg"))
                                        {
                                            itemData.id = itemData.asset_name.GetHashCode();
                                            DeskManager.item_detail_data_dic_.Add(itemData.id, itemData);

                                        }

                                    }
                                }
                            }
                            else
                            { Debug.Log($"Skipping invalid file: Mod/{str}"); }
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
            Dictionary<int, string> DupFilter = new Dictionary<int, string>();
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
                            string filename = Path.GetFileName(modpmat[i]);
                            using (AFileBase aFileBase = GameUty.FileSystemMod.FileOpen(filename))
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
                                                DupFilter.Add(key, filename);
                                                pmatlist.Add(key, new KeyValuePair<string, float>(key2, value));
                                            }
                                            else
                                            {
                                                Debug.LogWarning($"Skipping {filename}  because its target material has already been changed by {DupFilter[key]}  ");
                                            }
                                        }
                                        else
                                        {

                                            Debug.Log("ヘッダーエラー\n" + filename + "File header of Mod .pmat file is invalid! skipping it!");

                                        }
                                    }
                                }
                                else
                                {

                                    Debug.Log(filename + "を開けませんでした ( Mod .pmat file is invalid! skipping it)");
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
                            using (AFileBase aFileBase = GameUty.FileSystem.FileOpen(text))
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
                                            
                                            { Debug.Log("ヘッダーエラー\n" + text + "File header of official .pmat file or Mod override is invalid! skipping it!"); }
                                        }
                                    }
                                }
                                else
                                {
                                 
                                    { Debug.Log(text + "を開けませんでした ( Official .pmat file or Mod override is invalid! skipping it)"); }
                                }
                            }
                        }
                    }
                }

                m_hashPriorityMaterials.SetValue(null, pmatlist);
            }
        }

        public static bool ManMenuAdd_run = false;
        public static void ManMenuAdd(PhotoManEditManager self) {

            if (ManMenuAdd_run){ return; }
            ManMenuAdd_run = true;

            FieldInfo menu_file_name_list_ = typeof(PhotoManEditManager).GetField("menu_file_name_list_", BindingFlags.Static | BindingFlags.NonPublic);
            HashSet<string> menu_filename_list_val =  (HashSet<string>)menu_file_name_list_.GetValue(null);
            FieldInfo man_body_menu_list_self = self.GetType().GetField("man_body_menu_list_", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo man_head_menu_list_self = self.GetType().GetField("man_head_menu_list_", BindingFlags.NonPublic | BindingFlags.Instance);


            HashSet<string> menu_list = new HashSet<string>();
            List<SceneEdit.SMenuItem> head_list = new List<SceneEdit.SMenuItem>();
            List<SceneEdit.SMenuItem> body_list = new List<SceneEdit.SMenuItem>();
            MPN body = (MPN)Enum.Parse(typeof(MPN), "body");
            
            foreach (string filename in GameUty.ModOnlysMenuFiles)
            {
                if (filename.Contains("mhead") || filename.Contains("mbody"))
                {
                    menu_list.Add(filename);
                }
                
            }

            Debug.Log($"menu list pre union{ menu_list}");
            if (menu_filename_list_val != null)
            {
                menu_list.UnionWith(menu_filename_list_val);
            }
            Debug.Log($"menu list post union{ menu_list}");
            foreach (string filename in menu_list)

            {
                SceneEdit.SMenuItem smenuItem = new SceneEdit.SMenuItem();
                if (SceneEdit.GetMenuItemSetUP(smenuItem, filename, true))
                {
                    if (smenuItem.m_mpn == body)
                    {
                        body_list.Add(smenuItem);
                    }
                    else 
                    {
                        head_list.Add(smenuItem);
                    }


                }

            }

              
            menu_file_name_list_.SetValue(null, menu_list);
            man_body_menu_list_self.SetValue(self, body_list);
            man_head_menu_list_self.SetValue(self, head_list);
        }
    
    
    
    
    
    
    
    
    
    
    
    
    }


           

}

