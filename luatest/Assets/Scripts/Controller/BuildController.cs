﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NoYouDoIt.TheWorld;
using NoYouDoIt.DataModels;
using System.Linq;
namespace NoYouDoIt.Controller {
  public class BuildController : MonoBehaviour {
    public WorldController wcon;
    //public World world;
    public InputController inputCon;


    private void Awake() {

    }

    public void Init() {
      //WorldController.Instance.cbRegisterReady(Init);
      Debug.Log("init " + this.name);
      wcon = WorldController.Instance;
      //world = wcon.world;
    }
    private void Start() {


    }
    public enum BUILD_MODE {
      NONE,
      INSTALLEDITEM,
      TILE,
      ZONE, //shouldn't be used...so... why is it there?
      DECONSTRUCT
    }

    public BUILD_MODE buildMode { get; private set; } = BUILD_MODE.NONE;
    public string build { get; private set; } = "";

    public void SetBuildMode(BUILD_MODE bt, string b) {
      buildMode = bt;
      build = b;
      WorldController.Instance.inputController.mouseMode = MOUSE_MODE.BUILD;
    }

    public bool CreateBuildJobs(List<Tile> tiles) {
      bool didSomething = true;
      string localBuild = build;

      if (buildMode != BUILD_MODE.NONE) {
        switch (buildMode) {
          //InstalledItem item = 
          case BUILD_MODE.INSTALLEDITEM:
            CreateInstalledItemWork(tiles, localBuild);
            break;
          case BUILD_MODE.TILE:
            foreach (Tile tile in tiles) {
              TileType tt = TileType.TYPES[build];
              tile.SetType(tt);
            }
            break;
          case BUILD_MODE.ZONE:
            //wcon.world.zones.Add(new TileZone(tiles));
            //do nothing zones not implemented yet
            break;
          case BUILD_MODE.DECONSTRUCT:
            CreateRemoveInstalledItemWork(tiles);
            break;


        }
      } else {
        didSomething = false;
      }
      if (!Input.GetKey(KeyCode.LeftShift)) {
        ClearBuildMode();
      }

      return didSomething;
    }

    private void ClearBuildMode() {
      build = "";
      buildMode = BUILD_MODE.NONE;
      WorldController.Instance.inputController.mouseMode = MOUSE_MODE.SELECT;
    }

    private void CreateInstalledItemWork(List<Tile> tiles, string localBuild) {
      foreach (Tile tile in tiles) {

        //tile.placeInstalledObject();
        string localRecipe = InstalledItem.GetRecipeName(localBuild);
        if (World.current.isInstalledItemPositionValid(World.current, build, tile)) {
          WorkItem work = WorkItem.MakeWorkItem(tile, "SetInstalledItem", localBuild);

        }
        //  Job j = Job.MakeStandardJob(
        //        tile,
        //        //OnInstalledItemJobComplete, //(theJob) => { OnInstalledItemJobComplete(localBuild, theJob.tile); },
        //        //OnInstalledItemJobCancelled,
        //        JOB_TYPE.BUILD,
        //        1,
        //        localBuild
        //      );
        //  tile.AddJob(j);
        //  j.cbLuaRegisterJobComplete("OnInstalledItemJobComplete");
        //  j.cbLuaRegisterJobCancelled("OnInstalledItemJobCancelled");
        //  World.current.jobQueue.Push(j);

        //  //tile.pendingJob = true;
        //  //Debug.Log("jobs in queue: " +world.jobQueue.Count);

        //}
      }
    }

    private void CreateRemoveInstalledItemWork(List<Tile> tiles) {
      foreach (Tile tile in tiles.Where(e => !e.HasPendingWork)) {
        if (tile.installedItem != null && tile.installedItem.tile == tile) {
          WorkItem.MakeWorkItem(tile, "SetRemoveInstalledItem");
        }
      }
    }


    private void OnRemoveInstalledItemJobComplete(WorkItem work) {
      throw new NotImplementedException();
    }
    private void OnRemoveInstalledItemJobCancelled(WorkItem work) {
      throw new NotImplementedException();
    }


    //private void CreateRemoveInstalledItemJobs(List<Tile> tiles) {
    //  foreach (Tile tile in tiles) {
    //    //tile.placeInstalledObject();
    //    //string localRecipe = InstalledItem.GetRecipeName(localBuild);
    //    if (tile.installedItem != null && tile.installedItem.tile == tile) { //if the tile has an installed item and that tile is the primary tile for that installed item
    //      Job j = Job.MakeStandardJob(
    //            tile,
    //            //OnRemoveInstalledItemJobComplete, //(theJob) => { OnInstalledItemJobComplete(localBuild, theJob.tile); },
    //            //OnRemoveInstalledItemJobCancelled,
    //            JOB_TYPE.DECONSTRUCT,
    //            1,
    //            InstalledItem.DECONSTRUCT
    //          );
    //      tile.AddJob(j);
    //      j.cbLuaRegisterJobComplete("OnRemoveInstalledItemJobComplete");
    //      j.cbLuaRegisterJobCancelled("OnRemoveInstalledItemJobCancelled");
    //      World.current.jobQueue.Push(j);

    //      //tile.pendingJob = true;
    //      //Debug.Log("jobs in queue: " +world.jobQueue.Count);

    //    }
    //  }
    //}

    //private void OnRemoveInstalledItemJobComplete(Job job) {
    //  job.tile.RemoveJob(job);
    //  if (job.tile.installedItem != null) {
    //    job.tile.installedItem.Deconstruct();
    //  }
    //}

    //private void OnRemoveInstalledItemJobCancelled(Job job) {
    //  job.tile.RemoveJob(job);
    //}

    //private void OnInstalledItemJobComplete(Job job) {
    //  //Debug.Log("installed item job complete: " + job);



    //  if (!job.tile.RemoveJob(job)) {
    //    Debug.Log("could not remove job from tile");
    //  }
    //  //job.tile.pendingJob = false;
    //  if (World.current.PlaceInstalledItem(job.description, job.tile) == null) {
    //    job.inventory.Explode();
    //  }

    //}

    //private void OnInstalledItemJobComplete(string itemToBuild, Tile tile) {


    //}

    private void OnInstalledItemJobCancelled(WorkItem work) {
      throw new NotImplementedException();

    }
    //private void OnInstalledItemJobCancelled(Job job) {

    //  job.tile.RemoveJob(job);
    //}
  }
}
