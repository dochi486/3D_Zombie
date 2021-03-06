using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public partial class Player : Character
{
    Coroutine setLookAtTargetCoHandle;
    private void Start()
    {
        setLookAtTargetCoHandle = StartCoroutine(SetLookAtTargetCo());
    }

    IEnumerator SetLookAtTargetCo()
    {
        MultiAimConstraint multiAimConstraint = GetComponentInChildren<MultiAimConstraint>();
        RigBuilder rigBuilder = GetComponentInChildren<RigBuilder>();

        while (stateType != StateType.Die)
        {
            //float yPosition = 0;
            List<Zombie> allZombies = Zombie.Zombies;
            //FindObjectsOfType은 안 쓰는 게 좋다. 나중에 바꿀 것
            Transform lastTarget = null;

            if (allZombies.Count > 0)
            {
                var nearestZombie = allZombies.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First();
                if (lastTarget != nearestZombie.transform) //타겟이 있을 떄 작동
                {
                    //yPosition = 0.13f;
                    lastTarget = nearestZombie.transform;
                    var array = multiAimConstraint.data.sourceObjects;
                    array.Clear();
                    array.Add(new WeightedTransform(nearestZombie.transform, 1));
                    multiAimConstraint.data.sourceObjects = array;
                    rigBuilder.Build();
                }
            }
            //var pos = animator.transform.parent.position;
            //pos.y = yPosition;
            //animator.transform.parent.localPosition = pos;
            yield return new WaitForSeconds(1);
        }
        //HeadRig가 주기적으로 실행되게 한다. 
        //Awake는 오브젝트가 꺼져있을 때 실행이 안되므로(씬에 등장할 때 실행된다) Update 전에 무조건 실행되는 Start에서 실행
    }
    public enum StateType
    {
        Idle,
        Move,
        //Attack, //이동하면서 attack이 되기 때문에 하나의 상태가 아니라서 사용 안 한다.
        TakeHit,
        Roll,
        Die,
        Reload,
    }

    public bool isFiring = false; //총 쏘는 중인지 확인하는 변수
    public WeaponInfo mainWeapon;
    public WeaponInfo subWeapon;

    public WeaponInfo currentWeapon;

    internal void RetargetLookAt()
    {
        MultiAimConstraint multiAimConstraint = GetComponentInChildren<MultiAimConstraint>();
        multiAimConstraint.data.sourceObjects = new WeightedTransformArray(); //왜 Clear해주면 안되고 이렇게 해야할까?
        GetComponentInChildren<RigBuilder>().Build();

        StopCoroutine(setLookAtTargetCoHandle);
        setLookAtTargetCoHandle = StartCoroutine(SetLookAtTargetCo());
    }

    public Transform rightWeaponPosition; //무기의 부모 오브젝트(오른팔)

    new protected void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();

        //mainWeapon = new WeaponInfo(); //플레이할 때마다 총알 갯수 변경되는 거 수정하기 위한 코드였지만 이건 정석이 아닌 것 같아서 일단 보류

        InitWeapon(mainWeapon);
        InitWeapon(subWeapon);
        //if (subWeapon)
        //    subWeapon.Init();


        ChangeWeapon(mainWeapon);

        SetCinemachineCamera(); //모든 시네머신 버추얼 카메라에서 Player를 타겟으로 지정하게 Awake에서 실행
        HealthUI.Instance.SetGauge(hp, maxHp);


        AmmoUI.Instance.SetBulletCount(BulletCountInClip, MaxBulletCountInClip, AllBulletCount + BulletCountInClip, MaxBulletCount); ;

    }

    private void InitWeapon(WeaponInfo weaponInfo)
    {
        if (weaponInfo)
        {

            weaponInfo = Instantiate(mainWeapon, transform);
            //웨폰인포에 바로 접근해서 최대 총알 수가 플레이할 때마다 줄어들던 현상 수정하기 위해 추가한 부분
            weaponInfo.Init();
            weaponInfo.gameObject.SetActive(false);

        }
    }

    GameObject currentWeaponGo;
    private void ChangeWeapon(WeaponInfo _weaponInfo)
    {
        Destroy(currentWeaponGo);
        currentWeapon = _weaponInfo;

        animator.runtimeAnimatorController = currentWeapon.overrideAnimator;

        var weaponInfo = Instantiate(currentWeapon, rightWeaponPosition);
        currentWeaponGo = weaponInfo.gameObject;
        weaponInfo.transform.localScale = currentWeapon.gameObject.transform.localScale;
        weaponInfo.transform.localPosition = currentWeapon.gameObject.transform.localPosition;
        weaponInfo.transform.localRotation = currentWeapon.gameObject.transform.localRotation;
        currentWeapon = weaponInfo;

        if (currentWeapon.attackCollider)
            currentWeapon.attackCollider.enabled = false;

        //bulletPosition = weaponInfo.bulletPosition;

        if (weaponInfo.bulletLight != null)
            bulletLight = weaponInfo.bulletLight.gameObject;
        shootDelay = currentWeapon.delay;
    }

    [ContextMenu("SetCinemachineCamera")]
    private void SetCinemachineCamera()
    {
        var vcs = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var item in vcs)
        {
            item.Follow = transform;
            item.LookAt = transform;
        }
    }
    public float speed = 3f;
    public float speedWhileShooting = 3;

    void Update()
    {
        if (Time.deltaTime == 0)
            return; //왜 있는지 모르겠다

        if (stateType == StateType.Die)
            return;

        if (stateType != StateType.Roll)
        {
            LookAtMouse();
            Move();
            Fire();
            Roll(); //Roll상태가 아닐 때만 Roll할 수 있도록 
            ReloadBullet();
            if (Input.GetKeyDown(KeyCode.Tab))
                ToogleChangeWeapon();
        }

    }
    bool toggleWeapon = false;
    private void ToogleChangeWeapon()
    {
        ChangeWeapon(toggleWeapon == true ? mainWeapon : subWeapon);
        toggleWeapon = !toggleWeapon; //!붙이는 거 뭔지 
    }

    private void ReloadBullet()
    {
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(ReloadCo());
    }

    private IEnumerator ReloadCo()
    {
        stateType = StateType.Reload;
        animator.SetTrigger("Reload");
        int reloadCount = Math.Min(AllBulletCount, MaxBulletCountInClip);

        AmmoUI.Instance.StartReload(reloadCount, MaxBulletCountInClip, AllBulletCount, MaxBulletCount, ReloadTime);
        yield return new WaitForSeconds(ReloadTime);
        stateType = StateType.Idle;

        BulletCountInClip = reloadCount;
        AllBulletCount -= reloadCount;
    }

    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RollCo());
        }
    }

    public AnimationCurve rollingSpeedAC;

    public float rollingSpeedManualMultiply = 1; //인스펙터에서 수정하는 값


    public StateType stateType = StateType.Idle;
    private IEnumerator RollCo()
    {
        EndFiring(); //총 쏘는 게 끝났을 때 실행되는 코드

        stateType = StateType.Roll;
        //구르는 애니메이션 재생
        animator.SetTrigger("Roll");

        //구르는 동안 플레이어의 스피드를 빠르게 바꾼다. 
        float startTime = Time.time;
        float endTime = startTime + rollingSpeedAC[rollingSpeedAC.length - 1].time;
        while (endTime > Time.time)
        {
            float time = Time.time - startTime;
            float rollingSpeedMultiply = rollingSpeedAC.Evaluate(time) * rollingSpeedManualMultiply;

            transform.Translate(speed * transform.forward * rollingSpeedMultiply * Time.deltaTime, Space.World);

            yield return null;
        }
        stateType = StateType.Idle;

        //구르는 방향은 처음 바라보고 있던 방향으로 고정한다.

        //구르는 동안은 총알 발사 안되도록, 다른 방향으로 이동도 불가, 마우스 포인터 방향으로 바라보지도 않게


    }

    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    void LookAtMouse()
    {
        ////roll상태에선 forward를 바꾸지 않도록
        //if (stateType == StateType.Roll)
        //    return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //스크린 포인트를 ray로 변경한다. (마우스 포지션을 ray로 변경하겠다)

        if (plane.Raycast(ray, out float enter)) //카메라에서부터 레이 체크를 위해 만들었던 무한한 평면 plane까지 레이를 쏘고 그 결과값 enter를 out파라미터로 반환
        {
            //충돌 지점이 enter에 담기고 그 enter를 GetPoint해서 Vector3의 hitpoint로 사용
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = 0;
            dir.Normalize();
            transform.forward = dir;
        }
    }
    private void Move()
    {
        ////Roll중일 때는 move할 필요 없으니 나가도록
        //if (stateType == StateType.Roll)
        //    return;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move.z += 1;
        if (Input.GetKey(KeyCode.S))
            move.z -= 1;
        if (Input.GetKey(KeyCode.A))
            move.x -= 1;
        if (Input.GetKey(KeyCode.D))
            move.x += 1;
        if (move != Vector3.zero)
        {
            //카메라 방향과 이동하는 방향 비슷하게 보이도록? 카메라를 기준으로 이동시킨다
            Vector3 relativeMove; //상대적인 이동값
            relativeMove = Camera.main.transform.forward * move.z;
            //카메라의 포워드 앞 방향은 (0,0,1) * move.z
            //카메라가 회전하면 월드축과 상대적으로 카메라의 로컬좌표 기준으로 포워드, right등을 정한다.
            //로컬좌표와 월드좌표 상대적으로..
            //앞뒤이동 상대값

            relativeMove += Camera.main.transform.right * move.x;
            //카메라의 right는 x축의 양수값 (1,0,0) 
            //좌우이동 상대값
            relativeMove.y = 0;
            move = relativeMove;

            move.Normalize();


            float _speed = isFiring ? speedWhileShooting : speed; //총을 쏘는 중이면 총 쏘는 중의 스피드가 적용되고, 아니면 일반 스피드 사용 되도록

            transform.Translate(_speed * move * Time.deltaTime, Space.World);
            ////transform.forward = move; //이동하는 방향 바라보게 한다.
            ///

            float forwardDegree = transform.forward.VectorToDegree();//앵글은 숫자이기 때문에 0~360에 해당하는 값이 들어간다. 
            //트랜스폼.포워드는 방향이기 때문에 숫자가 아니다. 
            float moveAngle = move.VectorToDegree(); //방향 -> 앵글
            float dirRadian = (moveAngle - forwardDegree + 90) * Mathf.PI / 180; //방향 앵글, 애니메이터로 보내야할 각도
            Vector3 dir;
            dir.x = Mathf.Cos(dirRadian);
            dir.z = Mathf.Sin(dirRadian);
            //Radian은 호와 반지름의 비율이다???


            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.z);

            //if (Mathf.RoundToInt(transform.forward.x) == 1 || Mathf.RoundToInt(transform.forward.x) == -1)
            //{
            //    animator.SetFloat("DirX", transform.forward.z * move.z);
            //    animator.SetFloat("DirY", transform.forward.x * move.x);
            //}
            //else
            //{
            //    animator.SetFloat("DirX", transform.forward.x * move.x);
            //    animator.SetFloat("DirY", transform.forward.z * move.z);
            //}

        }
        //애니메이터의 파라미터 Speed를 실제 이동하는 속도 move.sqrMagnitude로 설정한다.
        //animator.SetFloat("DirX", transform.forward.x);
        //animator.SetFloat("DirY", transform.forward.z);
        animator.SetFloat("Speed", move.sqrMagnitude);
    }

    new internal void TakeHit(int damage)
    {
        base.TakeHit(damage);
        //print("피격");

        HealthUI.Instance.SetGauge(hp, maxHp);

        //피격 애니메이션 재생
        animator.SetTrigger("TakeHit");

        if (hp <= 0)
        {
            StartCoroutine(DieCo());
        }

    }

    public float dieDelayTime = 0.3f;

    private IEnumerator DieCo()
    {
        stateType = StateType.Die;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(dieDelayTime);
        animator.SetTrigger("Die");
    }

    public void OnZombieEnter(Collider other)
    {
        var zombie = other.GetComponent<Zombie>();
        zombie.TakeHit(currentWeapon.damage, currentWeapon.gameObject.transform.forward, currentWeapon.pushBackDistance);
    }
}