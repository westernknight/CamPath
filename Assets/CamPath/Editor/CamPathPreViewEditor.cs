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



    

    Vector3 lastPos;
    Vector3 lastRot;

    CamPathManager manager;
    

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
        //现在的方向     摄像机-焦点
        Vector3 currentDirect = script.transform.parent.position - manager.transform.position;



        //计算现在的方向投影到xz平面上
        Vector3 currentDirectToXZPlaneVec =  currentDirect;
        currentDirectToXZPlaneVec.y = 0;
       

 

        if (SceneView.currentDrawingSceneView)
        {
            SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
        }
        if (script.init == false)
        {
            //最终保存角度
            script.savedMapAngle = Vector3.Angle(currentDirectToXZPlaneVec, Vector3.right);//x轴对着镜头，所以是以Vector3.right轴取角度
            script.savedPos = script.transform.parent.position;
            script.init = true;
        }
        


    }

    GUIStyle style = new GUIStyle();


    public void OnSceneGUI()
    {
       // Handles.SphereCap(0, currentDirectToXZPlaneVecCap+manager.transform.position, Quaternion.identity, 1);
       //  Handles.SphereCap(0, manager.transform.position, Quaternion.identity, 1);
       //  Handles.SphereCap(0, kk, Quaternion.identity, 1);
//         Handles.PositionHandle(abc, Quaternion.identity);
//         Handles.ConeCap(0, inp, Quaternion.identity, 1);

    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label("up");
        GUILayout.BeginHorizontal();
        script.moveUp = GUILayout.HorizontalSlider(script.moveUp, -20, 20);
        script.moveUp = EditorGUILayout.FloatField(script.moveUp, GUILayout.Width(50));
        GUILayout.EndHorizontal();
        GUILayout.Label("forward");
        GUILayout.BeginHorizontal();
        script.moveForward = GUILayout.HorizontalSlider(script.moveForward, 50, -50);
        script.moveForward = EditorGUILayout.FloatField(script.moveForward, GUILayout.Width(50));
        GUILayout.EndHorizontal();
        Vector3 inputPos = script.savedPos + script.transform.parent.forward * script.moveForward + script.transform.parent.up * script.moveUp;


        GUILayout.Label("yaw");
        GUILayout.BeginHorizontal();
        script.yaw = GUILayout.HorizontalSlider(script.yaw, 170, -170);
        script.yaw = EditorGUILayout.FloatField(script.yaw, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        float mapAngle = script.savedMapAngle + script.yaw;
        //当前摄像机对于focus的方向
        Vector3 currentDirect = inputPos - manager.transform.position;
        //计算现在的方向投影到xz平面上
        Vector3 currentDirectToXZPlaneVec = currentDirect;
        currentDirectToXZPlaneVec.y = 0;
        //读取编辑器叠加的角度
        //Vector3.right旋转角度后变为targetDirect        
        Vector3 targetDirect = Quaternion.AngleAxis(mapAngle, Vector3.up) * Vector3.right ;
        //targetDirect转为投影xz平面上的长度
        targetDirect = targetDirect * currentDirectToXZPlaneVec.magnitude;
        //y方向还原，targetDirect就是最终的方向
        targetDirect.y = currentDirect.y;
        //方向与焦点相加就是摄像机的位置
        script.transform.parent.position = targetDirect + manager.transform.position;
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
