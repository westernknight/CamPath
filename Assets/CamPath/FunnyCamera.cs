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

[RequireComponent(typeof(TouchScript))]
public class FunnyCamera : MonoBehaviour
{
    public static FunnyCamera instance;
    public enum CameraProperty
    {
        Freedom,//全局
        FollowObject,//焦点跟随物体
        AttachObject,//摄像机位置方向跟随物体
    }
    #region Public Variable Region
    public GameObject cameraPropertyObject;
    /// <summary>
    /// 是否锁上camera不能由玩家操控
    /// </summary>
    public bool isLockInput = false;
    public CameraProperty cameraProperty = CameraProperty.Freedom;
    [Range(0, 1)]
    public float rollParam = 1;

    /// <summary>
    /// 缩放最大位移，鼠标滚轮每次增加0.1左右
    /// </summary>
    public float accumulateZoomMax = 0.5f;
    /// <summary>
    /// 玩家是否在控制镜头，如果是，不对游戏角色进行操控
    /// </summary>
    public bool isPlayerCharging = false;

    public float boundingSphereRadius = 0.5f;

    public LayerMask collisionMask = -1;

    #endregion

    #region Private Variable Region
    /// <summary>
    /// 焦点对象，默认是CamPathManager.instance.charactorName
    /// </summary>
    GameObject focusObject;
    /// <summary>
    /// 记录鼠标上次的位置
    /// </summary>
    Vector3 lastMousePosition;
    /// <summary>
    /// 摄像机到焦点的距离
    /// </summary>
    [HideInInspector]
    float distance = 3;
    /// <summary>
    /// 为了使用transform的函数算法，摄像机底下增加了一个calculationParam的对象用于算法使用
    /// </summary>
    GameObject calculationParam;
    /// <summary>
    /// 没有缩放的实际up camera Pos
    /// </summary>
    Vector3 upCameraPosition;
    /// <summary>
    /// 缩放累计值
    /// </summary>
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
    /// 没有缩放的实际 Pos
    /// </summary>
    Vector3 actuallyPos;

    /// <summary>
    /// 屏幕width/2转为世界坐标的宽度
    /// </summary>
    float cameraWorldWidth;
    /// <summary>
    /// 屏幕height/2转为世界坐标的宽度
    /// </summary>
    float cameraWorldHeight;

    /// <summary>
    /// 记录地图的大小
    /// </summary>
    Vector2 mapSize = new Vector2();
    /// <summary>
    /// 初始化顶视角camera的位置，是通过fullMax camera与焦点计算出来的 
    /// </summary>
    Vector3 initUpCameraPosition;

    bool isLerpTarget = false;
    Vector3 targetPosition;
    Quaternion targetRotation;

    //fov
    float hfovRad;
    float wfovRad;
    float hfovAngle;
    float wfovAngle;

    
    #endregion

    #region System Callback Function Region
    void Awake()
    {
        instance = this;
    }
    void Start()
    {

        maxPos = GameObject.Find(CamPathManager.instance.fullMapCameraMaxName).transform.position;
        minPos = GameObject.Find(CamPathManager.instance.fullMapCameraMinName).transform.position;
        maxRot = GameObject.Find(CamPathManager.instance.fullMapCameraMaxName).transform.rotation;
        minRot = GameObject.Find(CamPathManager.instance.fullMapCameraMinName).transform.rotation;

       


        transform.position = actuallyPos = maxPos;
        transform.rotation = maxRot;
        isLerpTarget = false;
        TargetToPositionAndRotation(transform.position, transform.rotation);
        focusObject = GameObject.Find(CamPathManager.instance.charactorName);
        if (focusObject)
        {
            distance = Vector3.Distance(transform.position, focusObject.transform.position);
        }
        accumulateZoom = accumulateZoomMax;
        calculationParam = new GameObject("calc");
        calculationParam.transform.parent = transform;
        calculationParam.transform.localRotation = Quaternion.identity;
        if (focusObject)
        {
            initUpCameraPosition = upCameraPosition = distance * focusObject.transform.up + focusObject.transform.position;
        }
  

        hfovRad = Camera.main.fieldOfView / 2 * Mathf.Deg2Rad;
        wfovRad = Mathf.Atan((float)Screen.width / (float)Screen.height * Mathf.Tan(hfovRad));
        hfovAngle = Camera.main.fieldOfView / 2;
        wfovAngle = wfovRad * Mathf.Rad2Deg;

        cameraWorldWidth = Mathf.Tan(wfovRad) * distance;
        cameraWorldHeight = Mathf.Tan(hfovRad) * distance;

        MapManager.Init();
        mapSize.x = MapManager.Instance.cellDiameterSize * MapManager.Instance.totalColNum;
        mapSize.y = MapManager.Instance.cellDiameterSize * MapManager.Instance.totalRowNum;




    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Vector3 focusPoint = transform.forward * distance + transform.position;
            Gizmos.DrawWireSphere(focusPoint, 0.5f);
            Gizmos.DrawWireSphere(upCameraPosition, 0.5f);


            Gizmos.DrawWireSphere(transform.position, boundingSphereRadius);

  
            //画出摄像机四边到平面的4个点
            //Vector3 tmp = upCameraPosition;
            //tmp.y = 0;
            //Gizmos.DrawWireSphere(tmp - point1, 0.5f);
            //Gizmos.DrawWireSphere(tmp - point2, 0.5f);
            //Gizmos.DrawWireSphere(tmp - point3, 0.5f);
            //Gizmos.DrawWireSphere(tmp - point4, 0.5f);

        }


    }
    /// <summary>
    /// 摄像机移动补帧
    /// </summary>
    /// <param name="targetVector"></param>
    /// <param name="targetQuaternion"></param>
    void TargetToPositionAndRotation(Vector3 targetVector, Quaternion targetQuaternion)
    {
        if (isLerpTarget)
        {
            targetPosition = targetVector;
            targetRotation = targetQuaternion;
        }
        else
        {
            targetPosition = targetVector;
            targetRotation = targetQuaternion;
            transform.position = targetVector;
            transform.rotation = targetQuaternion;
        }

    }
    bool trigger = false;
    Vector3 lastColPos;
    void OnTriggerEnter(Collider col)
    {

        trigger = true;
        
    }
    void OnTriggerExit(Collider col)
    {
        trigger = false;
    }

    void Update()
    {


        ///是否用缓动
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        
      

        if (isLockInput)
        {
            return;
        }
        if (cameraProperty == CameraProperty.AttachObject)
        {
            if (cameraPropertyObject != null)
            {
                isLerpTarget = false;
                TargetToPositionAndRotation(cameraPropertyObject.transform.position, cameraPropertyObject.transform.rotation);
            }
            return;
        }
        else if (cameraProperty == CameraProperty.FollowObject)
        {
            if (cameraPropertyObject != null)
            {
                isLerpTarget = true;
                Vector3 tmp = upCameraPosition;
                tmp = cameraPropertyObject.transform.position;
                tmp.y = upCameraPosition.y;
                ReCalculateCameraPosition(tmp);
            }
        }
        else if (cameraProperty == CameraProperty.Freedom)
        {
            isLerpTarget = false;
        }
        if (TouchScript.Instance.IsZooming())
        {
            isPlayerCharging = true;
            accumulateZoom += -TouchScript.Instance.zoom;
            if (accumulateZoom > accumulateZoomMax)
            {
                accumulateZoom = accumulateZoomMax;
            }
            else if (accumulateZoom < 0)
            {
                accumulateZoom = 0;
            }
            float tmpRoll = rollParam;
            float tmpDistance = distance;
            rollParam = accumulateZoom / accumulateZoomMax;
            lastMousePosition = Vector3.zero;
            if (focusObject)
            {
                distance = Vector3.Distance(focusObject.transform.position, Vector3.Lerp(minPos, maxPos, rollParam));
            }
            bool success = ReCalculateCameraPosition(upCameraPosition);
            if (!success)
            {
                rollParam = tmpRoll;
                distance = tmpDistance;
            }
        }
        else if (TouchScript.Instance.IsSliding() && cameraProperty == CameraProperty.Freedom)
        {

            isPlayerCharging = true;
            if (lastMousePosition == Vector3.zero)
            {
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector3 newMousePos = Input.mousePosition;
                Vector3 delta = TouchScript.Instance.slide;
                float hfovy = Camera.main.fieldOfView / 2;

                float z = Screen.height / 2 / Mathf.Tan(hfovy * Mathf.Deg2Rad);

                Vector3 brelpos = new Vector3();


                brelpos.x = delta.y / z * distance;//原来是brelpos.y = -delta.y / z * distance ;转平面，但会产生夹角
                brelpos.z = -delta.x / z * distance;

    
                //控制摄像机范围
                Vector3 direct = Quaternion.AngleAxis(CamPathManager.instance.mapAngle, Vector3.up) * (-brelpos);
                ReCalculateCameraPosition(upCameraPosition + direct);

                lastMousePosition = newMousePos;
            }
        }
        else
        {
            isPlayerCharging = false;
            lastMousePosition = Vector3.zero;
        }
       
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns>计算后是否改变了Camera，没有改变说明Camera碰撞了</returns>
    bool ReCalculateCameraPosition(Vector3 targetPos)
    {
        //计算roll之后的local pos
        calculationParam.transform.position = initUpCameraPosition;
        Vector3 lerpCameraLocalPos = calculationParam.transform.InverseTransformPoint(Vector3.Lerp(minPos, maxPos, rollParam));



        Vector3 tmp = upCameraPosition;
        upCameraPosition = targetPos;
        //4-3
        //| |
        //1-2

#if false //第二种边界算法
      if ((tmp - point4).z < 0)
        {
            upCameraPosition.z += -(tmp - point4).z;
        }
        if ((tmp - point4).x < 0)
        {
            upCameraPosition.x += -(tmp - point4).x;
        }
        if ((tmp - point2).z > mapSize.x)
        {

            upCameraPosition.z -= (tmp - point2).z - mapSize.x;
        }
        if ((tmp - point2).x > mapSize.y)
        {
            upCameraPosition.x -= (tmp - point2).x - mapSize.y;
        }
#else
        //|<-z轴->|   x轴
        if (upCameraPosition.z < cameraWorldWidth)
        {
            upCameraPosition.z = cameraWorldWidth;
        }
        else if (upCameraPosition.z > mapSize.x - cameraWorldWidth)
        {
            upCameraPosition.z = mapSize.x - cameraWorldWidth;
        }
        if (upCameraPosition.x < cameraWorldHeight)
        {
            upCameraPosition.x = cameraWorldHeight;
        }
        else if (upCameraPosition.x > mapSize.y - cameraWorldHeight)
        {
            upCameraPosition.x = mapSize.y - cameraWorldHeight;
        }
#endif
        //ray cast to floor
        
       calculationParam.transform.position = upCameraPosition;

      

        //local pos转为世界坐标就是实际坐标actuallyPos
        
        actuallyPos = calculationParam.transform.TransformPoint(lerpCameraLocalPos);

  
        RaycastHit hitInfo;
        if (!Physics.SphereCast(transform.position, 0.5f, actuallyPos - transform.position, out hitInfo, (actuallyPos - transform.position).magnitude, collisionMask))
        {
            TargetToPositionAndRotation(actuallyPos, Quaternion.Lerp(minRot, maxRot, rollParam));
            return true;
        }
        else
        {
            upCameraPosition = tmp;
            return false;
        }
    }



    #endregion

    #region Private Custom Function Region



#if false
       private void CentrePositionTrace()
    {
        centrePositionRay.origin = transform.position;
        centrePositionRay.direction = transform.forward;



        if (Physics.Raycast(centrePositionRay, out rayInfo))
        {
            //其实这里是做击中判断操作
            centrePosition = rayInfo.point;
        }
    }
    private void MapLitmitCalculate()
    {
        real_width = distance * Screen.width * Mathf.Tan(Camera.main.fieldOfView / 2 * Mathf.Deg2Rad) / Screen.height;
        real_height = distance * Mathf.Tan(Camera.main.fieldOfView / 2 * Mathf.Deg2Rad);
        centrePositionRay = new Ray(transform.position, transform.forward);
    }

    private void MapLitmitJudge(ref Vector3 Pos)
    {
        return;
        if (centrePosition.z + Mathf.Abs(Pos.z) + real_width > litmit_maxZ)
        {
            Pos.z -= centrePosition.z + Mathf.Abs(Pos.z) + real_width - litmit_maxZ;
            //Pos.z = litmit_maxZ - real_width;
        }
        if (centrePosition.z - Mathf.Abs(Pos.z) - real_width < litmit_minZ)
        {
            Pos.z += litmit_minZ - (centrePosition.z - Mathf.Abs(Pos.z) - real_width);
        }
        if (centrePosition.x + Mathf.Abs(Pos.x) + real_height > litmit_maxX)
        {
            Pos.x -= centrePosition.x + Mathf.Abs(Pos.x) + real_height - litmit_maxX;
        }
        if (centrePosition.x - Mathf.Abs(Pos.x) - real_height < litmit_minX)
        {
            Pos.x += litmit_minX - (centrePosition.x - Mathf.Abs(Pos.x) - real_height);
        }
    }
#endif

    #endregion
}
