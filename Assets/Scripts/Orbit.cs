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
        //����ź�� �÷��̾� ������ �Ÿ�
        offset = transform.position - target.position;
    }

   
    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        //offset�� update�� �ֱ�
        offset = transform.position - target.position;
    }
}
