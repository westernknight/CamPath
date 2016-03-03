using UnityEngine;
using System.Collections;
using System;

public class TouchScript : MonoBehaviour
{
    #region Public Variable Region

    /// <summary>
    /// 单例
    /// </summary>
    public static TouchScript Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    /// <summary>
    /// 触摸操作
    /// </summary>
    public enum ETouchOperation
    {
        Nothing,
        Sliding,
        Zooming
    }

    /// <summary>
    /// PC上的偏移值
    /// </summary>
    public float pcDeviation = 10;

    /// <summary>
    /// 安卓手机缩放偏移值
    /// </summary>
    public float androidZoomingDeviation = 1000;

    /// <summary>
    /// 安卓手机滑动偏移值
    /// </summary>
    public float androidSlidingDeviation = 10;

    /// <summary>
    /// 安卓手机上的误差极限值
    /// </summary>
    public float mobileThreshold = 20.0f;

    /// <summary>
    /// 滑动值
    /// </summary>
    public Vector3 slide;

    /// <summary>
    /// 缩放值
    /// </summary>
    public float zoom;

    /// <summary>
    /// 是否允许手势操作
    /// </summary>
    public bool isTouchAllow;
    /// <summary>
    /// 操作识别
    /// </summary>
    public ETouchOperation touchOperation;
    #endregion

    #region Private Variable Region

    
    private static TouchScript _instance;
    private float zoom_init_distance;
    private Vector3 slide_init_vector;

    #endregion

    #region System Callback Function Region

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (isTouchAllow)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            GetSlideInPC();
#elif UNITY_ANDROID || UNITY_IPHONE
            GetSlideInMobile();
#endif
        }
    }

    #endregion

    #region Public Custom Function Region

    /// <summary>
    /// 判断是否在滑动
    /// </summary>
    /// <returns>是否在滑动</returns>
    public bool IsSliding()
    {
        return touchOperation == ETouchOperation.Sliding;
    }

    public bool IsZooming()
    {
        return touchOperation == ETouchOperation.Zooming;
    }

    #endregion

    #region Protected Custom Function Region
    #endregion

    #region Private Custom Function Region

    private void Initialize()
    {
        Instance = this;
        zoom_init_distance = 0;
        slide_init_vector = Vector3.zero;
        slide = Vector3.zero;
        zoom = 0;
        SetTouchOperation(ETouchOperation.Nothing);
        isTouchAllow = true;
    }


    private void GetSlideInPC()
    {
        if (!Input.GetMouseButton(0) && Input.GetAxis("Mouse ScrollWheel") == 0)
        {

            slide = Vector3.zero;
            zoom = 0;
            slide_init_vector = Vector3.zero;
            SetTouchOperation(ETouchOperation.Nothing);
        }
        else if (Input.GetMouseButton(0))
        {
            if (slide_init_vector == Vector3.zero)
            {
                slide_init_vector = Input.mousePosition;
            }
            else
            {
                
                if ((slide_init_vector - Input.mousePosition).sqrMagnitude>1 || touchOperation == ETouchOperation.Sliding)
                {
                    slide = slide_init_vector - Input.mousePosition;
          
                    slide_init_vector = Input.mousePosition;
                    SetTouchOperation(ETouchOperation.Sliding);
                }
                else
                {
                    slide = Vector3.zero;
                    slide_init_vector = Input.mousePosition;
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            zoom = Input.GetAxis("Mouse ScrollWheel") / pcDeviation;
            SetTouchOperation(ETouchOperation.Zooming);
        }
    }

    private void GetSlideInMobile()
    {
        if (Input.touchCount == 0)
        {
            zoom_init_distance = 0;
            slide_init_vector = Vector3.zero;
            zoom = zoom_init_distance;
            slide = slide_init_vector;
            SetTouchOperation(ETouchOperation.Nothing);
        }
        else if (Input.touchCount == 1)
        { //Slide
            if (zoom_init_distance != 0)
                zoom_init_distance = 0;

            if (slide_init_vector == Vector3.zero)
            {
                slide_init_vector = Input.GetTouch(0).position;
            }
            else
            {

                if ((slide_init_vector - (Vector3)Input.GetTouch(0).position).sqrMagnitude > 1 || touchOperation == ETouchOperation.Sliding)
                {
                    slide = slide_init_vector - (Vector3)Input.GetTouch(0).position;

                    slide_init_vector = Input.GetTouch(0).position;
                    SetTouchOperation(ETouchOperation.Sliding);
                }
                else
                {
                    slide = Vector3.zero;
                    slide_init_vector = Input.mousePosition;
                }

            }
        }
        else
        { //Zoom
            if (slide_init_vector != Vector3.zero)
                slide_init_vector = Vector3.zero;

            if (zoom_init_distance == 0)
            {
                zoom_init_distance = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
            }
            else
            {
                var new_distance = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;

                if (Math.Abs(new_distance - zoom_init_distance) > mobileThreshold)
                {
                    zoom = (new_distance - zoom_init_distance) / androidZoomingDeviation;
                    zoom_init_distance = new_distance;
                    SetTouchOperation(ETouchOperation.Zooming);
                }
                else
                {
                    zoom = 0;
                    SetTouchOperation(ETouchOperation.Nothing);
                }
            }
        }
    }

    private void SetTouchOperation(ETouchOperation to)
    {
        if (to != touchOperation)
        {
            touchOperation = to;
        }
    }

    #endregion
}
