﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoYouDoIt.Controller {
  using NoYouDoIt.Controller;
  using NoYouDoIt.DataModels;
  using NoYouDoIt.TheWorld;
  using NoYouDoIt.Utils;
  public class WorkItemManager {

    Action<WorkItem> CBWorkCreated;
    Action<WorkItem> CBWorkCompleted;


    private List<WorkItem> workItems = new List<WorkItem>();
    private List<WorkItem> assignedWork = new List<WorkItem>();

    public WorkItemManager() {



    }

    public void Update(float deltaTime) {
      AssignWorkToRobots(deltaTime);

    }

    public void ReturnWork(WorkItem work) {
      if (work != null) {
        if (!workItems.Contains(work)) {
          workItems.Add(work);
        }
        if (assignedWork.Contains(work)) {
          assignedWork.Remove(work);
        }

        if (work.assignedRobot != null) {
          work.Unassign();
        }
      }

    }

    public void AddWorkItem(WorkItem item) {
      //Debug.Log(item);
      workItems.Add(item);
      if (CBWorkCreated != null) {
        CBWorkCreated(item);
      }

    }

    public void WorkItemComplete(WorkItem work) {
      if (assignedWork.Contains(work)) {
        assignedWork.Remove(work);
      }
      work.Unassign();
      if (CBWorkCompleted != null) {
        CBWorkCompleted(work);
      }
    }


    public void AssignWorkToRobots(float deltaTime) {
      for (int i = workItems.Count - 1; i >= 0; i -= 1) {
        WorkItem w = workItems[i];

        if (!w.IsItReadyYet()) continue;
        Entity worker = null;
        float dist = 0;
        float nearest = 0;
        /*
        int closeByIdle = World.current.entities.Count(
            e => e.typeName == "entities::basic_robot"
            && e.state == "idle"
            && w.workTile.chunk == e.pos.chunk);

        int closeByReady = World.current.entities.Count(
            e => e.typeName == "entities::basic_robot"
            && e.state == "find_work"
            && w.workTile.chunk == e.pos.chunk);

        if (closeByIdle > 0 && closeByReady == 0) continue;
        */

        foreach (Entity robot in World.current.entities.Where(e => e.state == "find_work")) {
          dist = Funcs.TaxiDistance(w.workTile, robot.pos);
          if (worker == null || dist < nearest) {
            worker = robot;
            nearest = dist;
          }

        }

        if (worker != null) {
          worker.work = w;
          worker.state = "init_work";
          w.Assign(worker);
          assignedWork.Add(w);
          workItems.Remove(w);
          //Debug.Log("gave work " + w.ToString()+ " to " + worker.name);
        }
      }


    }


    //public WorkItem GetNearestWork(Tile posTile) {
    //  WorkItem item = null;
    //  float shortestDist = 0;
    //  for (int i = workItems.Count - 1; i >= 0; i -= 1) {
    //    WorkItem w = workItems[i];

    //    if (!w.IsItReadyYet()) continue;

    //    float dist = Funcs.TaxiDistance(w.workTile, posTile);
    //    if (item == null || dist < shortestDist) {
    //      item = w;
    //      shortestDist = dist;

    //    }


    //  }

    //  if (item != null) {
    //    assignedWork.Add(item);
    //    workItems.Remove(item);
    //  }

    //  return item;

    //}


    public void CBRegisterOnCreated(Action<WorkItem> cb) {
      CBWorkCreated -= cb;
      CBWorkCreated += cb;
    }

    public void CBUnregisterOnCreated(Action<WorkItem> cb) {
      CBWorkCreated -= cb;
    }

    public void CBRegisterOnCompleted(Action<WorkItem> cb) {
      CBWorkCompleted -= cb;
      CBWorkCompleted += cb;
    }

    public void CBUnregisterOnCompleted(Action<WorkItem> cb) {
      CBWorkCompleted -= cb;
    }


  }


}
