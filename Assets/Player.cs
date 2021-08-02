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
            move.Normalize();
            transform.Translate(speed * move * Time.deltaTime, Space.World);
            //transform.forward = move; //이동하는 방향 바라보게 한다.
        }
        //애니메이터의 파라미터 Speed를 실제 이동하는 속도 move.sqrMagnitude로 설정한다.
        animator.SetFloat("DirX", move.x);
        animator.SetFloat("DirY", move.z);
        animator.SetFloat("Speed", move.sqrMagnitude);
    }
    //private void Fire()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        //animator.Play("Shoot");
    //        animator.SetBool("Fire", true);
    //        Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
    //    }
    //    else
    //    {
    //        animator.SetBool("Fire", false);
    //    }
    //}
}
