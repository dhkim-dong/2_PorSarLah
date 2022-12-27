using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float MaxDistance;
    [SerializeField] LayerMask itemlayerMask;
    [SerializeField] LayerMask NPCLayerMask;
    RaycastHit hit;
    private bool isQuestText;

    private float p_curHp;
    public float p_maxHp = 100;

    [SerializeField] Image p_Hpbar;

    // Start is called before the first frame update
    void Start()
    {
        p_curHp = p_maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (isQuestText)
        {
            GameManager.instance.isQuestItem = false;
            GameManager.instance.isNpc = false;
            isQuestText = false;
        }
        CheckItemByRayCastHit();
        //HpBarUpdateUI();
    }

    public void CheckItemByRayCastHit()
    {
        Debug.DrawRay(transform.position, transform.forward * MaxDistance);

        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance, itemlayerMask))
        {
            GameManager.instance.isQuestItem = true;
            isQuestText = true;
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance, NPCLayerMask))
        {
            GameManager.instance.isNpc = true;
            isQuestText = true;
        }
    }

    void HpBarUpdateUI()
    {
        p_Hpbar.fillAmount = p_curHp / p_maxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAtk"))
        {
            p_curHp -= 10; // 적에게 공격 당할 시 체력 감소

            if(p_curHp <= 0)
            {
                SceneManager.LoadScene("TeamProject");
            }
        }     
    }


    public void IncreaseHealth(int value)
    {
        p_curHp += value;
        if (p_curHp >= p_maxHp)
            p_curHp = p_maxHp;
    }
}
