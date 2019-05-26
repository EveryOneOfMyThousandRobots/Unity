﻿
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour {


  private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();




  public WorldController wcon;
  //public World world;


  private void Awake() {

    LoadSprites();
  }

  void Start() {
    Debug.Log("init done " + this.name);


  }


  // Update is called once per frame
  void Update() {


  }






  private void LoadSprites() {
    Sprite[] loaded = Resources.LoadAll<Sprite>("sprites");
    foreach (Sprite s in loaded) {
      //Debug.Log(s);
      string sn = s.name;
      int counter = 1;
      while (true) {
        if (!sprites.ContainsKey(sn)) {
          sprites.Add(sn, s);
          break;

        } else {
          sn = s.name + "_" + counter;
          counter += 1;
        }
      }

    }
  }

  public void SetTileSprite(Tile t) {
    SpriteRenderer sr = WorldController.Instance.GetGameObjectFromTile(t).GetComponent<SpriteRenderer>();
    //Debug.Log("give me sprite: " + t.type.spriteName);
    sr.sprite = GetRandomSprite(t.type.sprites);//GetSprite(t.type.spriteName);

  }

  public Sprite GetRandomSprite(string[] tSprites) {
    return GetSprite(tSprites[Random.Range(0, tSprites.Length)]);
  }

  public Sprite GetSprite(string name) {
    if (sprites.ContainsKey(name)) {
      return sprites[name];
    } else {
      Debug.Log("could not locate sprite [" + name + "]");
      return sprites["sprite_error"];
    }
  }

  public void JobCreated(Job j) {
    /*
    InstalledItem item = wcon.world.get
    Sprite s = j.jobType
    */
  }

  public Sprite GetSprite(Character c) {
    return GetSprite(c.spriteName);
  }

  public SpriteHolder GetDoorSprite(InstalledItem door, SpriteHolder sh) {
    float openness = door.itemParameters.GetFloat("openness");

    int index = (int) (openness / (1.0f / 7.0f));
    sh.s = GetSprite("installed_door_" + index);
    //if (openness > 0.75) {
    //  sh.s = GetSprite("installed_door_25");
    //} else if (openness > 0.5 && openness <= 0.75) {
    //  sh.s = GetSprite("installed_door_50");
    //} else if (openness > 0.1 && openness <= 0.5) {
    //  sh.s = GetSprite("installed_door_75");
    //} else {
      
    //}


    return sh;


  }

  public SpriteHolder GetSprite(InstalledItem item) {
    SpriteHolder sp = new SpriteHolder();
    sp.s = GetSprite(item.getRandomSpriteName());


    if (item.randomRotation) {
      int r = Random.Range(0, 4);
      sp.r = 90 * r;
    }



    if (item.linksToNeighbour) {
      int x = item.tile.x;
      int y = item.tile.y;
      bool northMatches, eastMatches, southMatches, westMatches;
      int countOfNeighbours = 0;
      //Dictionary<string, Tile> ngbrs = WorldController.Instance.world.GetNeighbours(item);
      Tile north = item.tile.GetNeighbour(World.NORTH);
      Tile east = item.tile.GetNeighbour(World.EAST);
      Tile south = item.tile.GetNeighbour(World.SOUTH);
      Tile west = item.tile.GetNeighbour(World.WEST);

      northMatches = hasMatchingNeighbour(north, item);
      if (northMatches) countOfNeighbours += 1;
      eastMatches = hasMatchingNeighbour(east, item);
      if (eastMatches) countOfNeighbours += 1;
      southMatches = hasMatchingNeighbour(south, item);
      if (southMatches) countOfNeighbours += 1;
      westMatches = hasMatchingNeighbour(west, item);
      if (westMatches) countOfNeighbours += 1;

      switch (countOfNeighbours) {
        case 0:

          break;
        case 1:
          sp.s = GetSprite(item.sprite_s);
          if (northMatches) {
            sp.r = 180;
          } else if (eastMatches) {
            sp.r = 90;
          } else if (westMatches) {
            sp.r = -90;
          }
          break;
        case 2:
          sp.s = GetSprite(item.sprite_ns);
          if (eastMatches && westMatches) {
            sp.r = 90;
          } else if (northMatches && southMatches) {
            sp.r = 0;
          } else {
            sp.s = GetSprite(item.sprite_sw);
            if (southMatches && westMatches) {

            } else if (northMatches && eastMatches) {
              sp.r = -180;
            } else if (northMatches && westMatches) {
              sp.r = -90;
            } else if (eastMatches && southMatches) {
              sp.r = 90;
            }
          }
          break;
        case 3:
          sp.s = GetSprite(item.sprite_nsw);

          if (northMatches && southMatches && westMatches) {

          } else if (northMatches && eastMatches && westMatches) {
            sp.r = -90;
          } else if (eastMatches && southMatches && westMatches) {
            sp.r = 90;
          } else if (northMatches && eastMatches && southMatches) {
            sp.r = 180;
          }

          break;
        case 4:
          sp.s = GetSprite(item.sprite_nesw);
          break;

      }


    } else {

    }
    return sp;
  }

  private bool hasMatchingNeighbour(Tile t, InstalledItem item) {
    if (t == null || t.installedItem == null || !item.neighbourTypes.Contains(t.installedItem.type)) {
      return false;
    } else {
      return true;
    }

  }

}

public class SpriteHolder {
  public Sprite s = null;
  public float r = 0;
  public SpriteHolder() { }
}
