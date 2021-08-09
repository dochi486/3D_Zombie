using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public partial class Player : Character
{
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
    public Transform rightWeaponPosition; //무기의 부모 오브젝트(오른팔)

    new protected void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();

        ChangeWeapon(mainWeapon);

        SetCinemachineCamera(); //모든 시네머신 버추얼 카메라에서 Player를 타겟으로 지정하게 Awake에서 실행
        HealthUI.Instance.SetGauge(hp, maxHp);
        AmmoUI.Instance.SetBulletCount(bulletCountInClip, maxBulletCountInClip, allBulletCount + bulletCountInClip, maxBulletCount); ;
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

        bulletPosition = weaponInfo.bulletPosition;

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

    void Start()
    {

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
        int reloadCount = Math.Min(allBulletCount, maxBulletCountInClip);

        AmmoUI.Instance.StartReload(reloadCount, maxBulletCountInClip, allBulletCount, maxBulletCount, reloadTime);
        yield return new WaitForSeconds(reloadTime);
        stateType = StateType.Idle;

        bulletCountInClip = reloadCount;
        allBulletCount -= reloadCount;
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

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = transform.position.y;
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
            //transform.forward = move; //이동하는 방향 바라보게 한다.

            if (Mathf.RoundToInt(transform.forward.x) == 1 || Mathf.RoundToInt(transform.forward.x) == -1)
            {
                animator.SetFloat("DirX", transform.forward.z * move.z);
                animator.SetFloat("DirY", transform.forward.x * move.x);
            }
            else
            {
                animator.SetFloat("DirX", transform.forward.x * move.x);
                animator.SetFloat("DirY", transform.forward.z * move.z);
            }

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
