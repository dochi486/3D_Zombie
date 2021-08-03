using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{

    float shootDelayEndTime;
    void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            isFiring = true;
            if (shootDelayEndTime < Time.time)
            {
                animator.SetBool("Fire", true);
                shootDelayEndTime = Time.time + shootDelay;
                IncreaseRecoil();
                StartCoroutine(InstantiateBulletFlashBulletCo());
            }
        }
        else
        {
            EndFiring();
        }
    }

    GameObject bulletLight;
    public float bulletFlashTime = 0.001f;
    private IEnumerator InstantiateBulletFlashBulletCo()
    {
        yield return null; //총 쏘는 애니메이션 시작 후에 총알이 나가도록 1프레임 쉰다. 
        Instantiate(bullet, bulletPosition.position, CalculateRecoil(transform.rotation));

        bulletLight.SetActive(true);
        yield return new WaitForSeconds(bulletFlashTime);
        bulletLight.SetActive(false);
    }

    float recoilValue = 0f;
    float recoilMaxValue = 1.5f;
    float recoilLerpValue = 0.1f;
    void IncreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, recoilMaxValue, recoilLerpValue);
    }
    void DecreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, 0, recoilLerpValue);

    }

    Vector3 recoil;
    Quaternion CalculateRecoil(Quaternion rotation)
    {
        recoil = new Vector3(Random.Range(-recoilValue, recoilValue), Random.Range(-recoilValue, recoilValue), 0);
        return Quaternion.Euler(rotation.eulerAngles + recoil);
    }

    [SerializeField] float shootDelay = 0.05f;
}
