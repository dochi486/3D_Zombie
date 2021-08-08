using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public enum WeaponType
    {
        Gun,
        Melee, //근접공격
    }
    public WeaponType type;
    public int damage = 20;

    public AnimatorOverrideController overrideAnimator;


    public float delay = 0.2f;
    public float pushBackDistance = 0.1f;

    [Header("총")]
    public GameObject bullet;
    public Transform bulletPosition;
    public Light bulletLight;
    public int maxBulletCount = 6;

    [Header("근접공격")]
    public float attackStartTime = 0.1f;
    public float attackTime = 0.4f;
    public Collider attackCollider;

}
