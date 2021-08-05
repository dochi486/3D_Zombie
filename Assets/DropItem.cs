using UnityEngine;

public enum DropItemType
{
    Gold,
    Point,
    Item,
}
public class DropItem : MonoBehaviour
{
    public DropItemType type;
    public int amount; //드랍된 개수
    public int itemID; //아이템이라면 아이템의 아이디가 뭔지
    bool alreadyDone = false;
    private void OnTriggerEnter(Collider other)
    {

        if (alreadyDone)
            return; //코루틴 함수 안에서는 return으로 나가는 게 아니라 yield break로 나간다. 

        if (other.CompareTag("Player"))
        {
            alreadyDone = true;
            switch (type)
            {
                case DropItemType.Gold:
                    break;
                case DropItemType.Point:
                    break;
                case DropItemType.Item:
                    break;

            }

            Destroy(transform.parent.gameObject);
        }


    }
}
