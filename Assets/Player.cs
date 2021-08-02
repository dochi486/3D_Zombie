using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        instance = this;
        animator = GetComponentInChildren<Animator>();
        //rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }
    public static Player instance;
    public float speed = 3f;
    public GameObject bullet;
    public Transform bulletPosition;
    //public Rigidbody rigid;


    void Update()
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
            transform.forward = move; //이동하는 방향 바라보게 한다.
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

            animator.Play("Shoot");
            Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        }
        animator.SetFloat("DirX", move.x);
        animator.SetFloat("DirY", move.z);
        animator.SetFloat("Speed", move.sqrMagnitude);
        //애니메이터의 파라미터 Speed를 실제 이동하는 속도 move.sqrMagnitude로 설정한다.
    }
}
