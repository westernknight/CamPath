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
using UnityEditor;
[CustomEditor(typeof(CamPathObject))]
public class CamPathObjectEditor : Editor
{


    private CamPathObject script;
 
    void OnEnable()
    {
        script = (CamPathObject)target;
        if (SceneView.currentDrawingSceneView)
        {
            //SceneView.currentDrawingSceneView.AlignViewToObject(script.transform);
        }
       
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Set Current Camera Position"))
        {
            script.transform.position = SceneView.currentDrawingSceneView.camera.transform.position;
            script.transform.rotation = SceneView.currentDrawingSceneView.camera.transform.rotation;

            GameObject.DestroyImmediate(script.transform.GetChild(0).gameObject);
            GameObject preView = new GameObject("PreView");
            preView.transform.parent = script.transform;
            preView.transform.localPosition = Vector3.zero;
            preView.transform.localRotation = Quaternion.identity;
            preView.transform.localScale = Vector3.one;
            preView.AddComponent<CamPathPreView>();

           
        }
    }
}
