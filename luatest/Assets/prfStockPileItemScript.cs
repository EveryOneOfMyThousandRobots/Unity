﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoYouDoIt.DataModels;
using System;

public class prfStockPileItemScript : MonoBehaviour {

  public string itemName = "inventory::steel_plates";
  public string itemDisplayName = "Steel Plates";

  public Button btnIncrement;
  public Button btnDecrement;
  public Button btnAll;
  public Text txtCurrentQty;

  public string stringValue = "";
  public GameObject valueInputField;
  private InputField inputField;
  public Text typeText;
  private InventoryItem item;
  public int currentQty;
  StockPileSetting stockPileSetting;

  public int intValue = 32;

  public void OnValueChanged(string s) {
    stringValue = s;
    int i = 0;
    if (int.TryParse(stringValue, out i)) {
      intValue = i;
      stringValue = i.ToString();
    } else if (stringValue == "all") {

    } else {
      intValue = 0;
      stringValue = intValue.ToString();
    }
    inputField.text = stringValue;

  }

  public void OnEndEdit(string s) {
    OnValueChanged(s);
  }

  public void Set(StockPileSetting sps) {
    this.stockPileSetting = sps;
    this.item = sps.item;
    itemDisplayName = item.niceName;
    itemName = this.item.type;
    intValue = sps.maxQty;
    stringValue = intValue.ToString();
    
    Init();
  }

  private void Init() {
    inputField = valueInputField.GetComponent<InputField>();
    stringValue = intValue.ToString();
    inputField.text = stringValue;
    typeText.text = itemDisplayName;
    inputField.onValueChanged.AddListener(OnValueChanged);
    inputField.onEndEdit.AddListener(OnEndEdit);

    btnIncrement.onClick.AddListener(Increment);
    btnDecrement.onClick.AddListener(Decrement);
    btnAll.onClick.AddListener(SetToAll);
  }

  // Start is called before the first frame update
  void Start() {


  }

  public void Increment() {
    intValue += 1;
    OnValueChanged(intValue.ToString());
  }

  public void Decrement() {
    intValue -= 1;
    if (intValue < 0) {
      intValue = 0;
    }
    OnValueChanged(intValue.ToString());
  }

  public void SetToAll() {
    
    OnValueChanged("all");
  }

  // Update is called once per frame
  void Update() {

  }

  internal void SetCurrentQty(int v) {
    this.currentQty = v;
    txtCurrentQty.text = this.currentQty.ToString();
  }
}
