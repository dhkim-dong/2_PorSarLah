using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Behaviour : MonoBehaviour
{
    public enum State
    {
        Idle, TRACE, ATTACK, DIE
    }
    // 몬스터의 현재 상태
    public State state = State.Idle;
    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    public float attackDist = 2.0f;
    
    // 디버그 모드
    [SerializeField] bool DebugMode = false;

    [SerializeField] [Range(0,360)] float ViewAngle = 0f; // 시야각
    [SerializeField] [Range(0,20)] float ViewRadius = 1f; // 시야범위
    [SerializeField] LayerMask ObstacleMask; // 경로 상의 방해물 추적 레이어
    [SerializeField] LayerMask whatIsTarget; // 추적 대상 레이어
    [SerializeField] BoxCollider meleeArea;
    [SerializeField] Transform playerTr;


    // 컴포넌트의 캐시 처리
    private Transform monsterTr;
    private NavMeshAgent nav; // 경로 계산 AI
    private Animator anim;
    private Rigidbody rigid;
    RaycastHit hit;

    public int curHp;
    private int maxHp = 100;



    // bool값
    public bool isDie;
    private bool isAttack;
    private bool isChase;


    private void OnEnable()
    {        
        curHp = maxHp;
    }

    private void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);

        float lookingAngle = transform.eulerAngles.y;

        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if(distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if(distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.Idle;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.Idle:
                    nav.isStopped = true;

                    anim.SetBool("isWalk", false);
                    break;

                case State.TRACE:
                    nav.SetDestination(playerTr.position);
                    nav.isStopped = false;

                    anim.SetBool("isWalk", true);
                    anim.SetBool("isAttack", false);
                    break;
                case State.ATTACK:
                    anim.SetBool("isAttack", true);
                    break;                  
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator MonAttack()
    {
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDie)
        {
            if (other.CompareTag("PlayerBullet"))
            {
                Destroy(other.gameObject);

                anim.SetTrigger("Hit");

                curHp -= 10;

                if (curHp <= 0)
                {
                    AudioManager.instance.Zombie_Die();
                    QuestManager.questManager.AddQuestItem("1", 1);
                    state = State.DIE;
                    isDie = true;
                    nav.isStopped = true;
                    anim.SetTrigger("doDie");
                    GetComponent<CapsuleCollider>().enabled = false;
                    Destroy(gameObject, 2.9f);
                }
            }
        }
    }

    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }
}
