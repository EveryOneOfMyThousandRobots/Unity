﻿using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace NoYouDoIt.DataModels {
  using NoYouDoIt.Utils;
  using NoYouDoIt.TheWorld;
  public class InventoryItem {

    public static readonly string ANY = "inv::any";
    private static Dictionary<string, InventoryItem> prototypes = new Dictionary<string, InventoryItem>();
    //properties
    public string type { get; private set; }
    public string niceName { get; private set; }
    //public bool isPrototype { get; private set; }
    //InventoryItem prototype = null;
    //private int prototypeStackSize = 50;
    public string spriteName { get; private set; }
    public int stackSize { get; private set; }


    //public int currentStack = 1;

    //public Tile tile { get; private set; }
    //public Character character { get; private set; }

    ////-----------------GET/SET----------------------

    //public void SetTile(Tile tile) {
    //  character = null;
    //  this.tile = tile;

    //}

    //public void SetCharacter(Character character) {
    //  this.character = character;
    //  this.tile = null;
    //}

    //----------------CONSTRUCTORS--------------------
    private InventoryItem() {

    }
    //private InventoryItem(InventoryItem proto) {
    //  this.type = proto.type;
    //  //this.isPrototype = false;
    //  this.niceName = proto.niceName;
    //  //this.prototype = proto;
    //  this.spriteName = proto.spriteName;


    //}

    //private InventoryItem Copy() {
    //  InventoryItem item = new InventoryItem(this.prototype);
    //  item.currentStack = this.currentStack;

    //  return item;

    //}

    //-----------------------------------------OVERRIDES-------------------------------

    public override string ToString() {
      return type + "(" + stackSize + ")"; //+ " isProto:" + isPrototype;
    }


    //--------------------------------------STATIC METHODS--------------------------------------

    public static InventoryItem GetPrototype(string type) {
      if (prototypes.ContainsKey(type)) {
        return prototypes[type];
      } else {
        return null;
      }

    }

    public static string GetNiceName(string type) {
      InventoryItem item = GetPrototype(type);

      if (item != null) {
        return item.niceName;
      } else {
        return null;
      }
    }

    public static List<string> GetAllPrototypeNames() {
      return new List<string>(prototypes.Keys);
    }

    public static InventoryItem GetRandomPrototype() {
      int r = UnityEngine.Random.Range(0, prototypes.Count);
      int c = 0;
      foreach (string k in prototypes.Keys) {
        if (r == c) {
          return prototypes[k];
        }
        c += 1;
      }
      return null;
    }
    //private static void CreateInventoryItemPrototypes(JArray InventoryPrototypesArray) {


    //  foreach (JObject jsonProto in InventoryPrototypesArray) {
    //    CreateInventoryItemPrototype(jsonProto);

    //    //Debug.Log(item);
    //  }


    //}

    private static void CreateInventoryItemPrototype(JObject jsonProto) {
      InventoryItem item = new InventoryItem();
      string type = Funcs.jsonGetString(jsonProto["type"], "none");
      string niceName = Funcs.jsonGetString(jsonProto["niceName"], "none");
      string spriteName = Funcs.jsonGetString(jsonProto["spriteName"], "");
      int stackSize = Funcs.jsonGetInt(jsonProto["stackSize"], 1);

      item.type = type;
      item.niceName = niceName;
      //item.isPrototype = true;
      //item.prototype = null;
      item.stackSize = stackSize;
      item.spriteName = spriteName;
      prototypes.Add(item.type, item);
      World.current.lua[Funcs.GetLuaVariableName(item.type)] = item.type;
    }

    public static int GetStackSize(string type) {
      if (prototypes.ContainsKey(type)) {
        return prototypes[type].stackSize;
      } else {
        return 0;
      }

    }

    public static void LoadFromFile() {
      prototypes.Clear();

      string path = Path.Combine(Application.streamingAssetsPath, "data", "InventoryItems");

      string[] files = Directory.GetFiles(path, "*.json");

      foreach(string file in files) {
        string fcontents = File.ReadAllText(file);
        JObject json = JObject.Parse(fcontents);

        CreateInventoryItemPrototype(json);
      }


    }

    //public static InventoryItem CreateInventoyItemInstance(string type) {


    //  InventoryItem item = null;

    //  if (type != null && prototypes.ContainsKey(type)) {
    //    item = new InventoryItem(prototypes[type]);


    //  }



    //  return item;
    //}



  }
}