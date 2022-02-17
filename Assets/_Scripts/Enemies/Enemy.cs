using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Mostly responsible for enemy collisions & data storage
public class Enemy : MonoBehaviour, IHittable, IAgent
{
    [field: SerializeField]
    public EnemyDataSO EnemyData { get; set; }

    [field: SerializeField]
    public int Health { get; set; }

    [field: SerializeField]
    public int Damage { get; private set; }

    [field: SerializeField]
    public float Range { get; private set; }

    [field: SerializeField]
    public UnityEvent OnGetHit { get; set; }

    [field: SerializeField]
    public UnityEvent OnDie { get; set; }
    public bool isDying = false;

    private AgentRenderer agentRenderer;
    private EnemyBrain enemyBrain;
    private AgentMovement agentMovement;

    private void Start()
    {
        Health = EnemyData.MaxHealth;
        Damage = EnemyData.Damage;
        Range = EnemyData.Range;
        agentRenderer = GetComponentInChildren<AgentRenderer>();
        enemyBrain = GetComponent<EnemyBrain>();
        agentMovement = GetComponent<AgentMovement>();
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        Health -= damage;
        if (Health >= 0)
            OnGetHit?.Invoke();
        else
        {
            OnDie?.Invoke();
            StartCoroutine(WaitToDie());
        }
    }

    IEnumerator WaitToDie(){
        isDying = true;
        DeadOrAlive();
        yield return new WaitForSeconds(5f);
        if (isDying == true)
        {
            // Call function that adds health to player if player passsive active
            Debug.Log("Before Epi Boost");
            PlayerSignaler.CallPlayerEpiBoost();
            Destroy(gameObject);
        }
    }

    public void DeadOrAlive()
    {
        if(isDying == true)
        {
            gameObject.layer = 0;
            enemyBrain.Move(Vector2.zero);
            agentMovement.currentVelocity = 0;
            enemyBrain.enabled = false;
            agentRenderer.isDying = true;
        }
        else
        {
            gameObject.layer = 8;
            enemyBrain.enabled = true;
            agentRenderer.isDying = false;
        }
    }

}
