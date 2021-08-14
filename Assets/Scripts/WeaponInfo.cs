using System;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public enum WeaponType
    {
        Gun,
        Melee, //근접공격
        Throw, //수류탄 같은 투척 무기!
    }
    public WeaponType type;
    public int damage = 20;

    public AnimatorOverrideController overrideAnimator;

    public int bulletCountInClip = 2; //탄창에 현재 있는 총알 수???
    public int maxBulletCountInClip = 6; //탄창에 들어가는 최대 총알 수
    public int allBulletCount = 500; //가지고 있는 총 총알 수
    public int maxBulletCount = 500; //최대로 가질 수 있는 총알 수
    public float reloadTime = 1f;
    //위의 변수들은 나중에 WeaponInfo로 옮길 것

    public float delay = 0.2f;
    public float pushBackDistance = 0.1f;

    [Header("총")]
    public GameObject bullet;
    public Transform bulletPosition;
    public Light bulletLight;
    //public int maxBulletCount = 6;

    [Header("근접공격")]
    public float attackStartTime = 0.1f;

    internal void Init()
    {
        allBulletCount = Math.Min(allBulletCount, maxBulletCount);
        int reloadCount = Math.Min(allBulletCount, maxBulletCountInClip); //내가 가진 것과 탄창에 들어가는 것 비교해서 더 작은 값 사용
        allBulletCount -= reloadCount;
        bulletCountInClip = reloadCount;
    }

    public float attackTime = 0.4f;
    public Collider attackCollider;

    [Header("투척무기")]
    public GameObject throwGo;


}
