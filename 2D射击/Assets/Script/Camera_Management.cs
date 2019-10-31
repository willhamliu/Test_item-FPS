using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Camera_Management : MonoBehaviour
{
    public static Camera_Management camera_Management;
    public Vector3 map_border;//地图边缘
    private float cameraspeed;

    float random_X;
    float random_Y;

    float update_x;//拖动时x轴位置
    float update_y;//拖动时y轴位置
    float move_T;
    bool camera_lock = false;
    public bool calibration { get; set; } = false;
    Vector3 oldTouch = Vector3.zero;
    bool Touch_static = false;
    public Vector3 fire_point { get;  set; }

    void Awake()
    {
        if (camera_Management == null)
        {
            camera_Management = this;
        }
    }
    private void Start()
    {
        Offset();
    }
    void Update()
    {
        Control();
    }
    
    public void Recoil()
    {
        StartCoroutine(_Recoil());
    }
    public void Offset()
    {
        calibration = false;
        StartCoroutine(_Offset());
    }
   
    private IEnumerator _Offset()
    {
        while (calibration == false)
        {

            float range_X = transform.position.x + Random.Range(-0.1f, 0.2f);
            float range_Y = transform.position.y + Random.Range(-1f, 1f);

            Vector3 start_poison = transform.position;

            float offset_T = 0;
            float calibration_T = 0;
           
            while (calibration_T < 1f && Mathf.Abs(range_Y) > 0.5f)
            {
                if (calibration == true)
                {
                    break;
                }
                if (offset_T < 1)
                {
                    offset_T += 0.01f;
                    transform.position = Vector3.Lerp(start_poison, new Vector3(range_X, range_Y, transform.position.z), offset_T);
                }
                else
                {
                    calibration_T += 0.01f;
                    transform.position = Vector3.Lerp(new Vector3(range_X, range_Y, transform.position.z), start_poison, calibration_T);
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
    private IEnumerator _Recoil()
    {
        fire_point = Camera.main.transform.position;
        float recoil_poison = transform.position.y + 2.5f;
        Vector3 back_poison = transform.position;
        float recoil_T = 0;
        float calibration_T = 0;

        if (transform.position.y + recoil_poison * 0.5f < Mathf.Abs(map_border.y))
        {
            while (true)
            {
                if (recoil_T < 1)
                {
                    recoil_T += 0.2f;
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, recoil_poison, transform.position.z), recoil_T);
                }
                else
                {
                    calibration_T += 0.2f;
                    transform.position = Vector3.Lerp(new Vector3(transform.position.x, recoil_poison, transform.position.z), back_poison, calibration_T);
                }
                if (calibration_T == 0.6f)
                {
                    Offset();
                    break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        else
        {
            while (true)
            {
                if (recoil_T < 1)
                {
                    recoil_T += 0.2f;
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, recoil_poison, transform.position.z), recoil_T);
                }
                else
                {
                    calibration_T += 0.2f;
                    transform.position = Vector3.Lerp(new Vector3(transform.position.x, recoil_poison, transform.position.z), back_poison, calibration_T);
                }
                if (calibration_T == 1)
                {
                    Offset();
                    break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }

    private void Control()
    {
#if UNITY_EDITOR_WIN
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Vector3 newTouch = Input.mousePosition;
            if (Vector3.Distance(newTouch, oldTouch)>1)
            {
                Touch_static = false;
                calibration = true;
                update_x = Mathf.Clamp(transform.position.x + Input.GetAxis("Mouse X"), map_border.x, -map_border.x);
                update_y = Mathf.Clamp(transform.position.y + Input.GetAxis("Mouse Y"), map_border.y, -map_border.y);
                move_T = Mathf.Clamp(move_T += Time.deltaTime * 0.1f, 0f, 0.2f);
                transform.position = Vector3.Lerp(transform.position, new Vector3(update_x, update_y, transform.position.z), move_T);
            }
            else if(Vector3.Distance(newTouch, oldTouch) < 5&&Touch_static ==false)
            {
                Offset();
                Touch_static = true;
            }
            oldTouch = newTouch;
        }
        if (Input.GetMouseButtonUp(0) && UI_Management.ui_Management.sniper_State == true && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Offset();
            move_T = Mathf.Clamp(move_T += Time.deltaTime, 0f, 1f);//惯性模拟
        }
#endif

#if UNITY_ANDROID
        if (Input.touchCount == 1&& camera_lock==false && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false)
        {
            Vector3 newTouch= Input.touches[0].position;
            if (Input.touches[0].phase == TouchPhase.Moved&& Vector3.Distance(newTouch, oldTouch) > 1)
            {
                Touch_static = false;
                calibration = true;
                update_x = Mathf.Clamp(transform.position.x + Input.GetAxis("Mouse X")*0.5f, map_border.x, -map_border.x);
                update_y = Mathf.Clamp(transform.position.y + Input.GetAxis("Mouse Y")*0.5f, map_border.y, -map_border.y);
                move_T = Mathf.Clamp(move_T += Time.deltaTime * 0.1f, 0f, 0.2f);
                transform.position = Vector3.Lerp(transform.position, new Vector3(update_x, update_y, transform.position.z), move_T);
                oldTouch = newTouch;
            }
            else if (Vector3.Distance(newTouch, oldTouch) <1 && Touch_static == false)
            {
                Offset();
                Touch_static = true;
            }
            if (Input.touches[0].phase == TouchPhase.Ended && UI_Management.ui_Management.sniper_State == true && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == true)
            {
                Offset();
            }
        }
        if (Input.touchCount == 2)
        {
            camera_lock = true;
        }
        if (Input.touchCount == 0)
        {
            camera_lock = false;
            move_T = Mathf.Clamp(move_T += Time.deltaTime, 0f, 1f);
        }
       
#endif
    }
}
