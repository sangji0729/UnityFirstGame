using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private float HP;
    private float speed = 0.1f;
    private float move;
    private Rigidbody enemyRB;
    private GameObject player;
    // Start is called before the first frame update
   

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        HP = 100;
    }

    // Update is called once per frame
    void Update()
    {
        enemyFollow();
    }

    void enemyFollow()
    {
        //���� �÷��̾�� �ٰ���
        enemyRB.AddForce((player.transform.position - transform.position).normalized * speed, ForceMode.Impulse);
    }

    void look()
    {
        //�����̴� �������� ĳ���Ͱ� �ٶ󺸱�
        transform.LookAt(transform.position);
    }
}
