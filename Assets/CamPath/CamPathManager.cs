#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：FTD
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：2/17/2016
// 模块描述：
//----------------------------------------------------------------*/
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamPathManager : MonoBehaviour
{

    [HideInInspector]
    public string fullMapCameraMaxName = "Camera Full Map Max";
    [HideInInspector]
    public string fullMapCameraMinName = "Camera Full Map Min";
    [HideInInspector]
    public string charactorName = "Camera Charactor";

    [HideInInspector]
    GameObject alignToObject;
    float updateValue = 0;

    public static CamPathManager instance;

    void Awake()
    {
        instance = this;
    }
    public void SettleCamera()
    {

        //search for a manager object within current scene
        GameObject charactor = GameObject.Find(charactorName);
        if (charactor)
        {
            transform.position = charactor.transform.position;
        }
    }
    public void AlignViewToObject(GameObject obj)
    {
        if (alignToObject==null)
        {
            alignToObject = obj;
            LeanTween.value(gameObject, 0, 1, 0.8f).setOnUpdate((float f) => { updateValue = f; }).setOnComplete(() => { alignToObject = null; updateValue = 0; });
        }
    }
    
    void Start()
    {
      
    }
    void Update()
    {
        if (alignToObject)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, alignToObject.transform.position, updateValue);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, alignToObject.transform.rotation, updateValue);
        }
    }
}
