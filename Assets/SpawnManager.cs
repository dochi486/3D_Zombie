using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int currentWaveIndex;

    //[System.Serializable]
    //public class RegenInfo
    //{

    //    public GameObject monster; //몬스터를 소환한다. 
    //    public float ratio; //어떤 몬스터가 어떤 확률로 소환될지 확률
    //}
    [System.Serializable]
    public class WaveInfo
    {
        public int spawnCount = 10; //10마리 생성되게 한다.
        public GameObject monster;
        public float time; //웨이브가 리젠되는 인터벌?

    }
    public List<WaveInfo> waves;

    IEnumerator Start()
    {
        
        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        foreach (var item in waves)
        {
            Debug.LogWarning($"{++currentWaveIndex} 시작");
            int spawnCount = item.spawnCount;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                Instantiate(item.monster, spawnPoint, Quaternion.identity);
            }

            float nextWaveStartTime = Time.time + item.time; //현재시간 + 웨이브의 시간
            while (Time.time < nextWaveStartTime)
                yield return null;
        }
    }

}
