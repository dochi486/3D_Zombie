using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : SingletonMonoBehavior<SpawnManager>
{
    public int currentWaveIndex;

    [System.Serializable]
    public class RegenInfo
    {

        public GameObject monster; //몬스터를 소환한다. 
        public float ratio; //어떤 몬스터가 어떤 확률로 소환될지 확률
    }
    [System.Serializable]
    public class WaveInfo
    {
        public int spawnCount = 10; //10마리 생성되게 한다.
        //public GameObject monster;
        public List<RegenInfo> monsters;
        public float time; //웨이브가 리젠되는 인터벌?

    }
    public List<WaveInfo> waves;

    public void OnClearAllMonster()
    {
        nextWaveStartTime = 0;
    }
    float nextWaveStartTime;
    public float randomRegenDelayMax = 0.5f;
    IEnumerator Start()
    {


        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
        foreach (var item in waves)
        {
            Debug.LogWarning($"{++currentWaveIndex}번째 웨이브 시작");
            int spawnCount = item.spawnCount;

            for (int i = 0; i < spawnCount; i++)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Length);
                Vector3 spawnPoint = spawnPoints[spawnIndex].transform.position;
                var monster = item.monsters.OrderBy(x => Random.Range(0, x.ratio)).Last().monster;
                Instantiate(monster, spawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(0, randomRegenDelayMax));

            }

            nextWaveStartTime = Time.time + item.time; //현재시간 + 웨이브의 시간
            while (Time.time < nextWaveStartTime) //웨이브가 끝나기 전에 모든 몬스터를 죽이면 다음 웨이브가 시작되도록한다. 
                yield return null;

            LightManager.Instance.ToggleLight(); //웨이브가 끝나면 낮밤 변경되는 부분
        }
    }

}
