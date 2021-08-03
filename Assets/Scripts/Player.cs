using System;
using System.Collections;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        bulletLight = GetComponentInChildren<Light>(true).gameObject;
    }
    void Start()
    {

    }
    public float speed = 3f;
    public GameObject bullet;
    public Transform bulletPosition;

    void Update()
    {
        LookAtMouse();
        Move();
        Fire();
        Roll();
    }

    private void Roll()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RollCo());
        }
    }

    private IEnumerator RollCo()
    {
        //구르는 애니메이션 재생
        animator.SetTrigger("Roll");
        yield return null;
        //구르는 동안 플레이어의 스피드를 빠르게 바꾼다. 

        //구르는 방향은 처음 바라보고 있던 방향으로 고정한다.

        //구르는 동안은 총알 발사 안되도록, 다른 방향으로 이동도 불가, 마우스 포인터 방향으로 바라보지도 않게


    }

    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    void LookAtMouse()
    {
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
            transform.Translate(speed * move * Time.deltaTime, Space.World);
            //transform.forward = move; //이동하는 방향 바라보게 한다.
        }
        //애니메이터의 파라미터 Speed를 실제 이동하는 속도 move.sqrMagnitude로 설정한다.
        animator.SetFloat("DirX", move.x);
        animator.SetFloat("DirY", move.z);
        animator.SetFloat("Speed", move.sqrMagnitude);
    }
}
