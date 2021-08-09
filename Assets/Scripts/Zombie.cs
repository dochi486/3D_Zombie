using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Zombie : Character
{
    public Transform target;
    NavMeshAgent agent;
    float originalSpeed;
    public LayerMask enemyLayer;
    public int power = 20;

    IEnumerator Start()
    {

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        target = FindObjectOfType<Player>().transform;
        originalSpeed = agent.speed;
        attackCollider = transform.Find("AttackRange").GetComponent<SphereCollider>(); //이름으로 찾아서 올바른 콜라이더 어택레인지를 찾도록했다.
        //attackCollider = GetComponentInChildren<SphereCollider>(); //부모 오브젝트에 있는 걸 먼저 찾아서
        //자기 자신한테 붙어있는 걸 먼저 찾기 때문에 오류가 났떤 것


        CurrentFsm = ChaseFSM;

        while (true) // 상태를 무한히 반복해서 실행하는 부분.
        {
            var previousFSM = CurrentFsm;

            fsmHandle = StartCoroutine(CurrentFsm());

            // FSM 안에서 에러 발생시 무한 루프 도는 것을 방지 하기 위해서 추가함.
            if (fsmHandle == null && previousFSM == CurrentFsm)
                yield return null;

            while (fsmHandle != null)
                yield return null;
        }

        //while (hp > 0)
        //{
        //    if (target)
        //        agent.destination = target.position;
        //    yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        //}
    }

    internal void TakeHit(int damage, Vector3 toMoveDirection, float pushBackDistance = 0.1f)
    {
        base.TakeHit(damage);
        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            animator.SetBool("Die", true); //bool로 애니메이터 트리거 만들어줌
        }
        //총알을 맞았을 때 뒤로 밀려난다. 
        PushBackMove(toMoveDirection, pushBackDistance);

        CurrentFsm = TakeHitFSM;
    }

    IEnumerator TakeHitFSM()
    {
        animator.Play(Random.Range(0, 2) == 0 ? "TakeHit1" : "TakeHit2", 0, 0);
        //0번 레이어의 0번 프레임부터 다시 시작한다는 의미.
        //애니메이션이 플레이 중이더라도 처음으로 돌아가서 다시 시작하게 된다. 

        //그리고 이동 스피드가 잠시동안 0이 된다.
        agent.speed = 0;
        //CancelInvoke(nameof(SetOriginalSpeed)); //이전에 실행하고 있던 invoke를 꺼줘서 좀비의 내비에이전트 속도가 잠시 0이 되도록한다. 
        ////이전에 실행한 SetTakeHitSpeed를 취소해줘야 스피드0으로 설정한 것이 제대로 적용된다?
        //Invoke(nameof(SetOriginalSpeed), TakeHitStopSpeedTime);
        ////좀비 내비메쉬에이전트의 스피드를 원래 속도로 돌려준다.  
        //코루틴 안에 있는 인보크는 가독성만 낮아지므로 코루틴으로 바꿔주는 게 좋다. 

        yield return new WaitForSeconds(TakeHitStopSpeedTime); //인보크 대신에 쉬는 코드를 먼저 넣고


        if (hp <= 0)
        {
            //yield return new WaitForSeconds(0.11f); //피격 모션 끝나는 걸 기다려준다.
            Die();
            yield break;
            //Invoke(nameof("Die"),1);//nameof를 사용하면 함수 이름 바꾸는 리팩토링할 때 유용
            //원래 인보크를 사용할 때는 Invoke("함수이름",딜레이시간);으로 써야한다. 
        }
        else
        {
            SetOriginalSpeed(); //에이전트의 속도를 원래 속도로 돌려주는 함수 실행.
        }
        CurrentFsm = ChaseFSM; //피격 당한 후에 바로 FSM으로 추격모드 설정
    }

    public float moveBackDistance = 1f;
    public float moveBackNoise = 0.1f;
    public float moveBackDuration = 0.5f; //밀리는 시간!
    public Ease moveBackEase = Ease.OutQuart;
    private void PushBackMove(Vector3 toMoveDirection, float _moveBackDistance)
    {
        toMoveDirection.x += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.z += Random.Range(-moveBackNoise, moveBackNoise);
        toMoveDirection.y = 0;
        toMoveDirection.Normalize();

        //transform.Translate(toMoveDirection * moveBackDistance, Space.World);
        //Tranlate하면 1프레임만에 바로 이동하기 때문에 제대로 밀리는 효과 주려면 다르게 해야한다. \

        transform.DOMove(transform.position + toMoveDirection * _moveBackDistance * moveBackDistance, moveBackDuration).SetEase(moveBackEase);

    }
    public float TakeHitStopSpeedTime = 0.1f;
    private void SetOriginalSpeed()
    {
        agent.speed = originalSpeed;
    }

    public float destroyDelayTime = 2f; //2초동안 기다렸다가 파괴된다. 
    public Material dieMaterial;
    public float dieMaterialDuration = 2f; //2초동안 매테리얼 변경된다. 매테리얼이 변경되는 시간!\

    [ContextMenu("DieTest")]
    void TestFn()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var item in renderers)
        {
            item.sharedMaterial = dieMaterial; //왜 sharedMaterial로 쓰지?
        }
        //dieMaterial.SetFloat("_Progress", 0.5f); //Dissolved Edge 애셋의 쉐이더 변수 중 Progress를 설정해주는 부분
        DOTween.To(() => 1f, (x) => dieMaterial.SetFloat("_Progress", x), 0, dieMaterialDuration).SetDelay(destroyDelayTime).OnComplete(() => Destroy(gameObject)); ;
    }

    void Die()
    {
        StageManager.Instance.AddScore(rewardScore);

        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var item in renderers)
        {
            item.sharedMaterial = dieMaterial; //왜 sharedMaterial로 쓰지?
        }

        dieMaterial.SetFloat("_Progress", 1);//Dissolved Edge 애셋의 쉐이더 변수 중 Progress를 설정해주는 부분
        //매테리얼 교체되는 동안 기다렸다가 파괴
        DOTween.To(() => 1f, (x) => dieMaterial.SetFloat("_Progress", x), 0.14f, dieMaterialDuration).SetDelay(destroyDelayTime).OnComplete(() => Destroy(gameObject)); ;

        //animator.Play("Die"); //bool로 조건 만들어줬으니까 주석 처리
        //Destroy(gameObject, destroyDelayTime); //1초 뒤에 게임오브젝트(자기자신)을 파괴)
    }


    // 추격 할대 플레이어한테 공격 가능한 거리면 공격.
    // 공격후 추격
    // 추격 공격

    Coroutine fsmHandle;
    protected Func<IEnumerator> CurrentFsm
    {
        get { return m_currentFsm; }
        set
        {
            if (fsmHandle != null)
                StopCoroutine(fsmHandle); //이전 코루틴이 있으면 항상 멈추도록 하는 부분

            m_currentFsm = value;
            fsmHandle = null;
        }
    }
    Func<IEnumerator> m_currentFsm;

    public float attackDistance = 1.16f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        //좀비가 공격하는 범위에 gizmo를 그린다. 
    }

    IEnumerator ChaseFSM()
    {
        if (target)
            agent.destination = target.position;

        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        SetFSM_SelectAttackTargetOrAttacOrChase();
    }

    private void SetFSM_SelectAttackTargetOrAttacOrChase()
    {
        if (IsTargetAttackable()) //타겟이 공격 가능한 상태인지 확인
        {
            //타겟이 공격 범위 안에 들어왔는지 판단
            if (TargetIsInAttackDistance())
                CurrentFsm = AttackFSM;
            else
                CurrentFsm = ChaseFSM;
        }
        else
        {
            //공격 가능한 타겟이 아니라면 타겟을 찾아야한다. 

            //타겟이 그래도 없다면 배회하거나

            //제자리에 서있는다.
            print("배회하는 중");
        }
    }

    private bool IsTargetAttackable()
    {
        if (target.GetComponent<Player>().stateType == Player.StateType.Die)
            return false; //플레이어가 죽은 상태라면 공격 불가능한 상태

        return true;
    }

    public float attackTime = 0.4f; //실제로 때리는 모션을 하기 전까지의 시간
    public float attackAnimationTime = 0.8f; //공격 애니메이션 클립의 총 길이
    public SphereCollider attackCollider;

    private IEnumerator AttackFSM()
    {
        //타겟을 바라보게 해서 때리게 하면 헛방이 없을 것 -> 플레이어가 구르면 못때린다
        var lookAtPos = target.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);

        //공격 애니메이션 플레이
        animator.SetTrigger("Attack");

        //공격 중에는 이동 스피드가 0으로 바뀌게 한다.
        agent.speed = 0;

        //공격 타이밍까지 대기(공격 애니메이션의 0.6프레임 부터 실제로 때리기 때문에)
        yield return new WaitForSeconds(attackTime);

        //특정 시간 지나면 충돌메쉬 사용해서 충돌 감지? <- 왜 하는 건지 모르겠다
        //공격 범위(AttackDistance)와 콜라이더(AttackRange)를 모두 사용하는 이유?
        //범위 안에 있지만 공격 콜라이더에 닿고있지 않은 상황이면 헛방을 때리도록?
        Collider[] enemyColliders = Physics.OverlapSphere(attackCollider.transform.position, attackCollider.radius, enemyLayer);
        foreach (var item in enemyColliders)
        {
            item.GetComponent<Player>().TakeHit(power);
        }
        //공격 애니메이션이 끝날 때까지 대기한다. 
        yield return new WaitForSeconds(attackAnimationTime - attackTime);
        //위에서 0.4초를 기다렸으므로 남은 기다릴 시간은 총 클립 길이에서 기다린 시간을 빼준다. 

        //이동 스피드 복구
        SetOriginalSpeed();

        //FSM 지정
        SetFSM_SelectAttackTargetOrAttacOrChase();
        //Chase에서는 target이 있으면 잠시 랜덤한 시간만큼 기다렸다가
        //공격 범위 안에 타겟이 있으면 공격을 하든, 추격을 하든 판단하는 로직이 있기 때문에
        //ChaseFSM으로 다음 로직 판단하게 할 수 있다. 
    }

    private bool TargetIsInAttackDistance()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < attackDistance; //범위 안에 타겟이 있다는 의미
    }

    public int rewardScore = 100;

}
