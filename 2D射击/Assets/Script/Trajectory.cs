﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trajectory : MonoBehaviour
{
    public static Trajectory trajectory;

    public Text wind_Speed_value;
    public Text hit_rate_value;
    public Text distance_value;
    public AudioClip aud_Hit;//击中靶机声音


    public Canvas canvas;
    public GameObject bullet;
    public GameObject hit_fx;
    public Transform hit_Point_y;//y轴弹着点(动态)
    public Transform hit_Point_x; //x轴弹着点(动态)
    public float time { get; private set; }//子弹飞行时间
    public float wind_Speed { get; private set; }//风速

    public float bullet_speed;//子弹速度
    public float Angle;//发射角度

    public float distance { get; private set; } = 800;//距离
    const int Gravity = 10;
    private Vector3 target_point = Vector3.zero;

    float offset_x;
    float offset_y;

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

        distance_value.text = distance.ToString()+"m";
        canvas.planeDistance = distance / 10;


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

        float updatetime = 0f;
        float drop = 0;
        float windage_yaw = 0;
        float rising = 0;
        while (updatetime < time)
        {
            drop += ((Gravity/10)* updatetime) *0.02f;
            windage_yaw += (wind_Speed/2 * updatetime) *0.02f;
            updatetime += 0.02f;
        }
        rising = distance/10 * Mathf.Tan(Mathf.Abs(Angle) * Mathf.Deg2Rad);
        offset_y = rising - drop;//下坠
        offset_x = windage_yaw;
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

            hit_Point_y.position = new Vector3(hit_Point_y.position.x, Camera.main.transform.position.y + (offset_y), hit_Point_y.position.z);
            hit_Point_x.position = new Vector3(Camera.main.transform.position.x + (offset_x), hit_Point_x.position.y, hit_Point_x.position.z);
        }
    }

    public void Fire()
    {
        Instantiate_item = Instantiate(bullet,UI_Management.ui_Management.Fire_point, Camera.main.transform.rotation);
        target_point = new Vector3(hit_Point_x.position.x, hit_Point_y.position.y, canvas.planeDistance);
        Invoke("Target_Detection", time);
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
        hit_rate_value.text = Hit_rate.ToString()+"%";
    }
}
