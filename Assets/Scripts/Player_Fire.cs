using System.Collections;
using UnityEngine;

public partial class Player : Character
{
    public int bulletCountInClip = 2; //탄창에 현재 있는 총알 수???
    public int maxBulletCountInClip = 6;
    public int allBulletCount = 500; //가지고 있는 총 총알 수
    public float reloadTime = 1f;

    public GameObject bullet;
    public Transform bulletPosition;

    float shootDelayEndTime;
    void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            isFiring = true;
            if (shootDelayEndTime < Time.time && bulletCountInClip > 0)
            {
                bulletCountInClip--;
                animator.SetTrigger("StartFire");

                AmmoUI.Instance.SetBulletCount(bulletCountInClip, maxBulletCountInClip, allBulletCount);

                //animator.SetBool("Fire", true);
                shootDelayEndTime = Time.time + shootDelay;

                switch (currentWeapon.type)
                {
                    case WeaponInfo.WeaponType.Gun:
                        IncreaseRecoil();
                        currentWeapon.StartCoroutine(InstantiateBulletFlashBulletCo());
                        break;
                    case WeaponInfo.WeaponType.Melee:
                        currentWeapon.StartCoroutine(MeleeAttackCo());
                        break;
                }

            }
        }
        else
        {
            EndFiring();
        }
    }

    IEnumerator MeleeAttackCo()
    {
        yield return new WaitForSeconds(currentWeapon.attackStartTime);
        currentWeapon.attackCollider.enabled = true;
        yield return new WaitForSeconds(currentWeapon.attackTime);
        currentWeapon.attackCollider.enabled = false;
    }

    private void EndFiring()
    {
        animator.SetBool("Fire", false); //Roll하는 동안에는 fire애니메이션 안되도록
        DecreaseRecoil();
        isFiring = false;
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
