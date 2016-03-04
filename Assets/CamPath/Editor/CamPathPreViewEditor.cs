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
        //现在的方向     摄像机-焦点
        Vector3 currentDirect = script.transform.parent.position - manager.transform.position;



        //计算现在的方向投影到xz平面上
        Vector3 currentDirectToXZPlaneVec =  currentDirect;
        currentDirectToXZPlaneVec.y = 0;
        //最终保存角度   //Vector3.Angle没有正负数
        //savedMapAngle = Vector3.Angle(currentDirectToXZPlaneVec, Vector3.right);//x轴对着镜头，所以是以Vector3.right轴取角度
        savedMapAngle = AngleSigned(currentDirectToXZPlaneVec, Vector3.right, Vector3.up);



        if (SceneView.currentDrawingSceneView)
        {
            SceneView.currentDrawingSceneView.AlignViewToObject(script.transform.parent);
        }
        savedPos = script.transform.parent.position;
        savedRot = script.transform.parent.rotation.eulerAngles;


    }

    GUIStyle style = new GUIStyle();

    public  float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return -Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
  
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
        moveUp = GUILayout.HorizontalSlider(moveUp, -10, 10);


        GUILayout.Label("forward");
        moveForward = GUILayout.HorizontalSlider(moveForward, 10, -10);

        Vector3 inputPos = savedPos + script.transform.parent.forward * moveForward + script.transform.parent.up * moveUp;


        GUILayout.Label("yaw");
        yaw = GUILayout.HorizontalSlider(yaw, 45, -45);
        float mapAngle = savedMapAngle+yaw;
        //当前摄像机对于focus的方向
        Vector3 currentDirect = inputPos - manager.transform.position;
        //计算现在的方向投影到xz平面上
        Vector3 currentDirectToXZPlaneVec = currentDirect;
        currentDirectToXZPlaneVec.y = 0;
        //读取编辑器叠加的角度
        //Vector3.right旋转角度后变为targetDirect        
        Vector3 targetDirect = Quaternion.AngleAxis(mapAngle, Vector3.up ) * Vector3.right;
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
