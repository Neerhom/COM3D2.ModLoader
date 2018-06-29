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
            string name = Path.GetFileName(self.create_prefab_name);
            if ((GameUty.FileSystemMod.IsExistentFile(name + ".asset_bg")))
            { @object = GameMain.Instance.BgMgr.CreateAssetBundle(name); }

        }

        // create prefab override in Maid.AddPrefab which is for character prefabs such as cum particles
        public static void  Maid_prefab_override (ref UnityEngine.Object @object, string f_strPrefab, string f_strName, string f_strDestBone, Vector3 f_vOffsetLocalPos, Vector3 f_vOffsetLocalRot)
        {
            string name = Path.GetFileName(f_strPrefab);
            if ((GameUty.FileSystemMod.IsExistentFile(name + ".asset_bg")))
            { @object = GameMain.Instance.BgMgr.CreateAssetBundle(name); }
        }

        // create prefab override in  BgMgr.AddPrefabToBg which is for background prefabs loaded
        // via scripts, such as dildobox
        public static void BgMgr_prefab_override(ref UnityEngine.Object @object, string f_strSrc, string f_strName, string f_strDest, Vector3 f_vPos, Vector3 f_vRot)
        {
            string name = Path.GetFileName(f_strSrc);
            if ((GameUty.FileSystemMod.IsExistentFile(name + ".asset_bg")))
            { @object = GameMain.Instance.BgMgr.CreateAssetBundle(name); }
        }


    }
}
