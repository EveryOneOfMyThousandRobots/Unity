﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.IO.Compression;

public static class Funcs {
  public static string getSpriteName(string name) {
    name = name.Replace(':', '_');



    return name;
  }

  public static string jsonGetString(JToken token, string df) {

    if (token == null) {
      return df;
    } else {
      return (string)token;
    }
  }

  public static bool jsonGetBool(JToken token, bool df) {
    if (token == null) {
      return df;
    } else {
      return (bool)token;
    }
  }

  public static int jsonGetInt(JToken token, int df) {
    if (token == null) {
      return df;
    } else {
      return (int)token;
    }
  }

  public static float jsonGetFloat(JToken token, float df) {
    if (token == null) {
      return df;
    } else {
      return (float)token;
    }
  }

  public static float QuickDistance(float x1, float y1, float x2, float y2) {
    float d = Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2);


    return d;
  }

  public static float TaxiDistance(Tile A, Tile B) {
    return TaxiDistance(A.x, A.y, B.x, B.y);
  }

  public static float TaxiDistance(float x1, float y1, float x2, float y2) {

    return Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1);
  }

  public static float Distance(Tile A, Tile B) {
    return Distance(A.x, A.y, B.x, B.y);
  }
  public static float Distance(float x1, float y1, float x2, float y2) {
    float d = Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));


    return d;
  }

  public static void CopyTo(Stream src, Stream dest) {
    byte[] bytes = new byte[4096];

    int cnt;

    while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
      dest.Write(bytes, 0, cnt);
    }
  }

  public static byte[] Zip(string str) {
    var bytes = Encoding.UTF8.GetBytes(str);

    using (var msi = new MemoryStream(bytes))
    using (var mso = new MemoryStream()) {
      using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
        //msi.CopyTo(gs);
        CopyTo(msi, gs);
      }

      return mso.ToArray();
    }
  }

  public static string Unzip(byte[] bytes) {
    using (var msi = new MemoryStream(bytes))
    using (var mso = new MemoryStream()) {
      using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
        //gs.CopyTo(mso);
        CopyTo(gs, mso);
      }

      return Encoding.UTF8.GetString(mso.ToArray());
    }
  }

  static void Main(string[] args) {
    byte[] r1 = Zip("StringStringStringStringStringStringStringStringStringStringStringStringStringString");
    string r2 = Unzip(r1);
  }

  public static string Base64Encode(byte[] bytes) {
    return System.Convert.ToBase64String(bytes);
  }

  public static byte[] Base64Decode(string base64EncodedData, bool whatever) {
    return  System.Convert.FromBase64String(base64EncodedData);
    
  }

    public static string Base64Encode(string plainText) {
    byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
    return System.Convert.ToBase64String(plainTextBytes);
  }

  public static string Base64Decode(string base64EncodedData) {
    byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
    return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
  }


  public static string TitleCase(string s) {
    string ss = s[0].ToString().ToUpper();

    ss += s.Substring(1).ToLower();


    return ss;
  }
}
