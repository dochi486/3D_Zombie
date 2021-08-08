using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class MoveToPlayer : MonoBehaviour
{
    public float maxSpeed = 20;
    NavMeshAgent agent;
    public float duration = 3; //3초동안 maxSpeed까지 속도가 상승하게 한다. 
    public bool alreadyDone = false;
    TweenerCore<float, float, FloatOptions> tweenResult;


    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (alreadyDone)
            yield break; //코루틴 함수 안에서는 return으로 나가는 게 아니라 yield break로 나간다. 

        if (other.CompareTag("Player"))
        {
            alreadyDone = true;
            agent = GetComponent<NavMeshAgent>();

            //tweenResult = DOTween.To(() => agent.speed, x => (agent.speed) = x, maxSpeed, duration);
            //maxspeed까지 duration동안 agent.speed를 변환시켜주겠다.
            DOTween.To(() => agent.speed, x => (agent.speed) = x, maxSpeed, duration).SetLink(gameObject);
            //게임오브젝트랑 DoTween을 연결시켜서 게임오브젝트가 파괴되면 DoTween도 함께 멈추도록 한다. 


            setDestinationCoHandle = StartCoroutine(SetDestiantionCo(other.transform));
        }

    }

    Coroutine setDestinationCoHandle;

    public void StopCoroutine()
    {
        StopCoroutine(setDestinationCoHandle);
    }

    IEnumerator SetDestiantionCo(Transform tr)
    {
        while (true)
        {
            agent.destination = tr.position;
            yield return null;
        }
    }
}
//private void OnDestroy()
//{
//    tweenResult.Kill();
//}
//코인이 플레이어를 한 번 감지한 후에 또 감지하지 못하도록 컬라이더를 끄거나 불 변수를 추가할 수 있는데
//컬라이더를 끄는 건 나중에 이 클래스가 다른 기능을 할 수 있으므로 되도록 불 변수 추가하는 게 좋다.
