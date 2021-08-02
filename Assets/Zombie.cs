using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    private int hp = 100;
    Animator animator;


    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        target = FindObjectOfType<Player>().transform;

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            yield return new WaitForSeconds(Random.Range(0, 2));
        }
    }

    internal void TakeHit(int damage)
    {
        hp -= damage;
        animator.Play(Random.Range(0,2) == 0 ? "TakeHit1" : "TakeHit2", 0,0);
        //0번 레이어의 0번 프레임부터 다시 시작한다는 의미.
        //애니메이션이 플레이 중이더라도 처음으로 돌아가서 다시 시작하게 된다. 

        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(Die), 1) ; //nameof를 사용하면 함수 이름 바꾸는 리팩토링할 때 유용
            //원래 인보크를 사용할 때는 Invoke("함수이름",딜레이시간);으로 써야한다. 
        }
    }

    void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 1); //1초 뒤에 게임오브젝트(자기자신)을 파괴)
    }

}
