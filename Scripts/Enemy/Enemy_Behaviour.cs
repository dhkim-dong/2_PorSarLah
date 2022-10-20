using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Behaviour : MonoBehaviour
{
    public enum State // ������ ���� ���¿� ���� ���, �̵�, ����, ���� �ൿ�� ���ϵ��� ������ FSM ������ �ۼ��Ͽ����ϴ�.
    {
        Idle, TRACE, ATTACK, DIE
    }

    /* IDLE   : ���Ϳ� �÷��̾���� �Ÿ��� �����Ÿ� ���� ª�ٸ� ����մϴ�. IDLE�� ������ �ð��� �����Ͽ� ���� �ð� �� PATROL�ϴ� ����� ������Ʈ �� �����Դϴ�.
     * TRACE  : ���Ϳ� �÷��̾���� �Ÿ��� �����Ÿ� ���̶�� �÷��̾�� �ٰ����ϴ�. 
     * ATTACK : ���Ϳ� �÷��̾���� �Ÿ��� ���ݻ�Ÿ� ���̶�� �÷��̾ �����մϴ�.
     * DIE    : �ڽ��� ���� ü���� 0���� �۰ų� ������ ��� �ൿ�� �ߴ��ϰ� �׽��ϴ�.
     */
    // ������ ���� ����
    public State state = State.Idle;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    // ���� �����Ÿ�
    public float attackDist = 2.0f;
    
    // ����� ��� : Editor ȯ�濡�� ������ �Ÿ��� Gizmo�� ���� Ȯ������ ������ ���ϴ� Bool ���Դϴ�.
    [SerializeField] bool DebugMode = false;

    [SerializeField] [Range(0,360)] float ViewAngle = 0f; // �þ߰�
    [SerializeField] [Range(0,20)] float ViewRadius = 1f; // �þ߹���
    [SerializeField] LayerMask ObstacleMask; // ��� ���� ���ع� ���� ���̾�
    [SerializeField] LayerMask whatIsTarget; // ���� ��� ���̾�
    [SerializeField] BoxCollider meleeArea;  // ������ ���� üũ�� BoxColider�� Trigger �浹�� �����Ͽ����ϴ�.
    [SerializeField] Transform playerTr;     // State�� IDLE, TRACE, ATTACK�� �����ϴ� �÷��̾��� ��ġ���� �����ϴ� �����Դϴ�. 
    // ���͸� ����ȭ�� ������ ��� �ش� ������δ� player�� ������ �� ���� �������� �ֽ��ϴ�.
    // GameObject.FindwitTag ����� ���Ͽ� Player�� ã�� ����� ���ؼ� �ش� ������ �ذ� �� �� �־����ϴ�.


    // ������Ʈ�� ĳ�� ó��
    private Transform monsterTr; // �Ÿ��� ����ϱ� ���� ������ Transform �����Դϴ�.
    private NavMeshAgent nav; // ��� ��� AI
    private Animator anim;    // �ִϸ��̼� ������ ���� �ִϸ������Դϴ�.
    private Rigidbody rigid;  // ����ȿ�� �� �̵� ������ ���Ͽ� �߰��� Component�Դϴ�.

    [SerializeField]
    private int curHp;        // Inspectorâ���� ü�� ������ ���Ͽ� ����ȭ�� �����Ͽ����ϴ�. ������ �� ������ maxHp�� ������ ������ �ʱ�ȭ �˴ϴ�.
    private int maxHp = 100;  // ������ �ִ� ü��



    // bool��
    public bool isDie;        // ���Ͱ� �׾��� �� ������ �����ϱ� ���Ͽ� Bool���� �����Ͽ����ϴ�.


     /*���������� ���� �� OnEnable������ Ȱ���Ͽ� Default������ �����ϱ� ���ؼ� ��������ϴ�. 
      * �ش� ������ ���� ������ ���Ͽ� ���������� �������� �ʾұ� ������ ������ ü�¸� �ʱ�ȭ �Ͽ����ϴ�.
      */
    private void OnEnable()  
    {        
        curHp = maxHp;
    }

    // Enemy_Behaviour�� ��ӹ޴� GameObject�� Inspector������ �ʱ�ȭ�Ͽ� ������ Null Reference������ �߻����� �ʰ� �Ͽ����ϴ�.
    private void Awake() 
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // Start�޼���� While �ݺ����� ���Ե� �ڷ�ƾ�� �����Ͽ� ������ State�� State�� ���� �ൿ ������ ó���ϵ��� �ۼ��Ͽ����ϴ�.
    void Start()
    {
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    void FreezeVelocity() // Enemy�� Rigidbody�� navmeshagent�� �̵��� �������� �ʱ� ���ؼ� �ӵ��� ������ ���ѽ��׽��ϴ�.
    {
        rigid.velocity = Vector3.zero;                // �ӵ� ó��
        rigid.angularVelocity = Vector3.zero;         // ���� ó��
    }

    private void FixedUpdate() 
    {
        FreezeVelocity();
    }

    Vector3 AngleToDir(float angle)                        //  0 ~ 360�� �þ߰��� �Ű������� ���� �Ÿ��� ���ϴ� �޼��� �Դϴ�. �ﰢ�Լ��� �̿��Ͽ����ϴ�.
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    private void OnDrawGizmos() // ������ ���ݻ�Ÿ� �߰ݻ�Ÿ��� Editor���� Wire�� ǥ�����ִ� �޼����Դϴ�.
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);

        float lookingAngle = transform.eulerAngles.y;

        // ��ä���� �þ߰��� ���߾��� ������ �Ÿ��� ����(ViewAngle)�� ���� ������ �ﰢ�Լ��� ����Ͽ� �Ÿ��� ����Ͽ����ϴ�.
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);    
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        // �þ߰��� Editorâ���� �ð������� �� �� �ֵ��� ������ ����Ͽ����ϴ�.
        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);

    }

    IEnumerator CheckMonsterState() // Moster�� State�� �����ϴ� �ٽ� �����Դϴ�.
    {
        while (!isDie)
        {

            // Ÿ�ٴ��� �ڽ��� �Ÿ��� ����մϴ�.
            yield return new WaitForSeconds(0.3f);
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            // �Ÿ��� ������� �ڽ��� State�� �����մϴ�.
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

    IEnumerator MonsterAction() // Moster ������ ���¿� ���� �ൿ�� �������ݴϴ�. 
    {
        // Case ���ǹ��� Ȱ���ؼ� Idle, Trace, Attack

        while (!isDie)
        {
            switch (state)
            {
                // nav�� ��Ȱ��ȭ �ؼ� �ڵ� ��ã�⸦ �������׽��ϴ�
                // Animator�� IDle �� �ش��ϴ� Bool���� �۽��Ͽ� �ִϸ��̼��� ����մϴ�.
                case State.Idle:
                    nav.isStopped = true;
                    anim.SetBool("isWalk", false);
                    break;

                // nav�� ����� Target(Player) �Ҵ��մϴ�.
                // ������ nav�� Ȱ��ȭ �մϴ�.
                // Animator�� Trace �� �ش��ϴ� Bool���� �۽��Ͽ� �ִϸ��̼��� ����մϴ�.
                case State.TRACE:
                    nav.SetDestination(playerTr.position);
                    nav.isStopped = false;

                    anim.SetBool("isWalk", true);
                    anim.SetBool("isAttack", false);
                    break;

                // �ڷ�ƾ�� Ȱ���Ͽ� ������ ������ �����Ͽ����ϴ�.
                // Animator�� IDle �� �ش��ϴ� Bool���� �۽��Ͽ� �ִϸ��̼��� ����մϴ�.
                case State.ATTACK:
                    StartCoroutine(MonAttack());
                    anim.SetBool("isAttack", true);
                    break;                  
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator MonAttack() // �ڷ�ƾ�� Ȱ���Ͽ� ������ ������ �����Ͽ����ϴ�.
    {
        yield return new WaitForSeconds(0.2f); // ������ �� �����̸� ��� ���Ͽ� �߰��Ͽ����ϴ�.
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f); // ������ �����ϴ� �ð��Դϴ�. �ش� �ð� ���� ���� ���ظ� ���� �� �ֽ��ϴ�.
        meleeArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other) // ������ �ǰ��� Trigger Event�� �����Ͽ����ϴ�. PlayerBullet�� �ǰݴ��� �� �������� �԰�, ü���� 0���� �۰ų� �������� �׽��ϴ�.
    {
        if (!isDie)
        {
            if (other.CompareTag("PlayerBullet")) // Trigger�� ������ Tag�� �����Ͽ����ϴ�.
            {
                Destroy(other.gameObject); // ���Ϳ� �ǰ��� źȯ�� �����մϴ�.

                anim.SetTrigger("Hit"); // � �ൿ ���̶� �ǰ��ϸ� �ߵ��ϵ��� Trigger�� �����ϰ� AnyState�� �Ҵ��Ͽ����ϴ�.

                // Player�� ������ �ϳ��θ� �����Ͽ� Const ����� �ο��Ͽ����ϴ�. 
                // Weapon Script�� Ȱ���Ͽ� ������ ���� data�� �����ϰ� �� data�� �ҷ����� ������� �پ��� ������ ���� ������ �߰��ϴ� ������� ������Ʈ�� �����Դϴ�.
                curHp -= 10;

                // ������ ������ �����Ͽ����ϴ�.
                if (curHp <= 0)
                {
                    AudioManager.instance.Zombie_Die();
                    QuestManager.questManager.AddQuestItem("1", 1); // ����Ʈ 1���� 1�� ����Ʈ Count�� ��½����ִ� �޼��带 ������ �̱����� �ҷ��ɴϴ�.
                    state = State.DIE;
                    isDie = true;
                    nav.isStopped = true;
                    anim.SetTrigger("doDie");
                    GetComponent<CapsuleCollider>().enabled = false;
                    Destroy(gameObject, 2.9f); 
                    // ������ �״� �ִϸ��̼ǰ� ���Ͱ� �����ִ� ��ũ�� �����ֱ� ���� ����� �Է����־����ϴ�.
                    // �پ��� ���͸� ������ ��� ������ ���Ϳ� �°� �״� �ð��� ���ߴ� ������Ʈ�� �ʿ��մϴ�.
                }
            }
        }
    }
}
