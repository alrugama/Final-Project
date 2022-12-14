using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBrain : MonoBehaviour, IAgentInput
{
    [field: SerializeField]
    public GameObject Target { get; set; }

    [field: SerializeField]
    public EnemyGun Weapon { get; set; }

    public Enemy enemy;

    [field: SerializeField]
    public AIState CurrentState { get; set; }

    [field: SerializeField]
    public UnityEvent OnFireButtonPressed { get; set; }

    [field: SerializeField]
    public UnityEvent OnFireButtonReleased { get; set; }

    [field: SerializeField]
    public UnityEvent<Vector3> OnMovementKeyPressed { get; set; }

    [field: SerializeField]
    public UnityEvent<Vector3> OnPointerPositionChange { get; set; }

    [field: SerializeField]
    public UnityEvent OnReloadButtonPressed { get; set; }

    private void Update()
    {
        if (Target == null)
        {
            OnMovementKeyPressed?.Invoke(Vector3.zero);
        }
        CurrentState.UpdateState();
    }

    public void Attack()
    {
        OnFireButtonPressed?.Invoke();
    }

    public void StopAttack()
    {
        OnFireButtonReleased?.Invoke();
    }

    public void Move(Vector3 movementDirection)
    {
        //Debug.Log("Movement Direction: " + movementDirection);
        OnMovementKeyPressed?.Invoke(movementDirection);
    }

    public void Aim(Vector3 targetPosition)
    {   
        OnPointerPositionChange?.Invoke(targetPosition);
    }

    private void Awake()
    {
        Target = FindObjectOfType<Player>().gameObject;
        Weapon = transform.GetComponentInChildren<EnemyGun>();
        enemy = transform.GetComponent<Enemy>();
    }

    private void Start()
    {
        // Target = FindObjectOfType<Player>().gameObject;
        enemy = transform.GetComponent<Enemy>();
    }

    internal void ChangetoState(AIState State)
    {
        CurrentState = State;
    }
}
