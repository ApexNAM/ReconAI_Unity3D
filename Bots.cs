using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))] // 스크립트를 적용했을때, NavMeshAgent를 자동으로 적용시켜줍니다.
public class Bots : MonoBehaviour
{
    public float Random_X; //X축의 위치를 무작위합니다.
    public float Random_Z; //Z축의 위치를 무작위합니다.

    Vector3 _posTarget, _startPos; // 시작할 위치를 설정하고, 목적지를 설정합니다.

    public NavMeshAgent agents; // 네비메쉬를 적용시켜줍니다.
    
    
    // 정찰하는 기능
    void EnemyControlFree()
    {
        if(agents.velocity == Vector3.zero) // 만약 목적지에 도착하면 무작위로 다시 목적지를 설정합니다.
        {
            _posTarget = GetRandomPos(_startPos, Random_X, Random_Z); // 목적지를 설정하거나 재설정합니다.
            agents.destination = _posTarget; // 목적지 설정으로 이동합니다.
        }
    }

    // 무작위로 목적지의 축을 설정합니다.
    Vector3 GetRandomPos(Vector3 center, float RandomX, float RandomZ)
    {
        float rX = Random.Range(-RandomX, RandomZ);
        float rZ = Random.Range(-RandomZ, RandomZ);

        Vector3 rv = new Vector3(rX,0,rZ);
        return center + rv;
    }
}
