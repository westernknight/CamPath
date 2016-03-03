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
[CustomEditor(typeof(CamPathCharactorPosition))]
public class CamPathCharactorPositionEditor : Editor
{

    CamPathCharactorPosition script;
    GUIStyle style = new GUIStyle();
    void OnEnable()
    {
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        script = (CamPathCharactorPosition)target;


        if (script.gameObject.name == "Position Start")
        {
            script.transform.localPosition = Vector3.zero;
        }


        CameraFullMapMaxPreview();
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
   

    }
    public void CameraFullMapMaxPreview()
    {
        Vector3 offset = GameObject.Find("CamPathManager").transform.position - script.transform.position;
        GameObject go = new GameObject("tmp");
        go.transform.position = GameObject.Find("Camera Full Map Max").transform.position - offset;
        go.transform.rotation = GameObject.Find("Camera Full Map Max").transform.rotation;

        if (SceneView.currentDrawingSceneView)
        {
            SceneView.currentDrawingSceneView.AlignViewToObject(go.transform);
        }
        

        GameObject.DestroyImmediate(go);
    
    }
    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.currentDrawingSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }
    public void OnSceneGUI()
    {

        script.position = script.transform.position;


        for (int i = 0; i < script.transform.parent.childCount; i++)
        {
            CamPathCharactorPosition obj = script.transform.parent.GetChild(i).GetComponent<CamPathCharactorPosition>();
            if (obj)
            {

                //Handles.PositionHandle(obj.transform.position, obj.transform.rotation);
                Handles.SphereCap(0, obj.transform.position, Quaternion.identity, 1);
                Handles.Label(obj.transform.position, obj.gameObject.name, style);
            }
            else
            {
                //monster
            }
        }


        //Handles.Label(script.transform.position, script.gameObject.name, style);
        //Handles.SphereCap(0, script.transform.position, Quaternion.identity, 1);


    }
}
