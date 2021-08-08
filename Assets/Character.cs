using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{

    public float bloodEffectYposition = 1.3f; //피 이펙트의 y포지션
    public GameObject bloodParticle;

    public int hp = 100;
    protected Animator animator;

    protected void CreateBloodEffect()
    {
        var pos = transform.position;
        pos.y = bloodEffectYposition;
        Instantiate(bloodParticle, pos, Quaternion.identity);
    }

    public static void CreateTextEffect(int number, Vector3 position, Color color)
    {
        GameObject memoryGo = (GameObject)Resources.Load("TextEffect");
        GameObject go = Instantiate(memoryGo, position, Camera.main.transform.rotation);
        TextMeshPro textMeshPro = go.GetComponent<TextMeshPro>();
        textMeshPro.text = number.ToNumber();
        textMeshPro.color = color;
    }
    public Color damageColor = Color.white;

    protected void TakeHit(int damage)
    {
        hp -= damage;
        CreateBloodEffect(); //피격되면 피 이펙트 생성
        CreateTextEffect(damage, transform.position, damageColor);
    }

}
