using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    public float fireCD;//射击间隔
    public float zoomCD;//变焦间隔
    public float fire_end;//最后一枚子弹的射击间隔

    public float reload_Duration;//换子弹时间
    public int MAX_Bullecount;//最大容弹量

    public int zoom_Count { get; set; } = 0;//变焦
    public int bullet_count { get; set; }//当前剩余子弹数量
    public float prevFire { get; set; }//上一次开火时间
    public float prevZoom { get; set; }//上一次变焦时间
    public float reload_complete { get; set; }//预计换子弹结束的时间
    public bool breath_end { get; set; }//屏息取消情况
    public bool breath_ban { get; set; }//屏息禁止情况
    public bool sniper_State { get; set; }=true;

    public AudioClip aud_Fire;//射击声音+拉枪栓声音
    public AudioClip aud_Reload;//换弹匣声音
    public AudioClip aud_Fire_Last;//子弹数为1时开枪后就不用拉枪栓了  
    public AudioClip aud_Zoom_Added;//换弹匣声音
    public AudioClip aud_Zoom_Decrease;//子弹数为1时开枪后就不用拉枪栓了
}
