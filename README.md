# COM3D2.ModLoader
Sybaris patcher for GameData-like functionality.

**Note. ModLoader version 1.7 and above, REQUIRE game version 1.48 or higher.**

Since Sybaris 2.1 currently isn't able to load any mods via Sybaris/Gamedata folder, this patcher was made to fix that, by giving such functionality to Mod folder.

This enables loading of any files that game would call from it's .arc files from Mod folder.

The patcher also enable loading of custom .arc files from Mod folder, but it should be unecessary.

ModLoader is capable of loading **.ks** and **.ogg** files from Mod folder (.ks support added in ver 1.5; .ogg since 1.7.2). The handling of those is a bit special and those curious can read about it on [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/Tech-behind-.arc-and-.ks-loading)

The patcher also gives priority to the files loaded from Mod folder over those loaded from .arc files, meaning if two files have the same name the one from Mod will be loaded.

Since version 1.6.2 ModLoader enables use of .asset_bg files to override game's prefabs, namely: Photo Mode background objects, background objects called via game's script files and character prefabs (particle effects, such as cum, you see during sex). Same method can also be applied to override in-game backgrounds, though this was possible for a while ever since loading of .asset_bg files, because game was coded so...

Folder structure inside Mod folder is irrelevant, so as long it's inside Mod it's going to work. 

ModLoader also enables loading of Unity 5.6.4 Asset Bundles as Backgrounds and Background objects for Photo mode, assuming their files extension was changed to .asset_bg.

It also enables appending of data to a set of .nei files with disregard for their list_nabled.nei counterparts or ID values.
See more details on [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/.asset_bg-files-and-NEI-append)
You can also refer to this [wiki page](https://github.com/Neerhom/COM3D2.ModLoader/wiki/Creating-.asset_bg-files) for a quick tutorial on how to create .asset_bg file and how to load them in COM3D2, using ModLoader

Because of this feature, ModLoader also comes with WORKING mirror props for Photo Mode!
Do note, that I have no claims over the scripts resposible for reflections, as it was a free asset from Unity store, which is no longer avaialbe. If you wish to use this script, simply decomplie the .dll files inside UnityInjector.

With introduction for nei append to desk item data, ModloAder also comes with Extra Desk Items mod, which adds few food items from Photomode, to be used in desk customization. And they're even sorted by categories!

**Due to cahnges in game version 1.48, ModLoader 1.7.1 handles loading of master/man mods**

A menu file that contains "mhead" or "mbody" in the filname is considered as master/man//player menu and will display it in master edit and photomode.

# Supporting patchers

# COM3D2.ProcScriptBinIgonre
Patcher prevents the game from throwing pesky "ProcScriptBin 例外 : ヘッダーファイルが不正です" errors and closing, when it thinks that file (menu, tex, mate, mod) header is incorrect.

# COM3D2.ModMenuAccel

A patcher that improves game startup times, by improving upond KISS' way of loading menu files from Mod folder.

The patcher also improves startup times (and even in-game load times, in few cases) by improving upon GetFileListAtExtension() method of both filesystem types, which make this patcher to be more than the name suggest, but since this functionality was added post-release, the name was kept as to simplify upgrade for the usesr. Credit for this bit of functionality goes to  はてな (twitter @hatena_37)

# COM3D2.AssetCleaner

A ptacher that fixed bug in COM3D2 which coudl result in unecessary high VRAM and RAM usage due to game not unloading resources on switching BackGrounds. This was mainly an issue for Edit and Photo/Studio mode. Since it modifies BgMgr.ChangeBG() method this will also result in less VRAM&RAM usage when changing BG via plugins such SceneCapture and MultipleMaids

# Pre-requisites
ModLoader requires any plugin/patcher loader that is capable of loading Sybaris patchers. 
For COM3D2 following are known:

[Sybaris 2.1](https://ux.getuploader.com/cm3d2_j/download/68)

[BepinEX 4.0 or newer](https://github.com/BepInEx/BepInEx) (personal recommendation, yes I'm biased towards it)

[Sybaris 2.2](https://ux.getuploader.com/cm3d2_j/download/154) is also a possible option, the functionality is not confirmed nor it will be as I'm not fan of it.


# Installation:

Put contents of Sybaris into Sybaris folder

To enable Mirror Props put contents of Mod Folder along game's exe file, and Contents of UnityInjector, into Sybaris/UnityInjector.

The initial version is written by [denikson](https://github.com/denikson) and i'm left with maintenance.
