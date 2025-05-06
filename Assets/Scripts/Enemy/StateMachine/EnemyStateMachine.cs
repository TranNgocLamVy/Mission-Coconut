using Photon.Chat;
using Photon.Pun;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemyStateMachine
{
    
    public EnemyState currentState { get; set; }

    public void Initialize(EnemyState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
