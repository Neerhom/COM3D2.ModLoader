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
        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll" };
        private const string HOOK_NAME = "COM3D2.ModMenuAccel.Hook";



        public static void Patch(AssemblyDefinition assembly)
        {

            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string hookDir = $"{HOOK_NAME}.dll";
            AssemblyDefinition hookAssembly = AssemblyLoader.LoadAssembly(Path.Combine(assemblyDir, hookDir));
            if (assembly.Name.Name == "Assembly-CSharp")
            {
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
            else
            {
                TypeDefinition FastStart = hookAssembly.MainModule.GetType($"{HOOK_NAME}.FastStart");
                // add hook to FileSystemArchive.GetFileListAtExtension(); pass paramter of the target, if parameter == "menu", the hook will call return for target after execution
                MethodDefinition FS_archive_GetFileListAtExtension = assembly.MainModule.GetType("FileSystemArchive").GetMethod("GetFileListAtExtension");
                MethodDefinition FileSystemArchiveGetFileListAtExtension = FastStart.GetMethod("FileSystemArchiveGetFileListAtExtension");

                FS_archive_GetFileListAtExtension.InjectWith(FileSystemArchiveGetFileListAtExtension, flags: InjectFlags.ModifyReturn | InjectFlags.PassParametersVal);

                // add hook to FileSystemWindows.AddFolder () and pass invokin (this) to it
                TypeDefinition FileSystemWindows = assembly.MainModule.GetType("FileSystemWindows");
                MethodDefinition FS_win_AddFolder = FileSystemWindows.GetMethod("AddFolder");
                MethodDefinition FileSystemWindowsAddFolderPost = FastStart.GetMethod("FileSystemWindowsAddFolderPost");

                FS_win_AddFolder.InjectWith(FileSystemWindowsAddFolderPost, 4, flags: InjectFlags.PassInvokingInstance); // since this method is very unlikely to be changed, i should be able to get away with hardocing code offset, which is righ after call instruction

                // add hook to FileSystemWindows.AddAutoPath; and modify return, so it woul always return true
                MethodDefinition FS_win_AddAutoPath = FileSystemWindows.GetMethod("AddAutoPath");
                MethodDefinition FileSystemWindowsAddAutoPath = FastStart.GetMethod("FileSystemWindowsAddAutoPath");
                FS_win_AddAutoPath.InjectWith(FileSystemWindowsAddAutoPath, flags: InjectFlags.ModifyReturn);

                // add hook to FileSystemWindows.GetFileListAtExtension(), pass parameter to it and modify return if custom method doesn't fail
                MethodDefinition FS_win_GetFileListAtExtension = FileSystemWindows.GetMethod("GetFileListAtExtension");
                MethodDefinition FileSystemWindowsGetFileListAtExtension = FastStart.GetMethod("FileSystemWindowsGetFileListAtExtension");

                FS_win_GetFileListAtExtension.InjectWith(FileSystemWindowsGetFileListAtExtension, flags: InjectFlags.ModifyReturn | InjectFlags.PassParametersVal);

            }




        }

    }

}
