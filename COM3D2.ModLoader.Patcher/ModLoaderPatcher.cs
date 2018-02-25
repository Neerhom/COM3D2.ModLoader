using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Inject;

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

            TypeDefinition gameUty = assembly.MainModule.GetType("GameUty");
            TypeDefinition hooks = hookAssembly.MainModule.GetType($"{HOOK_NAME}.Hooks");

            MethodDefinition init = gameUty.GetMethod("Init");
            MethodDefinition swapFileSystems = hooks.GetMethod("SwapFileSystems");

            init.InjectWith(swapFileSystems, 2); // Patch right after the file system was initially set
            gameUty.ChangeAccess("m_FileSystem", true, false); // Make original file system public to allow replacing
            gameUty.ChangeAccess("m_ModFileSystem", true, false); // Make the mod file system public
        }
    }
}
