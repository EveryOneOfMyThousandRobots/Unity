﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

using NoYouDoIt.TheWorld;
using NoYouDoIt.Utils;
using Unity.Jobs;
using Unity.Collections;

namespace NoYouDoIt.DataModels {

  //public struct StockPileJob : IJob {
  //  public NativeHashMap<int,int> naStockPileSettings;



  //  public void Execute() {

  //  }
  //}

  public class StockPileSetting {
    private static int ID = 0;
    public int id { get; private set; }
    public InventoryItem item;
    public string name { get; private set; }
    public int currentQty;
    public int maxQty;
    public int allocatedQty;
    public bool all = false;
    public int stackSize { get; private set; }
    public bool pendingWork = false;




    public StockPileSetting(InventoryItem item) {
      this.item = item;
      this.name = item.type;
      this.stackSize = InventoryItem.GetStackSize(this.name);
      this.maxQty = this.stackSize;
      StockPileSetting.ID += 1;
      this.id = ID;


    }
  }



  public class InventoryManager {
    //StockPileJob job;
    private World world;

    public Dictionary<string, StockPileSetting> stockpileSettings;
    public NYDIList<StockPileSetting> StockpileSettingsList;
    public Dictionary<string, StockPileSetting> looseQtys;
    public NYDIList<StockPileSetting> LooseSettingsList;
    private StockPileSetting[] stockpileSettingsArray;
    //private int spsIndex = 0;

    List<NYDITimer> timers;

    //public Dictionary<string, List<InventoryItem>> inventories;
    public List<Inventory> inventories;
    public List<Inventory> stockpiles;
    public NYDIList<Inventory> stockpilesList;
    //private List<InventoryItem> items;
    public InventoryManager(World world) {
      this.world = world;
      inventories = new List<Inventory>();//new Dictionary<string, List<InventoryItem>>();
      stockpileSettings = new Dictionary<string, StockPileSetting>();
      looseQtys = new Dictionary<string, StockPileSetting>();
      timers = new List<NYDITimer>();
      //timers.Add(new NYDITimer("updateTimer", 2, UpdateStockPileSettings));
      timers.Add(new NYDITimer("setStockpiles", 5, UpdateStockPileList));
      stockpiles = new List<Inventory>();
      stockpilesList = new NYDIList<Inventory>();

      StockpileSettingsList = new NYDIList<StockPileSetting>();
      LooseSettingsList = new NYDIList<StockPileSetting>();
    }

    public void UpdateStockPileList() {
      // stockpiles = inventories.Where(e => e.IsStockPile).ToList<Inventory>();
    }

    public void RegisterStockpile(InstalledItem item) {
      Inventory inv = item.tile.InventoryGetRef();
      if (!stockpiles.Contains(inv)) {
        stockpiles.Add(inv);
      }
      stockpilesList.Add(inv);


    }


    public void UnregisterStockpile(InstalledItem item) {
      Inventory inv = item.tile.InventoryGetRef();
      if (stockpiles.Contains(inv)) {
        stockpiles.Remove(inv);
      }
      stockpilesList.Remove(inv);


    }

    public void SetStockPileSettingWork(string name, bool b) {
      stockpileSettings[name].pendingWork = b;

    }

    public void SetStockpileQty(string name, int qty) {
      if (stockpileSettings.ContainsKey(name)) {
        stockpileSettings[name].currentQty = qty;
      }
    }

    public void SetLooseQty(string name, int qty) {
      if (looseQtys.ContainsKey(name)) {
        looseQtys[name].currentQty = qty;
      }
    }

    public void AddStockpileQty(string name, int qty) {
      if (stockpileSettings.ContainsKey(name)) {
        stockpileSettings[name].currentQty += qty;
      }
    }
    public void AddAllocatedStockpileQty(string name, int qty) {
      if (stockpileSettings.ContainsKey(name)) {
        stockpileSettings[name].allocatedQty += qty;
      }
    }

    public void AddAllocatedLooseQty(string name, int qty) {
      if (looseQtys.ContainsKey(name)) {
        looseQtys[name].allocatedQty += qty;
        if (looseQtys[name].allocatedQty < 0) {
          looseQtys[name].allocatedQty = 0;
        }
      }
    }

    public void AddLooseQty(string name, int qty) {
      if (looseQtys.ContainsKey(name)) {
        looseQtys[name].currentQty += qty;
      }
    }

    public void RegisterInventory(Inventory inventory) {
      if (!inventories.Contains(inventory)) {
        inventories.Add(inventory);
      }
    }

    public void UnregisterInventory(Inventory inventory) {
      if (inventories.Contains(inventory)) {
        inventories.Remove(inventory);
        if (inventory.TotalQty() > 0) {
          inventory.Explode();
        }
      }
    }

    public void InitStockpile() {

      //job.naStockPileSettings = new NativeHashMap<int, int>();
      foreach (string name in InventoryItem.GetAllPrototypeNames()) {
        InventoryItem item = InventoryItem.GetPrototype(name);

        StockPileSetting sps = new StockPileSetting(item);

        stockpileSettings[name] = sps;
        StockpileSettingsList.Add(sps);
        StockPileSetting loose = new StockPileSetting(item);
        looseQtys[name] = loose;
        LooseSettingsList.Add(loose);

        //job.naStockPileSettings.TryAdd(sps.id, 0);
      }
      //stockpileSettingsArray = stockpileSettings.Values.ToArray();

    }

    public void Update(float deltaTime) {
      foreach (NYDITimer timer in timers) {
        timer.Update(deltaTime);
      }


    }

    public int GetLooseQty(string name) {
      if (looseQtys.ContainsKey(name)) {
        return looseQtys[name].currentQty - looseQtys[name].allocatedQty;
      }
      return 0;
    }

    public int GetStockpileMaxQty(string name) {
      if (stockpileSettings.ContainsKey(name)) {
        return stockpileSettings[name].maxQty;
      }
      return 0;
    }

    public int GetStockpileQty(string name) {
      if (stockpileSettings.ContainsKey(name)) {
        return stockpileSettings[name].currentQty - stockpileSettings[name].allocatedQty;
      }
      return 0;
      //int qty = 0;
      //foreach (Inventory inv in inventories.Where(e => e.IsStockPile)) {
      //  qty += inv.HowMany(name);
      //}

      //return qty;
    }


    //public void UpdateStockPileSettings() {
    //  ////job.Schedule();
    //  //StockPileSetting sps = stockpileSettingsArray[spsIndex];
    //  //sps.currentQty = GetStockpileQty(sps.name);

    //  //spsIndex = (spsIndex + 1) % stockpileSettingsArray.Length;
    //}


    //public List<Tile> GetNearest(Tile fromHere, string itemType, int qty) {
    //  Dictionary<Tile, float> tileDist = new Dictionary<Tile, float>();
    //  List<Tile> tiles = new List<Tile>();
    //  //if (inventories.ContainsKey(itemType)) {
    //  foreach (Inventory inventory in inventories) {
    //    if (inventory.tile == null) continue;
    //    int hasQty = inventory.HowMany(itemType);

    //    if (hasQty > 0) {
    //      tileDist[inventory.tile] = Funcs.TaxiDistance(fromHere, inventory.tile);


    //    }
    //  }


    //  int qtyTaken = 0;
    //  foreach (KeyValuePair<Tile, float> kvp in tileDist.OrderBy(key => key.Value)) {
    //    if (qtyTaken == qty) {
    //      break;
    //    }
    //    if (kvp.Key.currentStack < (qty - qtyTaken)) {
    //      qtyTaken += kvp.Key.currentStack;

    //    } else {
    //      qtyTaken = qty;
    //    }
    //  }



    //  return tiles;
    //}

    public Tile GetNearest(Tile fromHere, string itemType, bool includeStockPiles = true) {
      if (itemType == InventoryItem.ANY) {


        itemType = InventoryItem.GetRandomPrototype().type;
      }

      List<Tile> tiles = new List<Tile>();
      Dictionary<Tile, float> nearestTiles = new Dictionary<Tile, float>();
      float maxSoFar = float.MaxValue;
      foreach (Inventory inventory in inventories.Where(e => e.HowMany(itemType) > 0 && e.tile != null)) {
        if (!includeStockPiles && inventory.tile.installedItem != null && inventory.tile.installedItem.type == "installed::stockpile") continue;

        float dist = Funcs.TaxiDistance(fromHere, inventory.tile);

        if (dist < maxSoFar) {
          maxSoFar = dist;
          nearestTiles[inventory.tile] = dist;
        }
      }
      foreach (KeyValuePair<Tile, float> kvp in nearestTiles.OrderBy(key => key.Value)) {
        return kvp.Key;
      }

      return null;
    }


    //if (inventories.ContainsKey(itemType)) {
    //  List<Tile> tiles = new List<Tile>();

    //  List<InventoryItem> items = inventories[itemType];

    //  foreach (InventoryItem item in items) {
    //    if (!includeStockPiles) {
    //      if (item.tile.installedItem != null && item.tile.installedItem.type == "installed::stockpile") continue;
    //    }
    //    tiles.Add(item.tile);
    //  }

    //  float lowest = 0;
    //  Tile nearest = null;
    //  float dist = 0;
    //  foreach (Tile t in tiles) {
    //    dist = Funcs.TaxiDistance(fromHere, t);

    //    if (nearest == null || dist < lowest) {
    //      nearest = t;
    //      lowest = dist;
    //    }
    //  }

    //  return nearest;

    //}
    //return null;
    //}

    //public bool PlaceItemOnTile(Tile t, InventoryItem item) {

    //  if (t.PlaceInventoryItem(item)) {
    //    if (item.currentStack == 0) {
    //      RemoveInventoryItem(item);
    //    }

    //    if (t.inventoryItem != null) {
    //      AddInventoryItem(t.inventoryItem);
    //    }
    //    return true;
    //  } else {
    //    return false;
    //  }
    //}

    //public void AddInventoryItem(InventoryItem item) {

    //  if (inventories.ContainsKey(item.type)) {
    //    if (!inventories[item.type].Contains(item)) {
    //      inventories[item.type].Add(item);
    //    }

    //  } else {
    //    inventories[item.type] = new List<InventoryItem>();
    //    inventories[item.type].Add(item);
    //  }

    //}

    //public InventoryItem RemoveQty(Tile tile, string name, int qty) {

    //  if (tile.inventoryItem != null && tile.inventoryItem.type == name && tile.inventoryItem.currentStack >= qty) {
    //    if (tile.inventoryItem.currentStack == qty) {
    //      RemoveInventoryItem(tile.inventoryItem);


    //    } else {

    //    }


    //  }

    //  return null;

    //}


    //public void RemoveInventoryItem(InventoryItem item) {
    //  if (item == null) return;
    //  if (inventories.ContainsKey(item.type)) {
    //    if (inventories[item.type].Contains(item)) {
    //      inventories[item.type].Remove(item);
    //    }

    //  }

    //}
  }
}