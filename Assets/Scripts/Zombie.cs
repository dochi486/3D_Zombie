using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;
    private int hp = 100;
    Animator animator;
    float originalSpeed;

    public float bloodEffectYposition = 1.3f; //피 이펙트의 y포지션
    public GameObject bloodParticle;
    private void CreateBloodEffect()
    {
        var pos = transform.position;
        pos.y = bloodEffectYposition;
        Instantiate(bloodParticle, pos, Quaternion.identity);
    }
    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        originalSpeed = agent.speed;

        target = FindObjectOfType<Player>().transform;

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    internal void TakeHit(int damage, Vector3 toMoveDirection)
    {
        hp -= damage;
        animator.Play(Random.Range(0,2) == 0 ? "TakeHit1" : "TakeHit2", 0,0);
        //0번 레이어의 0번 프레임부터 다시 시작한다는 의미.
        //애니메이션이 플레이 중이더라도 처음으로 돌아가서 다시 시작하게 된다. 

        CreateBloodEffect();

        //총알을 맞았을 때 뒤로 밀려난다. 
        PushBackMove(toMoveDirection);


        //그리고 이동 스피드가 잠시동안 0이 된다.
        agent.speed = 0;
        CancelInvoke(nameof(SetTakeHitSpeed));
        Invoke(nameof(SetTakeHitSpeed), TakeHitStopSpeedTime);





        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(Die), 1) ; //nameof를 사용하면 함수 이름 바꾸는 리팩토링할 때 유용
            //원래 인보크를 사용할 때는 Invoke("함수이름",딜레이시간);으로 써야한다. 
        }
    }


    public float moveBackDistance = 0.1f;
    public float moveBackNoise = 0.1f;
    private void PushBackMove(Vector3 toMoveDirection)
    {
        toMoveDirection.x += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.z += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.y = 0;
        toMoveDirection.Normalize();
        transform.Translate(toMoveDirection * moveBackDistance, Space.World);
    }
    public float TakeHitStopSpeedTime = 0.1f;
    private void SetTakeHitSpeed()
    {
        agent.speed = originalSpeed;
    }

    void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 1); //1초 뒤에 게임오브젝트(자기자신)을 파괴)
    }

}
