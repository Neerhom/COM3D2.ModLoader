using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace COM3D2.AssetCleaner.Patcher
{
    public static class Cleaner_Hook
    {

        // the method is hooked into BgMgr.ChangeBg and unloads previously loded asset bundle used for BG prefab
        public static void Bundle_Cleaner(BgMgr self)
        {
            if (self.current_bg_object!=null)
            {
                
                
                // check if prefab was loaded from asset bundle by checking cache dictionary
               
                string bundle_name = self.GetBGName() + ".asset_bg"; 
             
                if (self.asset_bundle_dic.ContainsKey(bundle_name))
                {
                    //if it's cahced, unload the bundle and all object loaded from it and clear chache key
                    self.asset_bundle_dic[bundle_name].ab.Unload(true); 
                    self.asset_bundle_dic.Remove(bundle_name);
                }
            }

           
           
            // it appears that KISS does unload bundles uses for BG objects, when all objects loaded from it are destroyed
            // so those are do no cause VRAM&RAM leakage
            // chances are, this patcher will become redundant when KISS decided to bother fixing this issue
            

        }




    }
}