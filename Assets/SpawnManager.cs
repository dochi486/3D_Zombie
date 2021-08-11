using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int spawnCount = 10; //10마리 생성되게 한다.
    public GameObject monster; //몬스터를 소환한다. 
    // Start is called before the first frame update
    void Start()
    {
        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            Instantiate(monster, spawnPoint, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
