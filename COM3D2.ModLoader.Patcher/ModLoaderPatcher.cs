using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Inject;
using Mono.Cecil.Cil;
using UnityEngine;

namespace COM3D2.ModLoader.Patcher
{
    public static class ModLoaderPatcher
    {
        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll" };
        private const string HOOK_NAME = "COM3D2.ModLoader.Managed";



        public static void Patch(AssemblyDefinition assembly)
        {
           
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string hookDir = $"{HOOK_NAME}.dll";
            AssemblyDefinition hookAssembly = AssemblyLoader.LoadAssembly(Path.Combine(assemblyDir, hookDir));

            TypeDefinition hooks = hookAssembly.MainModule.GetType($"{HOOK_NAME}.Hooks");
            TypeDefinition Prefab_manager = hookAssembly.MainModule.GetType($"{HOOK_NAME}.Prefab_manager");

            TypeDefinition gameUty = assembly.MainModule.GetType("GameUty");
            

            MethodDefinition init = gameUty.GetMethod("Init");
            MethodDefinition swapFileSystems = hooks.GetMethod("SwapFileSystems");

            init.InjectWith(swapFileSystems, 2); // Patch right after the file system was initially set
            gameUty.ChangeAccess("m_FileSystem", true, false); // Make original file system public to allow replacing
            gameUty.ChangeAccess("m_ModFileSystem", true, false); // Make the mod file system public

            // hook in the ArcLoader to enable loading of ARCs and .ks files
            init.InjectWith(hookAssembly.MainModule.GetType($"{HOOK_NAME}.ArcLoader").GetMethod("Install") ,-1);




            // enable nei append to PhotBGData
            TypeDefinition PhotoBGData = assembly.MainModule.GetType("PhotoBGData");
            PhotoBGData.ChangeAccess("bg_data_", true, false); // make public to allow hook access
            MethodDefinition PhotoBGDataCreate = PhotoBGData.GetMethod("Create");
            MethodDefinition PhotoBgExt = hooks.GetMethod("PhotoBGext");

            for (int inst = 0; inst < PhotoBGDataCreate.Body.Instructions.Count; inst++)

            {
                if (PhotoBGDataCreate.Body.Instructions[inst].OpCode == OpCodes.Call)
                {
                    MethodReference target = PhotoBGDataCreate.Body.Instructions[inst].Operand as MethodReference;
                    if (target.Name == "GetSaveDataDic")
                    {
                        PhotoBGDataCreate.InjectWith(PhotoBgExt, inst);
                        break;
                    }
                }
            }

            // enable nei append to PhotBGObjectData
            TypeDefinition PhotoBGObjectData = assembly.MainModule.GetType("PhotoBGObjectData");
            PhotoBGObjectData.ChangeAccess("bg_data_",true, false); // make public to allow hook access
            MethodDefinition PhotoBobjectDataCreate = PhotoBGObjectData.GetMethod("Create");
            MethodDefinition PhotoBGobjext = hooks.GetMethod("PhotoBGobjext");

           
            for (int inst = 0; inst < PhotoBobjectDataCreate.Body.Instructions.Count; inst++)

            {
                if (PhotoBobjectDataCreate.Body.Instructions[inst].OpCode == OpCodes.Stsfld)
                {

                    FieldDefinition target = PhotoBobjectDataCreate.Body.Instructions[inst].Operand as FieldDefinition;
                    if (target.Name == "category_list_")
                    {
                        PhotoBobjectDataCreate.InjectWith(PhotoBGobjext, inst - 1);
                         break;
                    }

                }
            }

           


        
     


            // nei apped PhotoMotionData
            TypeDefinition PhotoMotionData = assembly.MainModule.GetType("PhotoMotionData");
            PhotoMotionData.ChangeAccess("motion_data_", true);
            MethodDefinition PhotoMotionDataCreate = PhotoMotionData.GetMethod("Create");

            MethodDefinition PhotMotExt = hooks.GetMethod("PhotMotExt");
            for (int inst=0; inst< PhotoMotionDataCreate.Body.Instructions.Count; inst++)
            {
                if (PhotoMotionDataCreate.Body.Instructions[inst].OpCode == OpCodes.Stfld)
                {
                    FieldReference target = PhotoMotionDataCreate.Body.Instructions[inst].Operand as FieldReference;
                    if (target.Name == "CheckModFile")
                    {
                        PhotoMotionDataCreate.InjectWith(PhotMotExt, inst-2);
                        break;
                    }
                }
            }

            // patch in PmatHandler for loading of mod pmat files, override of base .pmat files and handling of pmat hash conflicts

           TypeDefinition ImportCM = assembly.MainModule.GetType("ImportCM");
               
          MethodDefinition ReadMaterial = ImportCM.GetMethod("ReadMaterial");
          ReadMaterial.InjectWith(hooks.GetMethod("PmatHandler"));

            //add mod asset bundles to BgFiles

           

            MethodDefinition addbundlestobg = Prefab_manager.GetMethod("Addbundlestobg");

            for (int inst = 0; inst < init.Body.Instructions.Count; inst++)
            {
                if (init.Body.Instructions[inst].OpCode == OpCodes.Call)
                {
                    MethodReference target = init.Body.Instructions[inst].Operand as MethodReference;

                    if (target.Name == "UpdateFileSystemPath")
                    {


                        init.InjectWith(addbundlestobg, codeOffset: inst + 1);
                        break;

                    }
                }
            }


            // enable  override of PhotoBGObj prefabs
            

            MethodDefinition PhotBGObj_Instantiate = PhotoBGObjectData.GetMethod("Instantiate");
            MethodDefinition PhotBGObj_Instantiate_Ext = Prefab_manager.GetMethod("PhotBGObj_Instantiate_Ext");

            for (int inst = 0; inst < PhotBGObj_Instantiate.Body.Instructions.Count; inst++)
            {
                if (PhotBGObj_Instantiate.Body.Instructions[inst].OpCode == OpCodes.Ldstr && (string)PhotBGObj_Instantiate.Body.Instructions[inst].Operand == "Prefab/")
                {

                    for (int j = 0; j < PhotBGObj_Instantiate.Body.Variables.Count; j++)

                    {


                        if (PhotBGObj_Instantiate.Body.Variables[j].VariableType.FullName == "UnityEngine.Object") // get index of local variable of UnityEngine.Object type
                        {
                            PhotBGObj_Instantiate.InjectWith(PhotBGObj_Instantiate_Ext, codeOffset: inst + 6, flags: InjectFlags.PassInvokingInstance | InjectFlags.PassLocals, localsID: new[] { j });

                            break;
                        }
                    }

                    break;

                }
            }

            // the following two methods look like they're shouldn't bechanged in the future, so i should be fine with hardcoding code offset
            // and there shouldn't be need in calculating index of target @object local

            // create prefab override in Maid.AddPrefab which is for character prefabs such as cum particles
            MethodDefinition Maid_AddPrefab = assembly.MainModule.GetType("Maid").GetMethod("AddPrefab");
            MethodDefinition Maid_prefab_override = Prefab_manager.GetMethod("Maid_prefab_override");

            Maid_AddPrefab.InjectWith(Maid_prefab_override, 5, flags: InjectFlags.PassParametersVal | InjectFlags.PassLocals, localsID: new[] { 0 });

            // create prefab override in  BgMgr.AddPrefabToBg which is for background prefabs loaded
            // via scripts, such as dildobox

            MethodDefinition BgMgr_AddprefabToBg = assembly.MainModule.GetType("BgMgr").GetMethod("AddPrefabToBg");
            MethodDefinition BgMgr_prefab_override = Prefab_manager.GetMethod("BgMgr_prefab_override");
            BgMgr_AddprefabToBg.InjectWith(BgMgr_prefab_override,13, flags: InjectFlags.PassParametersVal | InjectFlags.PassLocals, localsID: new[] { 1 });
        }


    }
}
