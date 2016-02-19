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
    /// PC上的偏移值
    /// </summary>
    public float pcDeviation = 10;

    /// <summary>
    /// 安卓手机操作的偏移值
    /// </summary>
    public float androidDeviation = 1000;

    /// <summary>
    /// 安卓手机上的误差极限值
    /// </summary>
    public float mobileThreshold = 15.0f;

    /// <summary>
    /// 滑动值
    /// </summary>
    public Vector3 slide;

    /// <summary>
    /// 缩放值
    /// </summary>
    public float zoom;

    #endregion

    #region Protected Variable Region
    #endregion

    #region Private Variable Region

    private static TouchScript _instance;
    private float zoom_init_distance;
    private Vector2 slide_init_vector;

    #endregion

    #region System Callback Function Region

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        GetSlideInPC();
#elif UNITY_ANDROID ||UNITY_IPHONE
        GetSlideInMobile();
#endif
    }

    #endregion

    #region Public Custom Function Region

    /// <summary>
    /// 判断是否在滑动
    /// </summary>
    /// <returns>是否在滑动</returns>
    public bool IsSliding()
    {
        Debug.Log("这个slide的值是："+slide+"，它等于zero不："+(slide == Vector3.zero));
        return slide != Vector3.zero;
    }

    public bool IsZooming()
    {
        return zoom != 0;
    }

    #endregion

    #region Protected Custom Function Region
    #endregion

    #region Private Custom Function Region

    private void Initialize()
    {
        Instance = this;
        zoom_init_distance = 0;
        slide_init_vector = Vector2.zero;
        slide = Vector3.zero;
        zoom = 0;
    }

    private void GetSlideInPC()
    {
        //if (!Input.GetMouseButton(0))
        //{
        //    slide = Input.mousePosition;
        //}
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            zoom = Input.GetAxis("Mouse ScrollWheel") / pcDeviation;
        }
        else
        {
            slide = Vector3.zero;
            zoom = 0;
        }
    }

    private void GetSlideInMobile()
    {
        if (Input.touchCount == 0)
        {
            zoom_init_distance = 0;
            slide_init_vector = Vector2.zero;
            zoom = zoom_init_distance;
            slide = slide_init_vector;
        }
        else if (Input.touchCount == 1)
        { //Slide
            if (slide_init_vector == Vector2.zero)
            {
                slide_init_vector = Input.GetTouch(0).position;
            }
            else
            {
                var new_distance = Input.GetTouch(0).position - slide_init_vector;
                //slide = new_distance / androidDeviation;
                slide_init_vector = new_distance;
            }
        }
        else
        { //Zoom
            if (zoom_init_distance == 0)
            {
                zoom_init_distance = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
            }
            else
            {
                var new_distance = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;

                if (Math.Abs(new_distance - zoom_init_distance) > mobileThreshold)
                {
                    zoom = (new_distance - zoom_init_distance) / androidDeviation;
                    zoom_init_distance = new_distance;
                }
                else
                {
                    zoom = 0;
                }
            }
        }
    }

    #endregion
}
