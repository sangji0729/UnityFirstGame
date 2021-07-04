using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;


    void Start()
    {
        //수류탄과 플레이어 사이의 거리
        offset = transform.position - target.position;
    }

   
    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        //offset을 update에 넣기
        offset = transform.position - target.position;
    }
}
