﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoYouDoIt.TheWorld;
using NoYouDoIt.DataModels;
namespace NoYouDoIt.Controller {
  public class SoundController : MonoBehaviour {
    WorldController worldController;
    //World world;


    private void Awake() {

    }

    private SoundController() {

    }

    public void Init() {
      //WorldController.Instance.cbRegisterReady(Init);
      Debug.Log("init " + this.name);
      worldController = WorldController.Instance;

      World.current.RegisterInstalledItemCB(OnInstalledItemCreated);
      World.current.CBRegisterTileChanged(OnTileTypeChanged);
    }
    // Start is called before the first frame update
    void Start() {

    }

    public void OnTileTypeChanged(Tile t) {
      //Debug.Log("play tile changed sound");

    }

    public void OnInstalledItemCreated(InstalledItem item) {
      //Debug.Log("play item created sound");

    }
  }
}
