using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Inject;
using Mono.Cecil.Cil;
using System.Reflection;
using System.IO;

namespace COM3D2.ModMenuAccel
{
    public class ModMenuAccel_Patcher
    {
        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll" };
        private const string HOOK_NAME = "COM3D2.ModMenuAccel.Hook";



        public static void Patch(AssemblyDefinition assembly)
        {
            
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string hookDir = $"{HOOK_NAME}.dll";
            AssemblyDefinition hookAssembly = AssemblyLoader.LoadAssembly(Path.Combine(assemblyDir, hookDir));
            TypeDefinition ModmenuAccel = hookAssembly.MainModule.GetType($"{HOOK_NAME}.ModmenuAccel");




            TypeDefinition GameUty = assembly.MainModule.GetType("GameUty");
            GameUty.ChangeAccess("m_aryMenuFiles", true);
            GameUty.ChangeAccess("m_aryModOnlysMenuFiles", true);
            GameUty.ChangeAccess("rid_menu_dic_", true);

            MethodDefinition UpdateFileSystemPath = GameUty.GetMethod("UpdateFileSystemPath");

            MethodDefinition Accel = ModmenuAccel.GetMethod("Accel");
      

            for (int i = 0; i < UpdateFileSystemPath.Body.Instructions.Count; i++)
            {
                if (UpdateFileSystemPath.Body.Instructions[i].OpCode == OpCodes.Stsfld)
                {
                    FieldReference target = UpdateFileSystemPath.Body.Instructions[i].Operand as FieldReference;
                    if (target.Name == "m_aryMenuFiles")
                    {
                        UpdateFileSystemPath.InjectWith(Accel, i + 1, flags: InjectFlags.ModifyReturn);
                        break;
                    }

                }
            }
        }
    }
}
