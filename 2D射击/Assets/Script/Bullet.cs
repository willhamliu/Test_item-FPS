using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float g = -10;//重力加速度
    Vector3 speed;//初速度向量
    Vector3 Gravity;//重力向量
    Vector3 hit_point;
    float dTime = 0;
    float Bullet_T = 0;
    float time;


    void Start()
    {
        hit_point = new Vector3(Trajectory.trajectory.hit_Point_x.position.x, Trajectory.trajectory.hit_Point_y.position.y,Trajectory.trajectory.canvas.planeDistance);
        time = Trajectory.trajectory.time;
        //speed = new Vector3((hit_point .x- transform.position.x) / time, (hit_point.y - transform.position.y) / time - 0.5f * g * time, (Trajectory.trajectory.canvas.planeDistance) / time);
        //Gravity = Vector3.zero;//重力初始速度为0
    }
  
    void FixedUpdate()
    {
        Bullet_T += (time / 50);
        transform.position = Vector3.Lerp( Camera_Management.camera_Management.fire_point, hit_point, Bullet_T);
        //Gravity.y = g * (dTime += Time.fixedDeltaTime);
        ////模拟位移

        //transform.Translate(speed * Time.fixedDeltaTime);
        //transform.Translate(Gravity * Time.fixedDeltaTime);
    }
}
