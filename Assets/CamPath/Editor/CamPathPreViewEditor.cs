#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：FTD
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：2/18/2016
// 模块描述：
//----------------------------------------------------------------*/
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[CustomEditor(typeof(CamPathPreView))]
public class CamPathPreViewEditor : Editor
{

    private CamPathPreView script;

    float moveForward = 0;
    float moveUp = 0;
    float pitchUp = 0;
    Vector3 savedPos;
    Vector3 savedRot;

    Vector3 lastPos;
    Vector3 lastRot;
    void OnEnable()
    {





        script = (CamPathPreView)target;

        ///首先要计算好parent(camera)的坐标
        ///
       // script.transform.parent.parent.GetComponent<CamPathManager>().PositionToFocusObject();

        CamPathManager manager = GameObject.FindObjectOfType<CamPathManager>();
        GameObject charactor = GameObject.Find(manager.charactorName);
        if (charactor)
        {
            manager.transform.position = charactor.transform.position;
        }


        if (SceneView.currentDrawingSceneView)
        {
            SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
        }
        savedPos = script.transform.parent.position;
        savedRot = script.transform.parent.rotation.eulerAngles;

     
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label("up");
        moveUp = GUILayout.HorizontalSlider(moveUp, -10, 10);
  

        GUILayout.Label("forward");
        moveForward = GUILayout.HorizontalSlider(moveForward, 10, -10);

        script.transform.parent.position = savedPos + script.transform.parent.forward * moveForward + script.transform.parent.up * moveUp;


        if (lastPos != script.transform.parent.position)
        {
            lastPos = script.transform.parent.position;
            if (SceneView.currentDrawingSceneView)
            {
                SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
            }
        }
        GUILayout.Label("pitch up");
        pitchUp = GUILayout.HorizontalSlider(pitchUp, 20, -20);
        Vector3 rot = savedRot;
        rot.x -= pitchUp;
        script.transform.parent.rotation = Quaternion.Euler(rot);
        if (lastRot != script.transform.parent.rotation.eulerAngles)
        {
            lastRot = script.transform.parent.rotation.eulerAngles;
            if (SceneView.currentDrawingSceneView)
            {
                SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
            }
        }
    }
}
