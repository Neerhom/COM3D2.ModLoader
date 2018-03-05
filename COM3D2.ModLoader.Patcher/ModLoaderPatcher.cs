using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Inject;
using Mono.Cecil.Cil;

namespace COM3D2.ModLoader.Patcher
{
    public static class ModLoaderPatcher
    {
        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll" };
        private const string HOOK_NAME = "COM3D2.ModLoader.Managed";



        public static void Patch(AssemblyDefinition assembly)
        {
            int counter = 0;
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string hookDir = $"{HOOK_NAME}.dll";
            AssemblyDefinition hookAssembly = AssemblyLoader.LoadAssembly(Path.Combine(assemblyDir, hookDir));

            TypeDefinition gameUty = assembly.MainModule.GetType("GameUty");
            TypeDefinition hooks = hookAssembly.MainModule.GetType($"{HOOK_NAME}.Hooks");

            MethodDefinition init = gameUty.GetMethod("Init");
            MethodDefinition swapFileSystems = hooks.GetMethod("SwapFileSystems");

            init.InjectWith(swapFileSystems, 2); // Patch right after the file system was initially set
            gameUty.ChangeAccess("m_FileSystem", true, false); // Make original file system public to allow replacing
            gameUty.ChangeAccess("m_ModFileSystem", true, false); // Make the mod file system public

            // enablie nei append to PhotBGData
            TypeDefinition PhotoBGData = assembly.MainModule.GetType("PhotoBGData");
            PhotoBGData.ChangeAccess("bg_data_", true, false); // make public to allow hook access
            MethodDefinition PhotoBGDatCreate = PhotoBGData.GetMethod("Create");
            MethodDefinition PhotoBgExt = hooks.GetMethod("PhotoBGext");

            for (int inst = 0; inst < PhotoBGDatCreate.Body.Instructions.Count; inst++)

            {
                if (PhotoBGDatCreate.Body.Instructions[inst].OpCode == OpCodes.Endfinally)
                {
                    counter += 1;
                    if (counter == 2)
                    {
                        PhotoBGDatCreate.InjectWith(PhotoBgExt, inst+1);
                        break;
                    }
                }
            }

            // enable nei append to PhotBGObjectData
            TypeDefinition PhotoBGObjectData = assembly.MainModule.GetType("PhotoBGObjectData");
            PhotoBGObjectData.ChangeAccess("bg_data_",true, false); // make public to allow hook access
            MethodDefinition PhotoBobjectDataCreate = PhotoBGObjectData.GetMethod("Create");
            MethodDefinition PhotoBGobjext = hooks.GetMethod("PhotoBGobjext");

            counter = 0;
            for (int inst = 0; inst < PhotoBobjectDataCreate.Body.Instructions.Count; inst++)

            {
                if (PhotoBobjectDataCreate.Body.Instructions[inst].OpCode == OpCodes.Endfinally)
                {
                    counter += 1;
                    if (counter == 2)
                    {
                        PhotoBobjectDataCreate.InjectWith(PhotoBGobjext, inst + 1);
                        break;
                    }
                }
            }

            // enable PhotoBGObj to use asset bundles

            MethodDefinition PhotBGObj_Instantiate = PhotoBGObjectData.GetMethod("Instantiate");
            MethodDefinition PhotBGObj_Instantiate_Ext = hooks.GetMethod("PhotBGObj_Instantiate_Ext");

            for (int inst = 0; inst < PhotBGObj_Instantiate.Body.Instructions.Count; inst++)
            {
                if (PhotBGObj_Instantiate.Body.Instructions[inst].OpCode == OpCodes.Ret)
                {
                    PhotBGObj_Instantiate.InjectWith(PhotBGObj_Instantiate_Ext, codeOffset: inst-5, flags: InjectFlags.PassInvokingInstance | InjectFlags.PassLocals, localsID: new[] { 2 });
                    break;
                }
            }
            //add mod asset bundles to BgFiles
            MethodDefinition UpdateFileSystemPath = gameUty.GetMethod("UpdateFileSystemPath");
            MethodDefinition addbundlestobg = hooks.GetMethod("addbundlestobg");
            counter = 0;
            for (int inst = 0; inst < UpdateFileSystemPath.Body.Instructions.Count; inst++)
            {
                if (UpdateFileSystemPath.Body.Instructions[inst].OpCode == OpCodes.Blt)
                {
                    counter += 1;
                    if (counter == 3)
                    {
                        UpdateFileSystemPath.InjectWith(addbundlestobg, codeOffset: inst + 1);
                        break;
                    }
                }
            }

        }
    }
}
