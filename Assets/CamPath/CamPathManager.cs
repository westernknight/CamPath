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

// 模块描述：CamPathManager的坐标在游戏当中是不会动的
//默认只有一个机位Camera Full Map Max 
//Camera Full Map Min是控制缩放的最小值不算一个机位
public class CamPathManager : MonoBehaviour
{
    /// <summary>
    /// 缩放镜头最大值对象名字
    /// </summary>
    [HideInInspector]
    public string fullMapCameraMaxName = "Camera Full Map Max";
    /// <summary>
    /// 缩放镜头最小值对象名字
    /// </summary>
    [HideInInspector]
    public string fullMapCameraMinName = "Camera Full Map Min";
    /// <summary>
    /// 焦点对象名字
    /// </summary>
    [HideInInspector]
    public string charactorName = "CamPathCharactor";

    /// <summary>
    /// 对齐对象动画处理时私有变量
    /// </summary>
    [HideInInspector]
    bool isAlignToObject = false;
    Vector3 alignPosition;
    Quaternion alignRotation;
    /// <summary>
    /// 对齐对象动画处理时动画百分比
    /// </summary>
    float alignPercent = 0;


    bool isFollowToObject = false;
    GameObject followToObject;

    public static CamPathManager instance;
    
    string currentCameraAlignObjectName;

    public enum AlignMethod
    {
        Instantaneous,
        AlignTo,
    }
    public AlignMethod alignMethod = AlignMethod.AlignTo;
    void Awake()
    {
        instance = this;
        alignMethod = AlignMethod.AlignTo;
        //默认第一个机位是"Camera Full Map Max"
        currentCameraAlignObjectName = fullMapCameraMaxName;
    }

    /// <summary>
    /// 游戏镜头使用设计好的机位
    /// </summary>
    /// <param name="cameraName"></param>
    public void AlignViewToCameraName(string cameraName)
    {
        AlignViewToCameraObject(GameObject.Find(cameraName));
    }
    public void AlignViewToCameraObject(GameObject obj)
    {
        alignPosition = obj.transform.position;
        alignRotation = obj.transform.rotation;
        if (alignMethod == AlignMethod.Instantaneous)
        {
            Camera.main.transform.position = alignPosition;
            Camera.main.transform.rotation = alignRotation;
        }
        else if (alignMethod == AlignMethod.AlignTo)
        {
            if (isAlignToObject)
            {
                LeanTween.cancel(gameObject);
                isAlignToObject = false;
            }

            if (isAlignToObject == false)
            {
                isAlignToObject = true;
                LeanTween.value(gameObject, 0, 1, 0.8f).setOnUpdate((float f) => { alignPercent = f; }).setOnComplete(() => { isAlignToObject = false; alignPercent = 0; });
            }
        }

    }
    /// <summary>
    /// 让游戏镜头的焦点设为某个位置
    /// </summary>
    /// <param name="positionName"></param>
    public void FocusTo(string positionName)
    {
        FocusTo(GameObject.Find(positionName));
    }
    public void FocusTo(GameObject obj)
    {
        Vector3 offset = transform.position - obj.transform.position;

        alignPosition = GameObject.Find(currentCameraAlignObjectName).transform.position - offset;
        alignRotation = GameObject.Find(currentCameraAlignObjectName).transform.rotation;


        if (alignMethod == AlignMethod.Instantaneous)
        {
            Camera.main.transform.position = alignPosition;
            Camera.main.transform.rotation = alignRotation;
        }
        else if (alignMethod == AlignMethod.AlignTo)
        {

            if (isAlignToObject)
            {
                LeanTween.cancel(gameObject);
                isAlignToObject = false;
            }
            

            if (isAlignToObject == false)
            {
                isAlignToObject = true;
                LeanTween.value(gameObject, 0, 1, 0.8f).setOnUpdate((float f) => { alignPercent = f; }).setOnComplete(() => { isAlignToObject = false; alignPercent = 0; });
            }
        }

    }
    void Start()
    {

    }

    void Update()
    {
        if (isAlignToObject && alignMethod == AlignMethod.AlignTo)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, alignPosition, alignPercent);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, alignRotation, alignPercent);
        }
    }
}
