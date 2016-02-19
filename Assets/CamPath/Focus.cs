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

public class Focus : MonoBehaviour
{
    [HideInInspector]
    public float distance = 3;
    Vector3 lastMousePosition;

    public GameObject focusObject;
    [Range(0,1)]
    public float rollParam = 1;

    public float accumulateMax = 0.5f;
    float accumulateZoom = 0.1f;
    /// <summary>
    /// 初始化Pos
    /// </summary>
    Vector3 maxPos;
    /// <summary>
    /// 初始化Pos
    /// </summary>
    Vector3 minPos;
    /// <summary>
    /// 初始化Pos
    /// </summary>
    Quaternion maxRot;
    /// <summary>
    /// 初始化Pos
    /// </summary>
    Quaternion minRot;

    /// <summary>
    /// 没有缩放的实际Pos
    /// </summary>
    Vector3 acturallyPos;

    public LayerMask mask = -1;
    void Start()
    {
        //test
       // focusWorldPosition = transform.forward * 3+transform.position;
        //transform.LookAt(focusWorldPosition);


        focusObject = GameObject.Find(CamPathManager.instance.charactorName);
        if (focusObject)
        {
            distance = transform.InverseTransformPoint(focusObject.transform.position).z;
        }

        maxPos = GameObject.Find(CamPathManager.instance.fullMapCameraMaxName).transform.position;
        minPos = GameObject.Find(CamPathManager.instance.fullMapCameraMinName).transform.position;
        maxRot = GameObject.Find(CamPathManager.instance.fullMapCameraMaxName).transform.rotation;
        minRot = GameObject.Find(CamPathManager.instance.fullMapCameraMinName).transform.rotation;
        acturallyPos = maxPos;
        accumulateZoom = accumulateMax;
    }
    bool IsMoveCamera()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Input.GetMouseButton(0) && (Input.touchCount == 1);
        }
        else
        {
            return Input.GetMouseButton(0);
        }
    }
    void Update()
    {
        if (GetComponent<TouchScript>().IsZooming())
        {
            Debug.Log(GetComponent<TouchScript>().zoom);
            accumulateZoom += -GetComponent<TouchScript>().zoom;
            if (accumulateZoom > accumulateMax)
            {
                accumulateZoom = accumulateMax;
            }
            else if (accumulateZoom<0)
            {
                accumulateZoom = 0;
            }

            rollParam = accumulateZoom / accumulateMax;
            lastMousePosition = Vector3.zero;
        }
        else if (IsMoveCamera())
        {

            Vector3 lerpedPos = Vector3.Lerp(minPos, maxPos, rollParam);
            transform.position = lerpedPos;
            if (focusObject)
            {
                distance = transform.InverseTransformPoint(focusObject.transform.position).z;
            }
            transform.position = acturallyPos;

            if (lastMousePosition == Vector3.zero)
            {
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector3 newMousePos = Input.mousePosition;
                Vector3 delta = newMousePos - (Vector3)lastMousePosition;
                float hfovy = Camera.main.fieldOfView / 2;

                float z = Screen.height / 2 / Mathf.Tan(hfovy * Mathf.Deg2Rad);

                Vector3 brelpos = new Vector3();


                brelpos.z = -delta.y / z * distance;//原来是brelpos.y = -delta.y / z * distance ;转平面，但会产生夹角
                brelpos.x = -delta.x / z * distance;

                float angle = Vector3.Angle(Vector3.up, transform.up);

                brelpos.z = brelpos.z * Mathf.Sin(angle * Mathf.Deg2Rad);

       
                transform.position -= brelpos;
                acturallyPos = transform.position;

                lastMousePosition = newMousePos;
            }
        }
        else
        {
            lastMousePosition = Vector3.zero;
        }
        

        

        Vector3 posOffset = acturallyPos - maxPos;
        transform.position = Vector3.Lerp(minPos, maxPos, rollParam) + posOffset;
        transform.rotation = Quaternion.Lerp(minRot,maxRot, rollParam);
    }
}
