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

    float yaw = 0;
    CamPathManager manager;
    float savedMapAngle;
    float lastAngle;
    void OnEnable()
    {

        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;



        script = (CamPathPreView)target;

        ///首先要计算好parent(camera)的坐标
        ///
        // script.transform.parent.parent.GetComponent<CamPathManager>().PositionToFocusObject();

        manager = GameObject.FindObjectOfType<CamPathManager>();
        GameObject charactor = GameObject.Find(manager.charactorName);
        if (charactor)
        {
            manager.transform.position = charactor.transform.position;
        }

        Vector3 direct = script.transform.parent.position - manager.transform.position;
        float currentDirectionAngle = Vector3.Angle(direct, Vector3.up);
        Debug.Log("currentDirectionAngle " + currentDirectionAngle);
        //通过direct计算标准方向（0度）与y轴的角度
        Vector3 standardDirect = (Quaternion.AngleAxis(currentDirectionAngle, -Vector3.forward) * Vector3.up).normalized * direct.magnitude;

        savedMapAngle = Vector3.Angle(direct, standardDirect);

        Debug.Log("savedMapAngle " + savedMapAngle);



        if (SceneView.currentDrawingSceneView)
        {
            SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
        }
        savedPos = script.transform.parent.position;
        savedRot = script.transform.parent.rotation.eulerAngles;


    }

    GUIStyle style = new GUIStyle();

    Vector3 abc;
    Vector3 kk;
    Vector3 inp;
    public void OnSceneGUI()
    {
        Handles.SphereCap(0, manager.transform.position, Quaternion.identity, 1);
        Handles.SphereCap(0,kk, Quaternion.identity,1);
        Handles.PositionHandle(abc, Quaternion.identity);

    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label("up");
        moveUp = GUILayout.HorizontalSlider(moveUp, -10, 10);


        GUILayout.Label("forward");
        moveForward = GUILayout.HorizontalSlider(moveForward, 10, -10);

        Vector3 inputPos = savedPos + script.transform.parent.forward * moveForward + script.transform.parent.up * moveUp;


        GUILayout.Label("yaw");
        yaw = GUILayout.HorizontalSlider(yaw, 45, -45);
        float mapAngle = savedMapAngle;
        //当前摄像机对于focus的方向
        Vector3 direct = inputPos - manager.transform.position;
        //通过direct计算标准方向（0度）与y轴的角度
        float currentDirectionAngle = Vector3.Angle(direct, Vector3.up);

        //通过a与direct计算标准方向，逆时针旋转direct，所以是负数-Vector3.forward
        Vector3 standardDirect = (Quaternion.AngleAxis(currentDirectionAngle, -Vector3.forward) * Vector3.up).normalized * direct.magnitude;

        //用标准方向计算mapAngle角度下的值，这个值就是设计师设计值
        //inputPos = Quaternion.AngleAxis(savedMapAngle, Vector3.up) * standardDirect + manager.transform.position;
        abc = standardDirect + manager.transform.position;
        kk = (Quaternion.AngleAxis(Vector3.Angle(direct, standardDirect), Vector3.up) * standardDirect + manager.transform.position);
        Debug.Log(inputPos + " " + (Quaternion.AngleAxis(Vector3.Angle(direct, standardDirect), Vector3.up) * standardDirect + manager.transform.position));
        inp = inputPos;
        script.transform.parent.position = inputPos;
        script.transform.parent.LookAt(manager.transform);


        if (lastPos != script.transform.parent.position)
        {
            lastPos = script.transform.parent.position;
            if (SceneView.currentDrawingSceneView)
            {
                SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
            }
        }
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
