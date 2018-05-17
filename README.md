# COM3D2.ModLoader
Sybaris patcher for GameData-like functionality.

**Note. ModLoader version 1.4.2 and above, REQUIRE game version 1.11 or higher.**

Since Sybaris 2.1 currently isn't able to load any mods via Sybaris/Gamedata folder, this patcher was made to fix that, by giving such functionality to Mod folder.

This enables loading of any files that game would call from it's .arc files from Mod folder.

The patcher also enable loading of custom .arc files from Mod folder, but it should be unecessary.

While ModLoader is capable of loading .ks files from Mod folder (added in ver 1.5) the handling of those is a bit special and those curious can read about it on [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/Tech-behind-.arc-and-.ks-loading)

The patcher also gives priority to the files loaded from Mod folder over those loaded from .arc files, meaning if two files have the same name the one from Mod will be loaded.

Folder structure inside Mod folder is irrelevant, so as long it's inside Mod it's going to work. 

ModLoader also enables loading of Unity 5.6.4 Asset Bundles as Backgrounds and Background objects for Photo mode, assuming their files extension was changed to .asset_bg.

It also enables appending of data to a set of .nei files with disregard for their list_nabled.nei counterparts or ID values.
See more details on [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/.asset_bg-files-and-NEI-append)
You can also refer to this [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/Creating-.asset_bg-files) for a quick tutorial on how to create .asset_bg file and how to load them in COM3D2, using ModLoader

Because of this feature, ModLoader also comes with WORKING mirror props for Photo Mode!
Do note, that I have no claims over the scripts resposible for reflections, as it was a free asset from Unity store, which is no longer avaialbe. If you wish to use this script, simply decomplie the .dll files inside UnityInjector.

# Supporting patchers

# COM3D2.ProcScriptBinIgonre
Patcher prevents the game from throwing pesky "ProcScriptBin 例外 : ヘッダーファイルが不正です" errors and closing, when it thinks that file (menu, tex, mate, mod) header is incorrect.

# COM3D2.ModMenuAccel

A patcher that improves game startup times, by improving upond KISS' way of loading menu files from Mod folder.

The patcher also improves startup times (and even in-game load times, in few cases) by improving upon GetFileListAtExtension() method of both filesystem types, which make this patcher to be more than the name suggest, but since this functionality was added post-release, the name was kept as to simplify upgrade for the usesr. Credit for this bit of functionality goes to  はてな (twitter @hatena_37)

Pre-requisites:
 [Sybaris 2.1](https://ux.getuploader.com/cm3d2_j/download/68) or UnityDoorStop [SybarisLoader](https://github.com/NeighTools/SybarisLoader)

While [Sybaris 2.2](https://ux.getuploader.com/cm3d2_j/download/154) is also a possible option, the functionality is not confirmed nor it will be as I'm not fan of it.


Installation:

Put contents of Sybaris into Sybaris folder

To enable Mirror Props put contents of Mod Folder along game's exe file, and Contents of UnityInjector, into Sybaris/UnityInjector.

The initial version is written by [denikson](https://github.com/denikson) and i'm left with maintenance.
