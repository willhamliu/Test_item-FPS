using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float Gravity = -10;//这个代表重力加速度

    private Vector3 MoveSpeed;//初速度向量

    private Vector3 GritySpeed = Vector3.zero;//重力的速度向量，t时为0
    private float dTime;//已经过去的时间
    private Vector3 currentAngle;

    void Start()
    {
        //通过一个公式计算出初速度向量
        //角度*力度
        MoveSpeed = Quaternion.Euler(new Vector3(Trajectory.trajectory.Angle, 0, 0)) * Vector3.forward * Trajectory.trajectory.bullet_speed/10;
        currentAngle = Vector3.zero;
    }
    void Update()
    {
        if (transform.position.z >= Trajectory.trajectory.distance/10)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        //v = at ;
        GritySpeed.y = (Gravity/10) * (dTime += Time.fixedDeltaTime);
        GritySpeed.x= Trajectory.trajectory.wind_Speed/2 * (dTime);

        //位移模拟轨迹
        transform.position += (MoveSpeed + GritySpeed) * Time.fixedDeltaTime;
        currentAngle.z = Mathf.Atan((MoveSpeed.y + GritySpeed.y) / MoveSpeed.x) * Mathf.Rad2Deg;
        transform.eulerAngles = currentAngle;
    }
}
