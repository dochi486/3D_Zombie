using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{

    public float bloodEffectYposition = 1.3f; //피 이펙트의 y포지션
    public GameObject bloodParticle;

    public int hp = 100;
    [HideInInspector]public int maxHp; 
    protected Animator animator;

    protected void Awake()
    {
        maxHp = hp; //최대 HP를 초기화해준다. 
    }

    protected void CreateBloodEffect()
    {
        var pos = transform.position;
        pos.y = bloodEffectYposition;
        Instantiate(bloodParticle, pos, Quaternion.identity);
    }

    public static void CreateTextEffect(int number, Vector3 position, Color color)
    {
        CreateTextEffect(number.ToNumber(), "TextEffect", position, color);
    }

    public static void CreateTextEffect(string number, string prefabName, Vector3 position, Color color)
    {
        GameObject memoryGo = (GameObject)Resources.Load(prefabName); //씬에 로드한 것이 아니라 메모리 상태로 리소스 폴더에서 로드한 게임오브젝트
        GameObject go = Instantiate(memoryGo, position, Camera.main.transform.rotation);
        TextMeshPro textMeshPro = go.GetComponent<TextMeshPro>();
        textMeshPro.text = number;
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
