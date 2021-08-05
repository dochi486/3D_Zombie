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
    public enum GetMethodType
    {
        TriggerEnter,
        KeyDown,
    }
    public GetMethodType getMethod;
    public KeyCode keyCode = KeyCode.E;

    void Awake()
    {
        enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (alreadyDone)
            return; //코루틴 함수 안에서는 return으로 나가는 게 아니라 yield break로 나간다. 

        if (other.CompareTag("Player"))
        {
            switch (getMethod)
            {
                case GetMethodType.TriggerEnter:
                    ItemAcquisition();
                    break;
                case GetMethodType.KeyDown:
                    enabled = true;
                    break;
            }

            Destroy(transform.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enabled = false;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            enabled = false;
            ItemAcquisition();
        }
    }

    private void ItemAcquisition()
    {
        alreadyDone = true;
        switch (type)
        {
            case DropItemType.Gold:
                StageManager.Instance.AddGold(amount);
                break;
        }
        Destroy(transform.root.gameObject);
    }
}
