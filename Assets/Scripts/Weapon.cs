using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;//데미지
    public float rate;//공격속도
    public int maxAmmo;//현재 무기의 최대 장탄수
    public int curAmmo;//현재 무기의 탄약
    public BoxCollider meleeArea;//근접공격 공격범위
    public TrailRenderer trailEffect;//근접공격 공격효과
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing"); //코루틴정지 함수
            StartCoroutine("Swing");//코루틴을 호출할땐 StartCoroutine 선언후 괄호안에 코루틴함수를 문자열로 작성
        }
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //근접무기 코루틴
    {
        //yield 키워드를 여러개 사용하여 시간차 로직 생성 가능

        //1
        yield return new WaitForSeconds(0.1f);//0.1초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        yield break;
    }

    IEnumerator Shot() //원거리 무기 코루틴
    {
        //1. 총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);//총알 오브젝트
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;//총알속도

        yield return null;

        //2. 탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);//인스턴스화된 탄피에 랜덤한 힘을 더해 탄피가 튀는 방향 랜덤화
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    //use() 메인루틴 -> Swing() 서브루틴 -> use() 메인루틴
    //use() 메인루틴 + Swing() 코루틴 (co-op)

}
