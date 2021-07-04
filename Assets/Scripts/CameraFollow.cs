using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    void Update()
    {
        transform.position = target.position + offset; //inspector밑 타겟에서 플레이어로 설정시
                                                       //카메라가 플레이어를 따라감
    }
}
