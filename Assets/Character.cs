using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public float bloodEffectYposition = 1.3f; //피 이펙트의 y포지션
    public GameObject bloodParticle;

    protected int hp = 100;
    protected Animator animator;

    protected void CreateBloodEffect()
    {
        var pos = transform.position;
        pos.y = bloodEffectYposition;
        Instantiate(bloodParticle, pos, Quaternion.identity);
    }

}
