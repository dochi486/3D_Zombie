using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public float speed = 3f;

    // Update is called once per frame
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

    }
}
