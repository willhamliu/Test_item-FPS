using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trajectory : MonoBehaviour
{
    public Text wind_Speed_value;
    public Text Hit_rate_value;
    public static Trajectory trajectory;

    public AudioClip aud_Hit;//击中靶机声音


    public Canvas canvas;
    public GameObject bullet_fx;
    public GameObject hit_fx;
    public Transform hit_Point_y;//y轴弹着点(动态)
    public Transform hit_Point_x; //x轴弹着点(动态)
    Vector3 target_point=Vector3.zero;
    public float time { get; private set; }//子弹飞行时间
    float wind_Speed;//风速
    float distance=1000;//距离
    const float bullet_speed=700;//子弹速度
    const int g = 10;

    float offset_x;

    float Fire_number = 0f;
    float fraction = 0f;
    float Hit_rate;//命中率

    GameObject Instantiate_item;

    private void Awake()
    {
        if (trajectory==null)
        {
            trajectory = this;
        }

        float range_X = Random.Range(0, 3);
        switch (range_X)
        {
            case 0:
                wind_Speed = -1;
                break;
            case 1:
                wind_Speed = 1;
                break;
            default:
                wind_Speed = 0;
                break;
        }
        wind_Speed_value.text = wind_Speed.ToString();

        if (wind_Speed < 0)
        {
            hit_Point_x.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f,0f,0f);
        }
        else if(wind_Speed >0)
        {
            hit_Point_x.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else if(wind_Speed == 0)
        {
            hit_Point_x.gameObject.SetActive(false);
        }

        time = distance / bullet_speed;
        offset_x = (g * time * time / 2) * (100 / distance);//下坠
    }
  
    private void Update()
    {
        if (UI_Management.ui_Management.sniper_State == false)
        {
            
            hit_Point_x.gameObject.SetActive(false);
            hit_Point_y.gameObject.SetActive(false);
        }
        else
        {
            if (wind_Speed != 0)
            {
                hit_Point_x.gameObject.SetActive(true);
            }
            hit_Point_y.gameObject.SetActive(true);

            hit_Point_y.position = new Vector3(hit_Point_y.position.x, Camera.main.transform.position.y + (-offset_x), hit_Point_y.position.z);
            hit_Point_x.position = new Vector3(Camera.main.transform.position.x + (wind_Speed / 2f), hit_Point_x.position.y, hit_Point_x.position.z);
        }
    }

    public void Fire()
    {
        Instantiate_item=Instantiate(bullet_fx, Camera.main.transform.position, Camera.main.transform.rotation);
        target_point = new Vector3(hit_Point_x.position.x, hit_Point_y.position.y, canvas.planeDistance);
        Invoke("Target_Detection", time);
        Debug.DrawRay(Camera.main.transform.position, target_point, Color.red);

    }
    public void Target_Detection()
    {
        Fire_number++;
        Ray ray = new Ray(transform.position, target_point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag== "TargetDrone")
            {

                AudioSource.PlayClipAtPoint(aud_Hit, hit.point);
                Instantiate(hit_fx, hit.point, hit_fx.transform.rotation);
                fraction++;
            }
        }
        Hit_rate =(int)((fraction / Fire_number)*100f);
        Hit_rate_value.text = Hit_rate.ToString()+"%";
        Destroy(Instantiate_item);
    }
}
