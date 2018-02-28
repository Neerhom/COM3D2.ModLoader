# COM3D2.ModLoader
Sybaris patcher for GameData-like functionality.

Since Sybaris currently isn't able to load any mods via Sybaris/Gamedata folder, this patcher was made to fix that, by giving such
functionality to Mod folder.

.ks files or scripts, are handled by the game in a different manner, which has not been changed for COM3D2, as such they can be loaded from Sybaris/Gamedata

This enables loading of any files that game would call from it's .arc files from Mod folder, with exception of aforementioned .ks files.

The patcher also gives priority to the files loaded from Mod folder over those loaded from .arc files, meaning if two files have the same name the one from Mod will be loaded.

Folder structure inside Mod folder is irrelevant, so as long it's inside Mod it's going to work. The exception are .asset_bg files,
which must be placed inside Mod/bg folder.

Pre-requisites:
[Sybaris](https://ux.getuploader.com/cm3d2_e/download/317)
[Mono.Cecil.Inject](https://github.com/denikson/Mono.Cecil.Inject/releases)

Installation:
Place Mono.Cecil.Inject into Sybaris/Loader

Place COM3D2.ModLoader.Patcher and COM3D2.ModLoader.Managed inside Sybaris/Loader

The initial version is written by [denikson](https://github.com/denikson) and i'm left with maintenance.
