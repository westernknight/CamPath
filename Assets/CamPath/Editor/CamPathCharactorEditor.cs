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
[CustomEditor(typeof(CamPathCharactor))]
public class CamPathCharactorEditor : Editor
{
    CamPathCharactor script;
    GUIStyle style = new GUIStyle();
    public void OnEnable()
    {
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        script = (CamPathCharactor)target;

        bool hasPositionStart = false;
        for (int i = 0; i < script.transform.childCount; i++)
        {
            if (script.transform.GetChild(i).name == "Position " + "Start")
            {
                hasPositionStart = true;
            }
        }
        if (hasPositionStart == false)
        {
            GameObject camGO = new GameObject("Position " + "Start");
            camGO.transform.parent = script.transform;
            camGO.transform.position = script.transform.position;
            camGO.transform.rotation = script.transform.rotation;
            camGO.transform.localScale = Vector3.one;
            camGO.AddComponent<CamPathCharactorPosition>().position = camGO.transform.position;
        }
        else
        {
            GameObject start = GameObject.Find("Position Start");
            start.transform.localPosition = Vector3.zero;
            start.GetComponent<CamPathCharactorPosition>().position = start.transform.position;
            
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Position"))
        {
            CreateNewPosition();
            GetSceneView().Focus();
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(100f)))
        {
            ExtensionMethodsEditor.SaveChildObjectPropertyToJsonFile(script.gameObject);
            //SaveConfig();
        }
        if (GUILayout.Button("Load", GUILayout.Width(100f)))
        {
          
            ExtensionMethodsEditor.ReadJsonFileDataToChildObjectProperty(script.gameObject);
            script.TempAllChildPosInEditor();
            //ReadConfig();
        }
        EditorGUILayout.EndHorizontal();
    }
    public void OnSceneGUI()
    {
    
        for (int i = 0; i < script.transform.childCount; i++)
        {
            CamPathCharactorPosition obj = script.transform.GetChild(i).GetComponent<CamPathCharactorPosition>();
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
        script.ResetAllChildPosInEditor();
    }
    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.currentDrawingSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }
    public void CreateNewPosition()
    {
        GameObject camGO = new GameObject("Position " + (script.transform.childCount == 0 ? "Start" : script.transform.childCount.ToString())  );
        camGO.transform.parent = script.transform;
        camGO.transform.position = script.transform.position;
        camGO.transform.rotation = script.transform.rotation;
        camGO.transform.localScale = Vector3.one;
        camGO.AddComponent<CamPathCharactorPosition>().position = camGO.transform.position;

//         GameObject preView = new GameObject("PreView");
//         preView.transform.parent = camGO.transform;
//         preView.transform.localPosition = Vector3.zero;
//         preView.transform.localRotation = Quaternion.identity;
//         preView.transform.localScale = Vector3.one;
//         preView.AddComponent<CamPathCharactorPositionPreView>();

        Undo.RegisterCreatedObjectUndo(camGO, "Created Position");
        Selection.activeGameObject = camGO;
    }
}
