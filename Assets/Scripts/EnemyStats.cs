using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody enemyRB;
    BoxCollider boxCollider;
    Material mat;

    void Awake()
    {
        enemyRB = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material; //Material? getComponent ??? <MeshRenderer>().material ? ?????
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));


        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else if(curHealth <= 0)
        {
            mat.color = Color.gray;
            gameObject.layer = 14;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            enemyRB.AddForce(reactVec * 1, ForceMode.Impulse);

            Destroy(gameObject, 4);
        }
    }

}
