using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Inject;
using System.Reflection;
using Mono.Cecil.Cil;

namespace COM3D2.AssetCleaner.Patcher
{
    public class Patcher
    {

        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll" };


        public static void Patch(AssemblyDefinition assembly)

        {

            TypeDefinition Hook = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location).MainModule.GetType("COM3D2.AssetCleaner.Patcher.Cleaner_Hook");


            TypeDefinition BgMBgr = assembly.MainModule.GetType("BgMgr");
            MethodDefinition ChangeBg = BgMBgr.GetMethod("ChangeBg");

            //cahnge dictionary acces for hook acces
           
            TypeDefinition AssetBundleObj = BgMBgr.NestedTypes.First(t => t.Name == "AssetBundleObj");

            AssetBundleObj.IsPublic = true;
            BgMBgr.ChangeAccess("asset_bundle_dic", true);
             

            ChangeBg.InjectWith(Hook.GetMethod("Bundle_Cleaner"), flags: InjectFlags.PassInvokingInstance);
        }
    }
}