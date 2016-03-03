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

public class CamPathCharactor : MonoBehaviour
{
 

    //读取数据后，要缓存位置
    public void TempAllChildPosInEditor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CamPathCharactorPosition campath = transform.GetChild(i).GetComponent<CamPathCharactorPosition>();
            if (campath==null)
            {
                campath = transform.GetChild(i).gameObject.AddComponent<CamPathCharactorPosition>();
            }
            campath.position = transform.GetChild(i).position;
        }
    }
    public void ResetAllChildPosInEditor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).position!=transform.GetChild(i).GetComponent<CamPathCharactorPosition>().position)
            {
                transform.GetChild(i).position = transform.GetChild(i).GetComponent<CamPathCharactorPosition>().position;
            }
           
        }
    }
    public void CreateStartPosition()
    {
        GameObject camGO = new GameObject("Position " + "Start");
        camGO.transform.parent = transform;
        camGO.transform.position = transform.position;
        camGO.transform.rotation = transform.rotation;
        camGO.transform.localScale = Vector3.one;
        camGO.AddComponent<CamPathCharactorPosition>().position = camGO.transform.position;



    }
}
