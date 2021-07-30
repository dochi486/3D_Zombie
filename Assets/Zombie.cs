using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    private int hp;
    Animator animator;


    // Start is called before the first frame update
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
        //animator.Play("TakeHit");
        
        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(Die), 1) ; //nameof를 사용하면 함수 이름 바꾸는 리팩토링할 때 유용
        }
    }

    void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 1); //1초 뒤에 게임오브젝트(자기자신)을 파괴)
    }

}
