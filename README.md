This repository contains my mods for ChilloutVR. Join the [ChilloutVR Modding Group discord](https://discord.gg/dndGPM3bxu) for support and more mods!  
Looking for more (universal) mods? [Check out my universal mods repository!](https://github.com/knah/ML-UniversalMods)

# **Don't want to install mods manually? [Check out the CVRMelonAssistant, an automatic mod installer!](https://github.com/knah/CVRMelonAssistant)**

## Alternatively, [read the manual installation section](#installation)

## Preface
Mods provided here are not made by or affiliated with Alpha Blend Interactive.  

Modifying ChilloutVR is allowed by its TOS as long as modding is non-malicious. As such, I expect these mods to be completely safe to use!  

However, if you encounter an issue with the game (especially after an update), **make sure that issue is not caused by mods before reporting it to ChilloutVR Team**.  
You can add `--no-mods` launch option (in Steam) to temporarily disable MelonLoader without uninstalling it completely.

Some of these mods are ports of my (now-dead) [VRChat mods](https://github.com/knah/VRCMods). You can take a peek there if you're curious about history.

## Lag Free Screenshots
A fork of this mod is currently maintained by @dakyneko [here](https://github.com/dakyneko/DakyModsCVR).

## MirrorResolutionUnlimiter
This mod adds a few extra options to fine-tune mirror look and performance. Doesn't actually change resolution limit yet.

Note that increasing mirror texture resolution will increase VRAM usage and lower performance, as your GPU will have to do more work.

If UI Expansion Kit is installed, mod settings page in the main menu will get two buttons to optimize and beautify all visible mirrors in the world.

Settings:
* Force auto resolution - removes mirror resolution limits set by world maker. Off by default.
* Mirror MSAA - changes MSAA specifically for mirrors. Valid values are 0 (same as main camera), 1, 2, 4 and 8. Lower MSAA may lead to "shimmering" and jaggies, especially in VR.
* Pixel lights in mirrors - allows to force enable or disable pixel lights in mirrors, changing how objects are affected by real-time lights

## UI Expansion Kit
This mod provides additional UI panels for use by other mods, and a unified mod settings UI.   
Refer to [API](UIExpansionKit/API) for mod integration.  
MirrorResolutionUnlimiter has an [example](MirrorResolutionUnlimiter/MirrorResolutionUnlimiterMod.cs) of soft dependency on this mod

This mod uses a watermelon emoji from [twemoji](https://github.com/twitter/twemoji) for settings page icon

## Installation
To install these mods, you will need to install [MelonLoader](https://discord.gg/2Wn3N2P) (discord link, see \#how-to-install).  
Then, you will have to put mod .dll files from [releases](https://github.com/knah/ChilloutMods/releases) into the `Mods` folder of your game directory

## Building
To build these, drop required libraries (found in `<cvr instanll dir>/ChilloutVR_Data/Managed` after melonloader installation, list found in `Directory.Build.props`) into Libs folder, apply a publicizer to them (such as [NStrip](https://github.com/BepInEx/NStrip)), then use your IDE of choice to build.
* Libs folder is intended for newest libraries (MelonLoader 0.4.0)

## License
With the following exceptions, all mods here are provided under the terms of [GNU GPLv3 license](LICENSE)
* UI Expansion Kit is additionally covered by [LGPLv3](UIExpansionKit/COPYING.LESSER) to allow other mods to link to it
* UI Expansion Kit uses a watermelon emoji from [twemoji](https://github.com/twitter/twemoji)
