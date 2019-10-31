using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Management : Player_Control
{
    public static UI_Management ui_Management;

    public Toggle Fire_State_Toggle;
    private Toggle[] zoom_Show_UI;

    public GameObject breath_Button;
    public GameObject background;
    public GameObject quit_Panel;
    public Text Bullet_count_UI;
    public Text Fire_State_Text;
    public Image Fire_State_Mask;
    public Image sights;
    public Image telescope;
    public Transform Zoom_Show;

    private float Camera_fov=9;

    public Slider Breath_Time;
    public Image Breath_ui_color;


    private void Awake()
    {
        if (ui_Management==null)
        {
            ui_Management = this;
        }
        Fire_State_Mask.enabled = false;

        Breath_Time.value = 0;
        bullet_count = MAX_Bullecount;

        Fire_State_Toggle.onValueChanged.AddListener((bool value) => { Sniper_State_update(value); });


        zoom_Show_UI = new Toggle[Zoom_Show.childCount];
        for (int i = 0; i < zoom_Show_UI.Length; i++)
        {
            zoom_Show_UI[i] = Zoom_Show.GetChild(i).GetComponent<Toggle>();
        }
        zoom_Show_UI[zoom_Count].isOn = true;

        Bullet_count_UI.text = bullet_count.ToString();
        prevFire = -fireCD;//避免第一次射击时需要等待
        prevZoom = -zoomCD;
    }
    private void Start()
    {
        quit_Panel.SetActive(false);
    }
    public void Sniper_State_update(bool value)
    {
        StartCoroutine(_Toggle_option(value));
    }
   
    public void Fire()//开火
    {
        //开火前需要判断3个条: 满足射速 不在换子弹状态 枪内有子弹
        if (Time.time > prevFire + fireCD && Time.time> reload_complete && bullet_count > 0)
        {
            Camera_Management.camera_Management.calibration = true;
            Camera_Management.camera_Management.Recoil();

            Trajectory.trajectory.Fire();


            StartCoroutine(_Recoil());

            if (bullet_count > 1)//如果开枪后枪里还有子弹就需要播放拉枪栓的声音
            {
                AudioSource.PlayClipAtPoint(aud_Fire, Camera.main.transform.position);
            }
            else if(bullet_count == 1)
            {
                AudioSource.PlayClipAtPoint(aud_Fire_Last, Camera.main.transform.position);
            }
            bullet_count--;
            Bullet_count_UI.text = bullet_count.ToString();
            prevFire = Time.time;
            if (bullet_count==0)
            {
                Invoke("Reload", fire_end);//当子弹数为0时，开火状态结束后就自动换子弹
            }
        }
        else
        {
            return;
        }
    }
    
    public void Reload()//换子弹
    {
        if (bullet_count > 0)
        {
            if (Time.time > prevFire + fireCD && bullet_count != MAX_Bullecount)
            {
                bullet_count = MAX_Bullecount;

                reload_complete = Time.time + reload_Duration;
                Invoke("Reload_Complete", reload_Duration);
                AudioSource.PlayClipAtPoint(aud_Reload, Camera.main.transform.position);
            }
        }
        else if (bullet_count==0)//由于打完最后1发子弹就自动换弹了，因此不用考虑最后1发子弹开火状态时间不同的问题
        {
            bullet_count = MAX_Bullecount;

            reload_complete = Time.time + reload_Duration;
            Invoke("Reload_Complete", reload_Duration);
            AudioSource.PlayClipAtPoint(aud_Reload, Camera.main.transform.position);
        }
    }
    private void Reload_Complete()//换子弹完成
    {
        Bullet_count_UI.text = bullet_count.ToString();
    }

    public void Zoom_Added()//焦距放大
    {
        //在开火状态或换弹匣状态的情况下都无法使用变焦功能
        if (Time.time < prevFire + fireCD || Time.time < reload_complete)
        {
            return;
        }
        if (prevZoom + zoomCD < Time.time)
        {
            if (zoom_Count != 2)
            {
                AudioSource.PlayClipAtPoint(aud_Zoom_Added, Camera.main.transform.position);
            }
            Camera_fov = Mathf.Clamp(Camera_fov -= 2, 4, 8);
            Camera.main.fieldOfView = Camera_fov;

            zoom_Count = Mathf.Clamp(zoom_Count += 1, 0, 2);
            zoom_Show_UI[zoom_Count].isOn = true;
            prevZoom = Time.time;
        }
    }

    public void Zoom_Decrease()//焦距减小
    {
        if (Time.time < prevFire + fireCD || Time.time < reload_complete)
        {
            return;
        }
        if (prevZoom + zoomCD < Time.time)
        {
            if (zoom_Count != 0)
            {
                AudioSource.PlayClipAtPoint(aud_Zoom_Decrease,Camera.main.transform.position);
            }
            Camera_fov = Mathf.Clamp(Camera_fov += 2, 4, 8);
            Camera.main.fieldOfView = Camera_fov;


            zoom_Count = Mathf.Clamp(zoom_Count -= 1, 0, 2);
            zoom_Show_UI[zoom_Count].isOn = true;
            prevZoom = Time.time;
        }
    }
    public void Down_breath_Button()
    {
        if (breath_ban==false)
        {
            Camera_Management.camera_Management.calibration = true;
            breath_end = false;
            StartCoroutine(_Breath());
        }
        else
        {
            return;
        }
    }
    public void UP_breath_Button()
    {
        breath_end = true;
        if (breath_ban==false)
        {
            Camera_Management.camera_Management.Offset();
        }
    }

    public void Quit()
    {
        quit_Panel.SetActive(true);
    }
    public void Quit_Confrim()
    {
        Application.Quit();
    }
    public void Quit_Cancel()
    {
        quit_Panel.SetActive(false);
    }
    public IEnumerator _Breath()//屏息状态
    {
        while(true)
        {
            if (breath_end==false&& breath_ban==false)
            {
                Breath_Time.value += 0.01f;
                if (Breath_Time.value == 1f)
                {
                    breath_ban = true;
                    Breath_ui_color.color = Color.red;
                    Camera_Management.camera_Management.Offset();
                }
            }
           
            if (breath_end==true|| breath_ban==true)
            {
                Breath_Time.value -= 0.001f;
            }
            if (Breath_Time.value == 0)
            {
                breath_ban = false;
                Breath_ui_color.color = Color.white;
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator _Recoil()//UI后坐力表现
    {
        float recoil_T= 0;
        float calibration_T= 0;
        Vector2 start_HW= sights.rectTransform.sizeDelta;

        while (calibration_T < 1f)
        {
            if (recoil_T < 1)
            {
                recoil_T += 0.2f;
                sights.rectTransform.sizeDelta = Vector2.Lerp(start_HW, new Vector2(5000, 5000), recoil_T);
            }
            else
            {
                calibration_T += 0.2f;
                sights.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(5000, 5000), start_HW, calibration_T);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator _Toggle_option(bool value)
    {
        float a = 0;
        float b = 255;
        while (b >= 0)
        {
            background.SetActive(true);
            a = a + 25;
            if (a < 225)
            {
                background.GetComponent<Image>().color = new Color(0, 0, 0, a / 255);
            }
            else
            {
                b = b - 25;
                background.GetComponent<Image>().color = new Color(0, 0, 0, b / 255);
            }
            if (a == 250)
            {
                if (value == false)
                {
                    Camera_Management.camera_Management.calibration = true;

                    sniper_State = false;

                    breath_Button.SetActive(false);
                    Fire_State_Mask.enabled = true;
                    sights.enabled = false;
                    telescope.enabled = true;
                    Fire_State_Text.text = "观测手模式";
                    Camera.main.fieldOfView = 9;
                }
                else
                {
                    Camera_Management.camera_Management.Offset();

                    sniper_State = true;

                    breath_Button.SetActive(true);
                    Fire_State_Mask.enabled = false;
                    sights.enabled = true;
                    telescope.enabled = false;
                    Fire_State_Text.text = "狙击手模式";
                    Camera.main.fieldOfView = Camera_fov;
                }
            }
            if (b < 0)
            {
                background.SetActive(false);
            }
            yield return null;
        }
    }
}
