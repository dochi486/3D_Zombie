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
            relativeMove = Camera.main.transform.forward * move.z; //앞뒤이동 상대값
            relativeMove += Camera.main.transform.right * move.x; //좌우이동 상대값
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
