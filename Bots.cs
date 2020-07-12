using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))] // 스크립트를 적용했을때, NavMeshAgent를 자동으로 적용시켜줍니다.
public class Bots : MonoBehaviour
{
    public GameObject PlayerModel; // 플레이어 모델링
    public Animator Animators; // 애니메이터
    public float Random_X; //X축의 위치를 무작위합니다.
    public float Random_Z; //Z축의 위치를 무작위합니다.

    Vector3 _posTarget, _startPos; // 시작할 위치를 설정하고, 목적지를 설정합니다.

    public NavMeshAgent agents; // 네비메쉬를 적용시켜줍니다.

    public string Team; // 타겟을 설정 (Tag 설정)
    public float BasicRange = 12.5f; // 기본 시야 거리

    public float Angle = 35f; // 시야 각도
    public float BotsRange = 45; // 봇의 시야
    public float AttackRange = 25; // 공격 거리 
    public float EnemyByDamage = 25f; // 적이 받을 데미지, 또는 무기의 성능


    public float timeBetweenShots = 0.3f; // 설정한 발사 속도 
    private float shotCounter; // 발사 속도


    public float SlowSpeed = 9f; // 적과 교전할때의 속도
    public float BasicSpeed = 15f; // 정찰할때의 속도

    // Start is called before the first frame update
    void Start()
    {
        agents = this.GetComponent<NavMeshAgent>(); // 네비메쉬 불러오기
        _startPos = _posTarget = transform.position; // 시작할 위치를 목적지로 설정합니다.
        Animators = PlayerModel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 매치가 시작되면 AI가 실행.
        if(GameManager.instance.isStart) {
            UpdateTheTarget();
            EnemyControlFree();
        }
    }

    // 적을 발견했을때, 적을 공격합니다.
    void UpdateTheTarget()
    {
        Animators.SetFloat("Walk",agents.speed);

        agents.speed = BasicSpeed; // 정찰 속도

        GameObject[] targets = GameObject.FindGameObjectsWithTag(Team); // 타겟을 인식

        foreach (GameObject enemy in targets) // 모든 타겟을 전체 설정
        {
            Vector3 LookTheTarget = (enemy.transform.position - transform.position).normalized; // 봇의 타겟 거리 인식을 설정합니다. 

            float T_Angle = Vector3.Angle(LookTheTarget, transform.forward); // 봇의 시야를 설정합니다. 
            if(T_Angle < Angle * 0.5f) // 만약 설정한 각도가 봇의 시야에 있다면
            {
                RaycastHit _hits;
                if(Physics.Raycast(transform.position, LookTheTarget, out _hits, BotsRange)) // 적의 거리 인식
                {
                    if(_hits.collider.tag == Team) // 봇이 타겟을 인식했다면?
                    {
                        agents.speed = SlowSpeed; // 속도를 줄여서 적과 교전하는데 안정적인 속도를 만듭니다..
                        BotsStartAttack(); // 플레이어를 공격합니다.

                        Vector3 targetPosition = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z); // 타겟을 바라보기 위해, 벡터를 설정합니다.
                        transform.LookAt(targetPosition); // 타겟을 바라봅니다.
                    }
                }
            }
        }    
    }

    void BotsStartAttack()
    {
        if(shotCounter > 0) // 총을 발사속도가 0보다 높을때 
        {
            shotCounter -= Time.deltaTime; // 발사 속도 발생
        }
        else
        {   
            shotCounter = timeBetweenShots; // 총 발사 속도를 설정한 발사속도로 설정된 채로 실행

            RaycastHit hits;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hits, AttackRange)) // 적의 공격 거리 인식
            {
                if(hits.collider.tag == Team) // 만약 타겟이 맞았다면
                {    
                    Animators.SetTrigger("Fire"); // 애니메이션이 실행됩니다..
                    hits.collider.transform.GetComponent<TempleHealths>().TakeDamage(EnemyByDamage); // 타겟과 플레이어가 데미지를 맞습니다.
                }
            }
        }
    }

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

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BasicRange);
    }
}
