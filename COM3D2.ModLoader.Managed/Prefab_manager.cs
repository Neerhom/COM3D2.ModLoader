using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Reflection;

namespace COM3D2.ModLoader.Managed
{
    class Prefab_manager
    {
        // add all existing .asset_bg files from Mod to GameUty.BgFiles dictionary
        public static void Addbundlestobg()
        {
            string[] array = GameUty.m_ModFileSystem.GetFileListAtExtension(".asset_bg"); ;
            for (int num9 = 0; num9 < array.Length; num9++)
            {

                string fileName = Path.GetFileName(array[num9]);

                GameUty.BgFiles[fileName] = GameUty.m_ModFileSystem;

            }
        }
        
        // override Object loaded from resources with Object created from asset bundle if bundle with same name exists
        public static void PhotBGObj_Instantiate_Ext(PhotoBGObjectData self, ref UnityEngine.Object @object)
        {
            string name = Path.GetFileName(self.create_prefab_name.ToLower());
            if ((GameUty.BgFiles.ContainsKey(name + ".asset_bg")))
            { @object = GameMain.Instance.BgMgr.CreateAssetBundle(name); }

        }

        // create prefab override in Maid.AddPrefab which is for character prefabs such as cum particles
        public static void  Maid_prefab_override (ref UnityEngine.Object @object, string f_strPrefab, string f_strName, string f_strDestBone, Vector3 f_vOffsetLocalPos, Vector3 f_vOffsetLocalRot)
        {
            string name = Path.GetFileName(f_strPrefab.ToLower());
            if ((GameUty.BgFiles.ContainsKey(name + ".asset_bg")))
            { @object = GameMain.Instance.BgMgr.CreateAssetBundle(name); }
        }

        // create prefab override in  BgMgr.AddPrefabToBg which is for background prefabs loaded
        // via scripts, such as dildobox
       
        public static UnityEngine.GameObject BgMgr_prefab_override(BgMgr self, string src, string name)
        {
            GameObject gameObject3 = null;
            if (!self.m_dicAttachObj.TryGetValue(name, out gameObject3))
            {
                UnityEngine.Object @object = self.CreateAssetBundle(src);
                if (@object == null)
                {
                    @object = Resources.Load("Prefab/" + src);
                }
                if (@object == null)
                {
                    return null;
                }
                gameObject3 = (UnityEngine.Object.Instantiate(@object) as GameObject);
                gameObject3.name = name;
                self.m_dicAttachObj.Add(name, gameObject3);
            }
            return gameObject3;
        }


        // create prefab override in desk manager
        // this bit of functionality is unlikely to be used, but it's mostly for feature-completion

        public static void DeskManger_OnchangeBg_EXT(ref DeskManager.InstansData instansData, ref GameObject gameObject)
        {
            string name = Path.GetFileName(instansData.item_data.prefab_name.ToLower());

            if ((GameUty.BgFiles.ContainsKey(name + ".asset_bg")))
            { gameObject = GameMain.Instance.BgMgr.CreateAssetBundle(name); }

        }
    }
}
