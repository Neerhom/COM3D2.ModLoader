# COM3D2.ModLoader
Sybarys patcher for GameData-like functionality.


Since Sybaris currently isn't able to load any mods via Sybaris/Gamedate folder, this patcher was made to fix that, by giving such 
functionaly to Mod folder.

This enable loading of any files that game would call from it's .arc files from Mod folder, with exception of .asset_bg files,
as code to load them isn't there yet. When future update enable loading of .asset_bg files, expect an update to allow loading of them as well.

The partcher also give priority to the files loaded from Mod folder over those loaded from .arc files, meaning if two files have the same name
the one from Mow will beloaded.

Folder structure inse Mod folder is irrelevant, so as long it's inside Mod it's going to work.


The initial version is written by [denikson](https://github.com/denikson) and i'm left with maintenance.
