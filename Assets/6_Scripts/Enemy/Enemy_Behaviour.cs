using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Behaviour : MonoBehaviour
{
    public enum State // 몬스터의 현재 상태에 따라 대기, 이동, 공격, 죽음 행동을 취하도록 간단한 FSM 구조로 작성하였습니다.
    {
        Idle, TRACE, ATTACK, DIE
    }

    /* IDLE   : 몬스터와 플레이어와의 거리가 추적거리 보다 짧다면 대기합니다. IDLE에 진입한 시간을 저장하여 일정 시간 후 PATROL하는 기능을 업데이트 할 예정입니다.
     * TRACE  : 몬스터와 플레이어와의 거리가 추적거리 안이라면 플레이어에게 다가갑니다. 
     * ATTACK : 몬스터와 플레이어와의 거리가 공격사거리 안이라면 플레이어를 공격합니다.
     * DIE    : 자신의 현재 체력이 0보다 작거나 같으면 모든 행동을 중단하고 죽습니다.
     */
    // 몬스터의 현재 상태
    public State state = State.Idle;
    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    public float attackDist = 2.0f;
    
    // 디버그 모드 : Editor 환경에서 몬스터의 거리를 Gizmo를 통해 확인할지 안할지 정하는 Bool 값입니다.
    [SerializeField] bool DebugMode = false;

    [SerializeField] [Range(0,360)] float ViewAngle = 0f; // 시야각
    [SerializeField] [Range(0,20)] float ViewRadius = 1f; // 시야범위
    [SerializeField] LayerMask ObstacleMask; // 경로 상의 방해물 추적 레이어
    [SerializeField] LayerMask whatIsTarget; // 추적 대상 레이어
    [SerializeField] BoxCollider meleeArea;  // 몬스터의 공격 체크를 BoxColider의 Trigger 충돌로 구현하였습니다.
    [SerializeField] Transform playerTr;     // State중 IDLE, TRACE, ATTACK을 결정하는 플레이어의 위치값을 저장하는 변수입니다. 
    // 몬스터를 직렬화로 연결할 경우 해당 방식으로는 player를 가져올 수 없는 문제점이 있습니다.
    // GameObject.FindwitTag 기능을 통하여 Player을 찾는 방법을 통해서 해당 문제는 해결 할 수 있었습니다.


    // 컴포넌트의 캐시 처리
    private Transform monsterTr; // 거리를 계산하기 위해 저장한 Transform 변수입니다.
    private NavMeshAgent nav; // 경로 계산 AI
    private Animator anim;    // 애니메이션 구현을 위한 애니메이터입니다.
    private Rigidbody rigid;  // 물리효과 및 이동 구현을 위하여 추가된 Component입니다.

    [SerializeField]
    private int curHp;        // Inspector창에서 체력 수정을 위하여 직렬화로 연결하였습니다. 생성될 때 지정된 maxHp와 동일한 값으로 초기화 됩니다.
    private int maxHp = 100;  // 몬스터의 최대 체력



    // bool값
    public bool isDie;        // 몬스터가 죽었을 때 변수를 관리하기 위하여 Bool값을 선언하였습니다.


     /*프리펩으로 생성 시 OnEnable변수를 활용하여 Default설정을 구현하기 위해서 만들었습니다. 
      * 해당 버전은 빠른 개발을 위하여 프리펩으로 구현하지 않았기 때문에 몬스터의 체력만 초기화 하였습니다.
      */
    private void OnEnable()  
    {        
        curHp = maxHp;
    }

    // Enemy_Behaviour를 상속받는 GameObject의 Inspector값들을 초기화하여 변수에 Null Reference에러가 발생하지 않게 하였습니다.
    private void Awake() 
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // Start메서드로 While 반복문이 포함된 코루틴을 실행하여 몬스터의 State와 State에 따른 행동 로직을 처리하도록 작성하였습니다.
    void Start()
    {
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    void FreezeVelocity() // Enemy의 Rigidbody가 navmeshagent의 이동을 방해하지 않기 위해서 속도와 각도를 제한시켰습니다.
    {
        rigid.velocity = Vector3.zero;                // 속도 처리
        rigid.angularVelocity = Vector3.zero;         // 각도 처리
    }

    private void FixedUpdate() 
    {
        FreezeVelocity();
    }

    Vector3 AngleToDir(float angle)                        //  0 ~ 360의 시야각을 매개변수에 따른 거리를 구하는 메서드 입니다. 삼각함수를 이용하였습니다.
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    private void OnDrawGizmos() // 몬스터의 공격사거리 추격사거리를 Editor에서 Wire로 표시해주는 메서드입니다.
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);

        float lookingAngle = transform.eulerAngles.y;

        // 부채꼴의 시야각의 정중앙을 지나는 거리와 각도(ViewAngle)의 절반 각도를 삼각함수를 사용하여 거리를 계산하였습니다.
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);    
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        // 시야각을 Editor창에서 시각적으로 볼 수 있도록 선으로 출력하였습니다.
        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

    }

    IEnumerator CheckMonsterState() // Moster의 State를 결정하는 핵심 로직입니다.
    {
        while (!isDie)
        {

            // 타겟대상과 자신의 거리를 계산합니다.
            yield return new WaitForSeconds(0.3f);
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            // 거리를 기반으로 자신의 State를 결정합니다.
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

    IEnumerator MonsterAction() // Moster 각각의 상태에 따른 행동을 설정해줍니다. 
    {
        // Case 조건문을 활용해서 Idle, Trace, Attack

        while (!isDie)
        {
            switch (state)
            {
                // nav를 비활성화 해서 자동 길찾기를 중지시켰습니다
                // Animator의 IDle 에 해당하는 Bool값을 송신하여 애니메이션을 출력합니다.
                case State.Idle:
                    nav.isStopped = true;
                    anim.SetBool("isWalk", false);
                    break;

                // nav의 대상을 Target(Player) 할당합니다.
                // 정지된 nav를 활성화 합니다.
                // Animator의 Trace 에 해당하는 Bool값을 송신하여 애니메이션을 출력합니다.
                case State.TRACE:
                    nav.SetDestination(playerTr.position);
                    nav.isStopped = false;

                    anim.SetBool("isWalk", true);
                    anim.SetBool("isAttack", false);
                    break;

                // 코루틴을 활용하여 몬스터의 공격을 구현하였습니다.
                // Animator의 IDle 에 해당하는 Bool값을 송신하여 애니메이션을 출력합니다.
                case State.ATTACK:
                    StartCoroutine(MonAttack());
                    anim.SetBool("isAttack", true);
                    break;                  
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator MonAttack() // 코루틴을 활용하여 몬스터의 공격을 구현하였습니다.
    {
        yield return new WaitForSeconds(0.2f); // 공격의 선 딜레이를 잡기 위하여 추가하였습니다.
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f); // 공격이 잔존하는 시간입니다. 해당 시간 내에 연속 피해를 입을 수 있습니다.
        meleeArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other) // 몬스터의 피격을 Trigger Event로 구현하였습니다. PlayerBullet에 피격당할 시 데미지를 입고, 체력이 0보다 작거나 같아지면 죽습니다.
    {
        if (!isDie)
        {
            if (other.CompareTag("PlayerBullet")) // Trigger에 반응할 Tag를 설정하였습니다.
            {
                Destroy(other.gameObject); // 몬스터에 피격한 탄환을 제거합니다.

                anim.SetTrigger("Hit"); // 어떤 행동 중이라도 피격하면 발동하도록 Trigger로 설정하고 AnyState에 할당하였습니다.

                // Player의 공격을 하나로만 구현하여 Const 상수를 부여하였습니다. 
                // Weapon Script를 활용하여 무기의 공격 data를 관리하고 그 data를 불러오는 방식으로 다양한 무기의 공격 변수를 추가하는 방식으로 업데이트할 사항입니다.
                curHp -= 10;

                // 몬스터의 죽음을 구현하였습니다.
                if (curHp <= 0)
                {
                    AudioManager.instance.Zombie_Die();
                    QuestManager.questManager.AddQuestItem("1", 1); // 퀘스트 1번에 1의 퀘스트 Count를 상승시켜주는 메서드를 구현한 싱글톤을 불러옵니다.
                    state = State.DIE;
                    isDie = true;
                    nav.isStopped = true;
                    anim.SetTrigger("doDie");
                    GetComponent<CapsuleCollider>().enabled = false;
                    Destroy(gameObject, 2.9f); 
                    // 몬스터의 죽는 애니메이션과 몬스터가 없애주는 싱크를 맞춰주기 위해 상수를 입력해주었습니다.
                    // 다양한 몬스터를 생성할 경우 각각의 몬스터에 맞게 죽는 시간을 맞추는 업데이트가 필요합니다.
                }
            }
        }
    }
}
