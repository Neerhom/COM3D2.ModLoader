using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Inject;
using Mono.Cecil.Cil;
using UnityEngine;
using System.Linq;

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
            try
            {
                init.InjectWith(swapFileSystems, 2); // Patch right after the file system was initially set
            }
            catch (Exception e)

            {
                Console.WriteLine("ModLoader:Failed to inject into method GameUty.Init");
                Console.WriteLine(e);
            }

            gameUty.ChangeAccess("m_FileSystem", true, false); // Make original file system public to allow replacing
            gameUty.ChangeAccess("m_ModFileSystem", true, false); // Make the mod file system public

            // hook in the ArcLoader to enable loading of ARCs and .ks files
            try
            {
                init.InjectWith(hookAssembly.MainModule.GetType($"{HOOK_NAME}.ArcLoader").GetMethod("Install"), -1);
            }
            catch (Exception e)
            {
                Console.WriteLine("ModLoader:Failed to inject into method GameUty.Init");
                Console.WriteLine(e);
            }



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
                        try
                        {
                            PhotoBGDataCreate.InjectWith(PhotoBgExt, inst);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ModLoader:Failed to inject into method PhotoBGData.Create");
                            Console.WriteLine(e);
                        }
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
                        try
                        {
                            PhotoBobjectDataCreate.InjectWith(PhotoBGobjext, inst - 1);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ModLoader:Failed to inject into method PhotoBGObjectData.Create");
                            Console.WriteLine(e);
                        }
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
                    FieldDefinition target = PhotoMotionDataCreate.Body.Instructions[inst].Operand as FieldDefinition;
                    if (target.Name == "CheckModFile")
                    {
                        try
                        {
                            PhotoMotionDataCreate.InjectWith(PhotMotExt, inst - 2);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ModLoader:Failed to inject into method PhotoMotionData.Create");
                            Console.WriteLine(e); ;
                        }
                        break;
                    }
                }
            }
            // nei appen to deskmanager data

            TypeDefinition Deskmanager = assembly.MainModule.GetType("DeskManager");
            MethodDefinition Deskmanager_CreateCsvData = Deskmanager.GetMethod("CreateCsvData");
            MethodDefinition DeskData_Ext = hooks.GetMethod("DeskData_Ext");
            TypeDefinition Deskmanager_Itemdata = Deskmanager.NestedTypes.First(t => t.Name == "ItemData");
            Deskmanager_Itemdata.ChangeAccess("id", true,false, true);//
            Deskmanager.ChangeAccess("item_detail_data_dic_", true); // make public to allow hook access
            Deskmanager.ChangeAccess("item_category_data_dic_", true);//
            Deskmanager.IsPublic = true;//

            for (int i = 0; Deskmanager_CreateCsvData.Body.Instructions.Count > i; i++)
            {
                if (Deskmanager_CreateCsvData.Body.Instructions[i].OpCode == OpCodes.Ldsfld)
                {
                    FieldDefinition target = Deskmanager_CreateCsvData.Body.Instructions[i].Operand as FieldDefinition;

                    if (target.Name == "item_inst_data_")
                    {
                        try
                        {
                            Deskmanager_CreateCsvData.InjectWith(DeskData_Ext, i);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ModLoader:Failed to inject into method DeskManager.CreateCsvData");
                            Console.WriteLine(e); ;
                        }
                        break;
                    }
                }

            }

            

            // patch in PmatHandler for loading of mod pmat files, override of base .pmat files and handling of pmat hash conflicts

            TypeDefinition ImportCM = assembly.MainModule.GetType("ImportCM");
               
          MethodDefinition ReadMaterial = ImportCM.GetMethod("ReadMaterial");
            try
            {
                ReadMaterial.InjectWith(hooks.GetMethod("PmatHandler"));
            }
            catch (Exception e)
            {
                Console.WriteLine("ModLoader:Failed to inject into method ImportCM.ReadMaterial");
                Console.WriteLine(e);
            }

            //add mod asset bundles to BgFiles



            MethodDefinition addbundlestobg = Prefab_manager.GetMethod("Addbundlestobg");

            for (int inst = 0; inst < init.Body.Instructions.Count; inst++)
            {
                if (init.Body.Instructions[inst].OpCode == OpCodes.Call)
                {
                    MethodReference target = init.Body.Instructions[inst].Operand as MethodReference;

                    if (target.Name == "UpdateFileSystemPath")
                    {

                        try
                        {
                            init.InjectWith(addbundlestobg, codeOffset: inst + 1);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ModLoader:Failed to inject into method Gameuty.Init");
                            Console.WriteLine(e);
                        }
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
                            try
                            {
                                PhotBGObj_Instantiate.InjectWith(PhotBGObj_Instantiate_Ext, codeOffset: inst + 6, flags: InjectFlags.PassInvokingInstance | InjectFlags.PassLocals, localsID: new[] { j });
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("ModLoader:Failed to inject into method PhotoBGObj.Instantiate");
                                Console.WriteLine(e);
                            }
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
            try
            {
                Maid_AddPrefab.InjectWith(Maid_prefab_override, 5, flags: InjectFlags.PassParametersVal | InjectFlags.PassLocals, localsID: new[] { 0 });
            }
            catch (Exception e)
            {
                Console.WriteLine("ModLoader:Failed to inject into method Maid.AddPrefab");
                Console.WriteLine(e);
            }
            // create prefab override in  BgMgr.AddPrefabToBg which is for background prefabs loaded
            // via scripts, such as dildobox
            TypeDefinition BgMgr = assembly.MainModule.GetType("BgMgr");
            BgMgr.ChangeAccess("m_dicAttachObj",true); // make public to allow hook access
            MethodDefinition BgMgr_AddprefabToBg = BgMgr.GetMethod("AddPrefabToBg");
            MethodDefinition BgMgr_prefab_override = Prefab_manager.GetMethod("BgMgr_prefab_override");
            MethodReference BgMgr_prefab_override_ref= assembly.MainModule.Import(BgMgr_prefab_override);
            
                        
            for (int i = 0; i < BgMgr_AddprefabToBg.Body.Instructions.Count; i++)
            {
                if (BgMgr_AddprefabToBg.Body.Instructions[i].OpCode == OpCodes.Ldftn)
                {
                     BgMgr_AddprefabToBg.Body.Instructions[i].Operand = BgMgr_prefab_override_ref; // 1.28 change. replace new delegate with custom method
                   
                }
            }

    
            // create prefab override in desk manager on change bg, to allow override of prefab used for desk items
            // this bit of functionality is unlikely to be used, but it's mostly for feature-completion


            MethodDefinition Deskmanager_OnChangeBg = Deskmanager.GetMethod("OnChangeBG");
            MethodDefinition DeskManger_OnchangeBg_EXT = Prefab_manager.GetMethod("DeskManger_OnchangeBg_EXT");

            
            for (int i = 0; Deskmanager_OnChangeBg.Body.Instructions.Count > i; i++)
            {
                if (Deskmanager_OnChangeBg.Body.Instructions[i].OpCode == OpCodes.Ldstr && (string)Deskmanager_OnChangeBg.Body.Instructions[i].Operand == "Prefab/")
                {
                    for (int j = 0; j < Deskmanager_OnChangeBg.Body.Variables.Count; j++)
                    {
                     
                        if (Deskmanager_OnChangeBg.Body.Variables[j].VariableType.FullName == "DeskManager/InstansData")
                        {
                            try
                            {
                                Deskmanager_OnChangeBg.InjectWith(DeskManger_OnchangeBg_EXT, i + 8, flags: InjectFlags.PassLocals, localsID: new[] { j, j + 1 });
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("ModLoader:Failed to inject into method DEskmanager.OnChangeBG");
                                Console.WriteLine(e);
                            }
                            break;
                                
                        }
                    }
                    break;
                }
                  
            }
        }


    }
}

