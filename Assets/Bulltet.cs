using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulltet : MonoBehaviour
{
    public float speed = 20;
    public float destroyTime = 1f;
    public int power = 20;
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0,0, speed * Time.deltaTime), Space.Self);
        //포워드를 사용하려면 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Zombie") //GC발생하는 코드 CompareTag는 GC발생 안해서 CampareTag가 더 좋다
        {
            var zombie = other.GetComponent<Zombie>();
            zombie.TakeHit(power);
            Destroy(gameObject);
        }
    }
}
