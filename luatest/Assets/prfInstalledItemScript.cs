﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NoYouDoIt.DataModels;
using NoYouDoIt.TheWorld;
using NoYouDoIt.Utils;
using NoYouDoIt.Controller;
public class prfInstalledItemScript : MonoBehaviour {
  public TextMeshProUGUI txtItemName;
  public TMP_InputField tmpInputField;
  public Toggle tglActive;
  public GameObject goContent;
  public GameObject prfKVP;
  public TMP_Dropdown chooseRecipeDrop;
  string itemName;
  string workCondition;
  public TMP_Text currentRecipeText;
  InstalledItem item;
  List<GameObject> kvpGo;
  bool ok = false;
  Dictionary<string, string> availableRecipes = new Dictionary<string, string>();
  List<TMP_Dropdown.OptionData> niceNames = new List<TMP_Dropdown.OptionData>();

  public void OnConditionValueChanged(string s) {
    workCondition = s;

  }

  public void OnConditionEditEnded(string s) {
    OnConditionValueChanged(s);
  }

  public void Set(InstalledItem item) {
    ok = false;
    kvpGo = new List<GameObject>();
    this.item = item;
    this.itemName = item.niceName;
    this.workCondition = item.workCondition == null ? "" : item.workCondition;
    txtItemName.text = item.niceName;
    tmpInputField.text = this.workCondition;
    gameObject.SetActive(true);
    WorldController.Instance.inputController.SetInputMode(INPUT_MODE.SHOWING_DIALOGUE);
    WorldController.Instance.gameState = GAME_STATE.PAUSE;
    Dictionary<string, string> kvps = item.itemParameters.GetItems();

    tglActive.isOn = item.active;

    currentRecipeText.text = "";
    if (item.workRecipeName != null) {
      currentRecipeText.text = Funcs.PadPair(46,"current recipe", Recipe.GetNiceName(item.workRecipeName));
      currentRecipeText.text += "\n" + Funcs.PadPair(46, ".", ".");
      //Debug.Log(currentRecipeText.textBounds.max.x);
      currentRecipeText.text += Recipe.GetRecipe(item.workRecipeName).ToString(46);
      currentRecipeText.text += "\n" + Funcs.PadPair(46, ".", ".");
    }

    
    //currentRecipeText.text += "\n" + Funcs.PadPair(46, ".", ".");
    //currentRecipeText.text += "\n" + Funcs.PadPair(46, ".", ".");

    


    foreach (string k in kvps.Keys) {
      CreateKVPControl(k, kvps[k]);
    }
    availableRecipes.Clear();
    niceNames.Clear();
    chooseRecipeDrop.ClearOptions();
    //Debug.Log("avaiable recipes: " + item.availableRecipes.Count);
    //item.itemParameters.SetInt("num recipes", item.availableRecipes.Count);

    if (item.canChangeRecipe) {
      foreach (string r in item.availableRecipes) {
        string niceName = Recipe.GetNiceName(r);
        if (niceName == null) {

        } else {
          availableRecipes[niceName] = r;
          TMP_Dropdown.OptionData da = new TMP_Dropdown.OptionData(niceName);
          niceNames.Add(da);
          
        }
      }

      
      chooseRecipeDrop.AddOptions(niceNames);
      chooseRecipeDrop.interactable = true;
      chooseRecipeDrop.value = chooseRecipeDrop.options.IndexOf(niceNames.Find(e => e.text == Recipe.GetNiceName(item.workRecipeName)));
    } else {
      chooseRecipeDrop.interactable = false;
    }
  }

  private void CreateKVPControl(string k, string v) {
    GameObject go = SimplePool.Spawn(prfKVP, Vector3.one, Quaternion.identity);
    go.transform.SetParent(goContent.transform);
    go.transform.localScale = Vector3.one;
    prfInstalledItemKVPScript kvpcontrol = go.GetComponent<prfInstalledItemKVPScript>();
    kvpcontrol.Set(k, v);

    kvpGo.Add(go);
  }

  void Start() {
    tmpInputField.onValueChanged.AddListener(OnConditionValueChanged);
    tmpInputField.onEndEdit.AddListener(OnConditionEditEnded);
  }

  // Update is called once per frame
  void Update() {


  }

  public void AddBlankKVP() {
    CreateKVPControl("##new key", "##new value");
  }


  public void OkClicked() {
    ok = true;
    Deactivate();
  }

  public void CancelClicked() {
    Deactivate();

  }

  private void Deactivate() {

    foreach (GameObject kvp in kvpGo) {
      prfInstalledItemKVPScript kvps = kvp.GetComponent<prfInstalledItemKVPScript>();
      if (ok) {
        item.itemParameters.SetString(kvps.k, kvps.v);
      }
      SimplePool.Despawn(kvp);
    }
    if (ok) {
      this.item.active = tglActive.isOn;
      this.item.workCondition = workCondition.Length == 0 ? null : workCondition;
      if (chooseRecipeDrop.interactable && chooseRecipeDrop.options.Count > 0) {
        this.item.nextWorkRecipeName = availableRecipes[niceNames[chooseRecipeDrop.value].text];
      }

    }


    gameObject.SetActive(false);
    WorldController.Instance.inputController.SetInputMode(INPUT_MODE.GAME);
    WorldController.Instance.gameState = GAME_STATE.PLAY;


  }
}
