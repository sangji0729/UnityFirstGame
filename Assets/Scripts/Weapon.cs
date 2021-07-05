using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;//데미지
    public float rate;//공격속도
    public BoxCollider meleeArea;//근접공격 공격범위
    public TrailRenderer trailEffect;//근접공격 공격효과

    public void Use()
    {
        if(type == Type.Melee) { }
        StopCoroutine("Swing"); //코루틴정지 함수
        StartCoroutine("Swing");//코루틴을 호출할땐 StartCoroutine 선언후 괄호안에 코루틴함수를 문자열로 작성
    }

    IEnumerator Swing()
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

    //use() 메인루틴 -> Swing() 서브루틴 -> use() 메인루틴
    //use() 메인루틴 + Swing() 코루틴 (co-op)

}
