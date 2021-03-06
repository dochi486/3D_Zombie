using System.Collections;
using UnityEngine;

public partial class Player : Character
{
    public int BulletCountInClip
    {
        get => currentWeapon.bulletCountInClip;
        set => currentWeapon.bulletCountInClip = value;
    }//탄창에 현재 있는 총알 수???
    public int MaxBulletCountInClip => currentWeapon.maxBulletCountInClip; //탄창에 들어가는 최대 총알 수
    public int AllBulletCount
    {
        get => currentWeapon.allBulletCount;
        set => currentWeapon.allBulletCount = value;
    } //가지고 있는 총 총알 수
    //속성을 이렇게도 사용 가능 (한 줄짜리 속성을 사용 할 때 이렇게 많이 쓴다)
    public int MaxBulletCount => currentWeapon.maxBulletCount; //최대로 가질 수 있는 총알 수
    public float ReloadTime => currentWeapon.reloadTime;
    public GameObject Bullet => currentWeapon.bullet;
    public Transform BulletPosition => currentWeapon.bulletPosition;

    float shootDelayEndTime;
    void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (BulletCountInClip > 0)
            {
                isFiring = true;
                if (shootDelayEndTime < Time.time)
                {
                    BulletCountInClip--; //감소시켜야하기 때문에(이름 정확히 모르겠다 후치 어쩌구 같은데) 읽기전용 속성 아닌 쓰기까지 가능한 속성으로 만들어줌
                    animator.SetTrigger("StartFire");

                    AmmoUI.Instance.SetBulletCount(BulletCountInClip, MaxBulletCountInClip,
                        AllBulletCount + BulletCountInClip, MaxBulletCount);

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
                        case WeaponInfo.WeaponType.Throw:
                            currentWeapon.StartCoroutine(ThrowAttackCo());
                            break;
                    }
                }

            }
            else
            {
                if (reloadAllertDelayEndTime < Time.time)
                {
                    reloadAllertDelayEndTime = Time.time + reloadAllertDelay;

                    CreateTextEffect("Reload!", "TextBubble", transform.position, Color.white, transform);
                }
            }

        }
        else
        {
            EndFiring();
        }
    }

    private string ThrowAttackCo()
    {
        throw new System.NotImplementedException(); //수류탄 작업 안해서 함수 작성을 못해~
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
        //animator.SetBool("Fire", false); //Roll하는 동안에는 fire애니메이션 안되도록
        DecreaseRecoil();
        isFiring = false;
    }

    public GameObject bulletLight;
    public float bulletFlashTime = 0.001f;
    private IEnumerator InstantiateBulletFlashBulletCo()
    {
        yield return null; //총 쏘는 애니메이션 시작 후에 총알이 나가도록 1프레임 쉰다. 
        GameObject bulletGo = Instantiate(Bullet, BulletPosition.position, CalculateRecoil(transform.rotation));
        bulletGo.GetComponent<Bulltet>().pushBackDistance = currentWeapon.pushBackDistance;

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
    [SerializeField] float reloadAllertDelay = 0.05f;
    float reloadAllertDelayEndTime;
}
