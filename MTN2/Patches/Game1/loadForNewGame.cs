﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;
using MTN2.MapData;

namespace MTN2.Patches.Game1Patches {
    /// <summary>
    /// Patches the method Game1.loadForNewGame to allow the implementation
    /// of custom farms, overriding existing maps, and additional maps pertaining to
    /// the content pack of said custom farm.
    /// </summary>
    public class loadForNewGamePatch {

        private static CustomFarmManager farmManager;
        private static IMonitor Monitor;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="farmManager">The class controlling information pertaining to the custom farms (and the loaded farm).</param>
        /// <param name="Monitor">SMAPI's IMonitor, to print out useful information to user.</param>
        public loadForNewGamePatch(CustomFarmManager farmManager, IMonitor Monitor) {
            loadForNewGamePatch.farmManager = farmManager;
            loadForNewGamePatch.Monitor = Monitor;
        }

        /// <summary>
        /// Postfix method. Occurs after the original method of Game1.loadForNewGame is executed.
        /// 
        /// Loads the custom farm in, if it hasn't already (saved game is being loaded) into farmManager.
        /// Sets the Farm map with the correct map (the custom map that was requested).
        /// Loads additional maps that are apart of custom farm's content pack.
        /// Loads map overrides that are apart of custom farm's content pack.
        /// </summary>
        public static void Postfix() {
            int farmIndex;
            Map map;
            string mapAssetKey;

            if (farmManager.LoadedFarm == null) {
                farmManager.LoadCustomFarm(Game1.whichFarm);
            }

            if (!farmManager.Canon) {
                for (farmIndex = 0; farmIndex < Game1.locations.Count; farmIndex++) {
                    if (Game1.locations[farmIndex].Name == "Farm") break;
                }

                mapAssetKey = farmManager.GetAssetKey(out map);
                Game1.locations[farmIndex] = new Farm(mapAssetKey, "Farm");
            }

            //Loaded Farm Maps
            //Memory.farmMaps.Add(new additionalMap<Farm>("BaseFarm", "Farm", (Game1.whichFarm > 4) ? Memory.loadedFarm.farmMapType : fileType.xnb, "Farm", "Base Farm", Game1.getFarm()));

            if (!farmManager.Canon && farmManager.LoadedFarm.AdditionalMaps != null) {
                foreach (MapFile mf in farmManager.LoadedFarm.AdditionalMaps) {
                    object newMap;

                    if (mf.FileType == FileType.raw) {
                        map = farmManager.LoadMap(mf.FileName + ".tbin");
                    }

                    mapAssetKey = farmManager.GetAssetKey(mf.FileName, mf.FileType);

                    switch (mf.MapType) {
                        case "Farm":
                        case "FarmExpansion":
                        case "MTNFarmExtension":
                            newMap = new Farm(mapAssetKey, mf.Name);
                            Game1.locations.Add((Farm)newMap);
                            //Game1.locations.Add(new FarmExtension(mapAssetKey, m.Location, newMap as Farm));
                            //Memory.farmMaps.Add(new additionalMap<Farm>(m, Game1.locations.Last() as Farm));
                            break;
                        case "FarmCave":
                            newMap = new FarmCave(mapAssetKey, mf.Name);
                            Game1.locations.Add((FarmCave)newMap);
                            break;
                        case "GameLocation":
                            newMap = new GameLocation(mapAssetKey, mf.Name);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                        case "BuildableGameLocation":
                            newMap = new BuildableGameLocation(mapAssetKey, mf.Name);
                            Game1.locations.Add((BuildableGameLocation)newMap);
                            break;
                        default:
                            newMap = new GameLocation(mapAssetKey, mf.Name);
                            Game1.locations.Add((GameLocation)newMap);
                            break;
                    }
                    Monitor.Log("Custom map loaded. Name: " + (newMap as GameLocation).Name + " Type: " + newMap.ToString());
                }
            }

            if (!farmManager.Canon && farmManager.LoadedFarm.Overrides != null) {
                int i;
                foreach (MapFile mf in farmManager.LoadedFarm.Overrides) {
                    if (mf.FileType == FileType.raw) {
                        map = farmManager.LoadMap(mf.FileName + ".tbin");
                    }
                    mapAssetKey = farmManager.GetAssetKey(mf.FileName, mf.FileType);

                    for (i = 0; i < Game1.locations.Count; i++) {
                        if (Game1.locations[i].Name == mf.Name) break;
                    }

                    if (i >= Game1.locations.Count) {
                        Monitor.Log(String.Format("Unable to replace {0}, map was not found. Skipping", mf.Name), LogLevel.Warn);
                    } else {
                        switch (mf.Name) {
                            case "AdventureGuild":
                                Game1.locations[i] = new AdventureGuild(mapAssetKey, mf.Name);
                                break;
                            case "BathHousePool":
                                Game1.locations[i] = new BathHousePool(mapAssetKey, mf.Name);
                                break;
                            case "Beach":
                                Game1.locations[i] = new Beach(mapAssetKey, mf.Name);
                                break;
                            case "BusStop":
                                Game1.locations[i] = new BusStop(mapAssetKey, mf.Name);
                                break;
                            case "Club":
                                Game1.locations[i] = new Club(mapAssetKey, mf.Name);
                                break;
                            case "Desert":
                                Game1.locations[i] = new Desert(mapAssetKey, mf.Name);
                                break;
                            case "Forest":
                                Game1.locations[i] = new Forest(mapAssetKey, mf.Name);
                                break;
                            case "FarmCave":
                                Game1.locations[i] = new FarmCave(mapAssetKey, mf.Name);
                                break;
                            case "Mountain":
                                Game1.locations[i] = new Mountain(mapAssetKey, mf.Name);
                                break;
                            case "Railroad":
                                Game1.locations[i] = new Railroad(mapAssetKey, mf.Name);
                                break;
                            case "SeedShop":
                                Game1.locations[i] = new SeedShop(mapAssetKey, mf.Name);
                                break;
                            case "Sewer":
                                Game1.locations[i] = new Sewer(mapAssetKey, mf.Name);
                                break;
                            case "Town":
                                Game1.locations[i] = new Town(mapAssetKey, mf.Name);
                                break;
                            case "WizardHouse":
                                Game1.locations[i] = new WizardHouse(mapAssetKey, mf.Name);
                                break;
                            case "Woods":
                                Game1.locations[i] = new Woods(mapAssetKey, mf.Name);
                                break;
                            default:
                                Game1.locations[i] = new GameLocation(mapAssetKey, mf.Name);
                                break;
                        }
                        Monitor.Log("Map has been overridden with a custom map: " + mf.Name);
                    }
                }
            }
        }
    }
}
