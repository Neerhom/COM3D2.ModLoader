# COM3D2.ModLoader
Sybaris patcher for GameData-like functionality.

Since Sybaris currently isn't able to load any mods via Sybaris/Gamedata folder, this patcher was made to fix that, by giving such functionality to Mod folder.

.ks files or scripts, are handled by the game in a different manner, which has not been changed for COM3D2, as such they can be loaded from Sybaris/Gamedata

This enables loading of any files that game would call from it's .arc files from Mod folder, with exception of aforementioned .ks files.

I'm aware that [Sybaris 2.0](https://ux.getuploader.com/cm3d2_j/download/66#Qlq6Jr0.twitter_tweet_count_no_m) is a thing,
however, it doesn't run for me (Win7x64, with x64 opengl32.dll) and crashes COM. So if the patching porcess were to change (luckily it's the same as for Sybaris, according to readme) I can't provide any support for it.

The patcher also gives priority to the files loaded from Mod folder over those loaded from .arc files, meaning if two files have the same name the one from Mod will be loaded.

Folder structure inside Mod folder is irrelevant, so as long it's inside Mod it's going to work. 

Loading of .pmat files is supported, however their override is likely not, though I'm sure what's point of that would be


ModLoader also enables loading of Unity 5.6.4 Asset Bundles as Backgrounds and Background objects for Photo mode, assuming their files extension was changed to .asset_bg.

It also enables appending of data to photo_bg_list.nei and phot_bg_object_list.nei and their list_enabled counterparts.
See more details on [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/.asset_bg-files-and-what-to-do-with-them)

Because of this feature, ModLoader also comes with WORKING mirror props for Photo Mode!
Do note, that I have no claims over the scripts resposible for reflections, as it was a free asset from Unity store, which is no longer avaialbe. If you wish to use this script, simply decomplie the .dll files inside UnityInjector.



Pre-requisites:
[Sybaris](https://ux.getuploader.com/cm3d2_e/download/317)
[Mono.Cecil.Inject](https://github.com/denikson/Mono.Cecil.Inject/releases)

Installation:
Place Mono.Cecil.Inject into Sybaris/Loader

Place COM3D2.ModLoader.Patcher and COM3D2.ModLoader.Managed inside Sybaris/Loader

For Sybaris 2.0 the installation is that same, but you'd placing files into Sybaris folder instead. 

To enable Mirror Props put contents of Mod Folder along game's exe file, and Contents of UnityInjector, into Sybaris/Plugins/UnityInjector; for Sybaris 2.0 place contents of UnityInjector into Sybaris/UnityInjector

The initial version is written by [denikson](https://github.com/denikson) and i'm left with maintenance.
