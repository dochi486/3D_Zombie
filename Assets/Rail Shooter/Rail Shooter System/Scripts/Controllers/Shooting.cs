using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private RaycastHit hit;

    public Gun gun;

    public GameObject wallHitEffect;

    public GameObject enemyHitEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("y");
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            PlayGunEffects();

            if (Physics.Raycast(ray, out hit, gun.fireLength))
            {
                hit.transform.gameObject.SendMessage("ReceiveDamage", gun.damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void PlayGunEffects()
    {
        gun.muzzleFlash.Play();

        gun.gunshotAudio.Play();
    }
}
