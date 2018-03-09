using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Mono.Cecil.Inject;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace COM3D2.ProcScriptBinIgonre.Patcher
{
    public static class ProcScriptBinIgonre
    {



        public static readonly string[] TargetAssemblyNames = { "Assembly-CSharp.dll" };

        //cut a number of instructions starting at and including specified position
        public static void InstCutter(MethodDefinition method, int cutpos, int cutN)
        {


            for (int i = 0; i < cutN; i++)
            {

                method.Body.Instructions.RemoveAt(cutpos);

            }
        }

        public static void Patch ( AssemblyDefinition assembly)
        {

            // remove call to Ndebug.Assert in SceneEdit.InitMenuItemScript
            // this thing is responsible for Maid Edit game crash
            TypeDefinition SceneEdit = assembly.MainModule.GetType("SceneEdit");
            MethodDefinition InitMenuItemScript = SceneEdit.GetMethod("InitMenuItemScript");

            for (int inst = 0; inst < InitMenuItemScript.Body.Instructions.Count; inst++)
            {

                if (InitMenuItemScript.Body.Instructions[inst].OpCode == OpCodes.Ldstr)

                {
                    string target = InitMenuItemScript.Body.Instructions[inst].Operand as string;

                    if (target == "CM3D2_MENU")
                    {
                        InstCutter(InitMenuItemScript, inst - 1, 7);

                    }

                }

            }

            // remove call to Ndebug.Assert in SceneEdit.InitModMenuItemScript
            // not sure about this one, as this one is likely for .mod files, which should be headache of KISS, but whatever
            MethodDefinition InitModMenuItemScript = SceneEdit.GetMethod("InitModMenuItemScript");

            for (int inst = 0; inst < InitModMenuItemScript.Body.Instructions.Count; inst++)
            {

                if (InitModMenuItemScript.Body.Instructions[inst].OpCode == OpCodes.Ldstr)

                {
                    string target = InitModMenuItemScript.Body.Instructions[inst].Operand as string;

                    if (target == "CM3D2_MOD")
                    {
                        InstCutter(InitModMenuItemScript, inst - 1, 7);

                    }

                }

            }



            // remove call to Ndebug.Assert in ImportCM.LoadMaterial() and ImportCM.LoadTexture()
            // Not sure if there is a point in murdering these

            TypeDefinition ImportCM = assembly.MainModule.GetType("ImportCM");

            MethodDefinition LoadMaterial = ImportCM.GetMethod("LoadMaterial");


            for (int inst = 0; inst < LoadMaterial.Body.Instructions.Count; inst++)
            {

                if (LoadMaterial.Body.Instructions[inst].OpCode == OpCodes.Ldstr)

                {
                    string target = LoadMaterial.Body.Instructions[inst].Operand as string;

                    if (target == "CM3D2_MATERIAL")
                    {
                        InstCutter(LoadMaterial, inst +3, 5);

                    }

                }

            }

            MethodDefinition LoadTexture = ImportCM.GetMethod("LoadTexture");


            for (int inst = 0; inst < LoadTexture.Body.Instructions.Count; inst++)
            {

                if (LoadTexture.Body.Instructions[inst].OpCode == OpCodes.Ldstr)

                {
                    string target = LoadTexture.Body.Instructions[inst].Operand as string;

                    if (target == "CM3D2_TEX")
                    {
                        InstCutter(LoadTexture, inst + 3, 5);

                    }

                }

            }




            // remove call to Ndebug.Assert in Menu.ProcScriptBin
            // Not sure if there is a point in murdering this one
            TypeDefinition Menu = assembly.MainModule.GetType("Menu");
            MethodDefinition ProcScriptBin = Menu.GetMethod("ProcScriptBin");

            
            for (int inst = 0; inst < ProcScriptBin.Body.Instructions.Count; inst++)
            {

                if (ProcScriptBin.Body.Instructions[inst].OpCode == OpCodes.Ldstr)

                {
                    string target = ProcScriptBin.Body.Instructions[inst].Operand as string;

                    if (target == "CM3D2_MENU")
                    {
                        InstCutter(ProcScriptBin, inst-1, 7);

                    }
                    
                }
                
            }


            // remove call to Ndebug.Assert in Menu.ProcModScriptBin
            // Not sure if there is a point in murdering this one
          
            MethodDefinition ProcModScriptBin = Menu.GetMethod("ProcModScriptBin");


            for (int inst = 0; inst < ProcModScriptBin.Body.Instructions.Count; inst++)
            {

                if (ProcModScriptBin.Body.Instructions[inst].OpCode == OpCodes.Ldstr)

                {
                    string target = ProcModScriptBin.Body.Instructions[inst].Operand as string;

                    if (target == "CM3D2_MOD")
                    {
                        InstCutter(ProcModScriptBin, inst - 1, 7);

                    }

                }

            }








        }

    }
}
