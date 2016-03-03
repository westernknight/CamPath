#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：FTD
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：2/22/2016
// 模块描述：
//----------------------------------------------------------------*/
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public static class ExtensionMethodsEditor
{
    public static void SaveChildObjectPropertyToJsonFile(GameObject gameObject)
    {
        SaveChildObjectPropertyToJsonFile(gameObject, gameObject.name, "");
    }
    public static void SaveChildObjectPropertyToJsonFile(GameObject gameObject,string jsonFileName,string folder)
    {
        if (string.IsNullOrEmpty(folder))
        {
            
        }
        else if (!folder.Contains("/"))
        {
            folder += "/";
        }
        string path = Application.streamingAssetsPath + "/Config/" +folder+ jsonFileName + ".json";
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        LitJson.JsonData allData = new LitJson.JsonData();
        allData.SetJsonType(LitJson.JsonType.Array);

        {
            LitJson.JsonData property = new LitJson.JsonData();
            property.SetJsonType(LitJson.JsonType.Array);


            Transform t = gameObject.transform;

            LitJson.JsonData name = new LitJson.JsonData();
            name = t.name;

            LitJson.JsonData pos = new LitJson.JsonData();
            pos.SetJsonType(LitJson.JsonType.Array);
            pos.Add(t.position.x);
            pos.Add(t.position.y);
            pos.Add(t.position.z);

            LitJson.JsonData rot = new LitJson.JsonData();
            rot.SetJsonType(LitJson.JsonType.Array);
            rot.Add(t.rotation.x);
            rot.Add(t.rotation.y);
            rot.Add(t.rotation.z);
            rot.Add(t.rotation.w);

            property.Add(name);
            property.Add(pos);
            property.Add(rot);

            allData.Add(property);
        }
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            LitJson.JsonData property = new LitJson.JsonData();
            property.SetJsonType(LitJson.JsonType.Array);


            Transform t = gameObject.transform.GetChild(i);

            LitJson.JsonData name = new LitJson.JsonData();
            name = t.name;

            LitJson.JsonData pos = new LitJson.JsonData();
            pos.SetJsonType(LitJson.JsonType.Array);
            pos.Add(t.position.x);
            pos.Add(t.position.y);
            pos.Add(t.position.z);

            LitJson.JsonData rot = new LitJson.JsonData();
            rot.SetJsonType(LitJson.JsonType.Array);
            rot.Add(t.rotation.x);
            rot.Add(t.rotation.y);
            rot.Add(t.rotation.z);
            rot.Add(t.rotation.w);

            property.Add(name);
            property.Add(pos);
            property.Add(rot);

            allData.Add(property);
        }

        FileInfo fi = new FileInfo(path);
        StreamWriter sw = new StreamWriter(fi.Create());

        sw.Write(LitJson.JsonMapper.ToJson(allData));
        sw.Close();
        Debug.Log("save ok: " + path);
    }
    public static void ReadJsonFileDataToChildObjectProperty(GameObject gameObject)
    {
        ReadJsonFileDataToChildObjectProperty(gameObject, gameObject.name,"");
    }
    public static void ReadJsonFileDataToChildObjectProperty(GameObject gameObject, string jsonFileName, string folder)
    {
        if (string.IsNullOrEmpty(folder))
        {

        }
        else if (!folder.Contains("/"))
        {
            folder += "/";
        }
        string path = Application.streamingAssetsPath + "/Config/" + gameObject.name + ".json";
        FileInfo fi = new FileInfo(path);
        if (fi != null)
        {
            if (fi.Exists)
            {
                while (gameObject.transform.childCount!=0)
                {
                    GameObject.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
                }

                StreamReader sr = new StreamReader(fi.OpenRead());
                string json = sr.ReadToEnd();
                sr.Close();
                LitJson.JsonData allData = LitJson.JsonMapper.ToObject(json);
                if (allData.IsArray)
                {
                    {
                        LitJson.JsonData property = allData[0];

                        LitJson.JsonData nameProperty = property[0];
                        gameObject.name = (string)nameProperty;

                        LitJson.JsonData posProperty = property[1];
                        Vector3 pos = new Vector3((float)(double)posProperty[0], (float)(double)posProperty[1], (float)(double)posProperty[2]);

                        LitJson.JsonData rotProperty = property[2];
                        Quaternion rot = new Quaternion((float)(double)rotProperty[0], (float)(double)rotProperty[1], (float)(double)rotProperty[2], (float)(double)rotProperty[3]);

                        gameObject.transform.position = pos;
                        gameObject.transform.rotation = rot;
                    }
                    for (int i = 1; i < allData.Count; i++)
                    {
                        GameObject go = new GameObject();
                        go.transform.parent = gameObject.transform;


                        LitJson.JsonData property = allData[i];

                        LitJson.JsonData nameProperty = property[0];
                        go.name = (string)nameProperty;

                        LitJson.JsonData posProperty = property[1];
                        Vector3 pos = new Vector3((float)(double)posProperty[0], (float)(double)posProperty[1], (float)(double)posProperty[2]);

                        LitJson.JsonData rotProperty = property[2];
                        Quaternion rot = new Quaternion((float)(double)rotProperty[0], (float)(double)rotProperty[1], (float)(double)rotProperty[2], (float)(double)rotProperty[3]);

                        go.transform.position = pos;
                        go.transform.rotation = rot;

                    }
                }
                Debug.Log("load ok: " + path);
            }
        }

    }
}
