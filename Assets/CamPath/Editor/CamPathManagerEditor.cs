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
using System.IO;


/// <summary>
/// 镜头根据CamPathManager的Camera Charactor来移动
/// </summary>
[CustomEditor(typeof(CamPathManager))]
public class CamPathManagerEditor : Editor
{

    private CamPathManager script;
    GUIStyle style = new GUIStyle();
    void OnEnable()
    {
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        script = (CamPathManager)target;

        CreateChar();
        

        GameObject charactor = GameObject.Find(script.charactorName);
        if (charactor)
        {
           script. transform.position = charactor.transform.position;
        }


        CreateFullMapCamera();

        CreateFunnyCamera();
    }
    void CreateFunnyCamera()
    {
        if (Camera.main == null)
        {
            GameObject tmp = new GameObject("Camera", typeof(Camera), typeof(AudioListener));
            tmp.tag = "MainCamera";
            GameObject obj = GameObject.Find(script.fullMapCameraMaxName);

            Camera.main.transform.position = obj.transform.position;
            Camera.main.transform.rotation = obj.transform.rotation;
        }
        {
            FunnyCamera cam = Camera.main.GetComponent<FunnyCamera>();
            if (cam == null)
            {
                Camera.main.gameObject.AddComponent<FunnyCamera>();
            }
        }
       
    }
    /// <summary>
    /// 创建2个基本的摄像头
    /// </summary>
    void CreateFullMapCamera()
    {
        //search for a manager object within current scene
        GameObject camera = GameObject.Find(script.fullMapCameraMaxName);

        //if no manager object was found
        if (camera == null)
        {
            //create a new gameobject with that name
            if (SceneView.currentDrawingSceneView)
            {
                camera = new GameObject(script.fullMapCameraMaxName);
                camera.transform.parent = script.transform;
                GameObject charactor = GameObject.Find(script.charactorName);
                //camera.transform.position = SceneView.currentDrawingSceneView.camera.transform.position;
                //camera.transform.rotation = SceneView.currentDrawingSceneView.camera.transform.rotation;
                camera.transform.position = (charactor.transform.up + charactor.transform.right).normalized * 15 + charactor.transform.position;
                camera.transform.LookAt(charactor.transform);

                camera.transform.localScale = Vector3.one;
                camera.AddComponent<CamPathObject>();

                GameObject preView = new GameObject("PreView");
                preView.transform.parent = camera.transform;
                preView.transform.localPosition = Vector3.zero; 
                preView.transform.localRotation = Quaternion.identity;
                preView.transform.localScale = Vector3.one;
                preView.AddComponent<CamPathPreView>();

                camera = new GameObject(script.fullMapCameraMinName);
                camera.transform.parent = script.transform;
                //camera.transform.position = SceneView.currentDrawingSceneView.camera.transform.position;
                //camera.transform.rotation = SceneView.currentDrawingSceneView.camera.transform.rotation;
                camera.transform.position = (charactor.transform.up + charactor.transform.right).normalized * 14 + charactor.transform.position;
                camera.transform.LookAt(charactor.transform);
                camera.transform.localScale = Vector3.one;
                camera.AddComponent<CamPathObject>();

                preView = new GameObject("PreView");
                preView.transform.parent = camera.transform;
                preView.transform.localPosition = Vector3.zero;
                preView.transform.localRotation = Quaternion.identity;
                preView.transform.localScale = Vector3.one;
                preView.AddComponent<CamPathPreView>();
                //如果有，就读取配置
                //ReadConfig();
            }

        }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();







        GUILayout.Label("Camera Editor");
        if (GUILayout.Button("New Camera", GUILayout.Width(100f)))
        {
            CreateNewCamera();
            GetSceneView().Focus();
        }


        GUILayout.Label("Camera Full Map Config");
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save", GUILayout.Width(100f)))
        {
            ExtensionMethodsEditor.SaveChildObjectPropertyToJsonFile(script.gameObject);
        }
        if (GUILayout.Button("Load", GUILayout.Width(100f)))
        {

            ExtensionMethodsEditor.ReadJsonFileDataToChildObjectProperty(script.gameObject);
            for (int i = 0; i < script.transform.childCount; i++)
            {
                script.transform.GetChild(i).gameObject.AddComponent<CamPathObject>();

                GameObject preView = new GameObject("PreView");
                preView.transform.parent = script.transform.GetChild(i).gameObject.transform;
                preView.transform.localPosition = Vector3.zero;
                preView.transform.localRotation = Quaternion.identity;
                preView.transform.localScale = Vector3.one;
                preView.AddComponent<CamPathPreView>();

            }
            GameObject charactor = GameObject.Find(script.charactorName);
            if (charactor)
            {
                script.transform.position = charactor.transform.position;
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("\nTest");

        for (int i = 0; i < script.transform.childCount; i++)
        {
            CamPathObject obj = script.transform.GetChild(i).GetComponent<CamPathObject>();
            if (obj)
            {
                if (GUILayout.Button(obj.name))
                {
                    if (Application.isPlaying)
                    {
                        script.AlignViewToCameraObject(obj.gameObject);
                    }
                    else
                    {
                        Camera.main.transform.position = obj.transform.position;
                        Camera.main.transform.rotation = obj.transform.rotation;
                    }

                }
            }
        }
    }
    public void OnSceneGUI()
    {
        for (int i = 0; i < script.transform.childCount; i++)
        {
            CamPathObject obj = script.transform.GetChild(i).GetComponent<CamPathObject>();
            if (obj)
            {
                Handles.Label(obj.transform.position, obj.gameObject.name, style);
                Handles.PositionHandle(obj.transform.position, obj.transform.rotation);
            }
            else
            {
                //monster
            }
        }

    }
    void CreateChar()
    {

        //search for a manager object within current scene

        

        GameObject charactor = GameObject.Find(script.charactorName);

        //if no manager object was found
        if (charactor == null)
        {
            //create a new gameobject with that name
            if (SceneView.currentDrawingSceneView)
            {
                Vector3 pos = SceneView.currentDrawingSceneView.camera.transform.TransformVector(0, 0, 5);
                charactor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                charactor.GetComponent<BoxCollider>().enabled = false;
                charactor.GetComponent<MeshRenderer>().enabled = false;
                charactor.name = script.charactorName;
                charactor.transform.position = pos + SceneView.currentDrawingSceneView.camera.transform.position;
                charactor.AddComponent<CamPathCharactor>().CreateStartPosition() ;

                Selection.activeGameObject = charactor;
            }

        }
    }
    public void CreateNewCamera()
    {
        GameObject camGO = new GameObject("Camera " + script.transform.childCount);
        camGO.transform.parent = script.transform;
        camGO.transform.position = SceneView.currentDrawingSceneView.camera.transform.position;
        camGO.transform.rotation = SceneView.currentDrawingSceneView.camera.transform.rotation;
        camGO.transform.localScale = Vector3.one;
        camGO.AddComponent<CamPathObject>();

        GameObject preView = new GameObject("PreView");
        preView.transform.parent = camGO.transform;
        preView.transform.localPosition = Vector3.zero;
        preView.transform.localRotation = Quaternion.identity;
        preView.transform.localScale = Vector3.one;
        preView.AddComponent<CamPathPreView>();

        Undo.RegisterCreatedObjectUndo(camGO, "Created Camera");
        Selection.activeGameObject = camGO;

    }
    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.currentDrawingSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }
}
public class CreateCamPathManager : EditorWindow
{
    [MenuItem("Window/地图管理器/CamPath Manager")]
    //initialize method
    static void Init()
    {
        string managerName = "CamPathManager";
        //search for a manager object within current scene
        GameObject manager = GameObject.Find(managerName);

        //if no manager object was found
        if (manager == null)
        {
            //create a new gameobject with that name
            manager = new GameObject(managerName);
            manager.AddComponent<CamPathManager>();
            Undo.RegisterCreatedObjectUndo(manager, "Created Manager");
        }
        else if (manager.GetComponent<CamPathManager>() == null)
        {
            manager.AddComponent<CamPathManager>();
        }
        //in both cases, select the gameobject
        Selection.activeGameObject = manager;
    }
}