using System;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [SerializeField] private PlayerID defendingPlayer; // Dueño de este arco: el punto va para el rival

    public event Action<PlayerID> OnGoalConceded;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Ball>(out _))
            OnGoalConceded?.Invoke(defendingPlayer);
    }
}