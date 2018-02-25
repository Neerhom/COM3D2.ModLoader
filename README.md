# COM3D2.ModLoader
Sybarys patcher for GameData-like functionality.


Since Sybaris currently isn't able to load any mods via Sybaris/Gamedate folder, this patcher was made to fix that, by giving such 
functionaly to Mod folder.

This enable loading of any files that game would call from it's .arc files from Mod folder, with exception of .asset_bg files,
as code to load them isn't there yet. When future update enable loading of .asset_bg files, expect an update to allow loading of them as well.

The partcher also give priority to the files loaded from Mod folder over those loaded from .arc files, meaning if two files have the same name the one from Mod will be loaded.

Folder structure inside Mod folder is irrelevant, so as long it's inside Mod it's going to work.

Pre-requisites:
[Sybaris](https://ux.getuploader.com/cm3d2_e/download/317)
[Mono.Cecil.Inject](https://github.com/denikson/Mono.Cecil.Inject/releases)

Installation:
Place Mono.Cecil.Inject into Sybaris/Loader

Place COM3D2.ModLoader.Patcher and COM3D2.ModLoader.Managed inside Sybaris/Loader

The initial version is written by [denikson](https://github.com/denikson) and i'm left with maintenance.
