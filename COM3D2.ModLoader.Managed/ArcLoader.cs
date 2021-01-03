using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CM3D2.Toolkit.Arc;
using CM3D2.Toolkit.Arc.Entry;
using CM3D2.Toolkit.Arc.FilePointer;

namespace COM3D2.ModLoader.Managed
{
    public static class ArcLoader
    {
        
            public static void Install()
            {
                // get gamepath and Mod folder
                string gamepath = Path.GetFullPath(".\\");
                string ModFolder = Path.Combine(gamepath, "Mod");


                ArcFileSystem fs = new ArcFileSystem();

                // Iterate through each file in Mod folder and add it to the proxy ARC file
                foreach (string file in Directory.GetFiles(ModFolder, "*.ks", SearchOption.AllDirectories))
                {
                    string name = Path.GetFileName(file);

                    ArcFileEntry arcFile = !fs.FileExists(file) ? fs.CreateFile(name) : fs.GetFile(file);
                    arcFile.Pointer = new WindowsFilePointer(file);
                }

                foreach (string file in Directory.GetFiles(ModFolder, "*.ogg", SearchOption.AllDirectories))
                {
                    string name = Path.GetFileName(file);

                    ArcFileEntry arcFile = !fs.FileExists(file) ? fs.CreateFile(name) : fs.GetFile(file);
                    arcFile.Pointer = new WindowsFilePointer(file);
                }

            // Save the ARC file on disk because the game does not support loading ARC from meory easily
            // the temp ARC is saved in ML_temp folder in game's folder, so as to not get in the way of general mods
            if (!Directory.Exists(Path.Combine(gamepath, "ML_temp")))
                    Directory.CreateDirectory(Path.Combine(gamepath, "ML_temp"));

                using (FileStream fStream = File.Create(Path.Combine(gamepath, "ML_temp\\ML_temp.arc")))
                {
                    fs.Save(fStream);
                }

                // Get the game's file system and add our custom ARC file
                FileSystemArchive gameFileSystem = GameUty.FileSystem as FileSystemArchive;

                gameFileSystem.AddArchive(Path.Combine(gamepath, "ML_temp\\ML_temp.arc"));

                // load custom arcs, cuz yolo

                foreach (string arc in Directory.GetFiles(ModFolder, "*.arc", SearchOption.AllDirectories))
                {
                    gameFileSystem.AddArchive(arc);
                }

            }
        
    }
}
