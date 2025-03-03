using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Hero : Entity
{
    private Truck _truck;
    public event Action OnAttack;

    private void Awake()
    {
        _truck = GetComponentInParent<Truck>();
    }

    private void Update()
    {
        OnAttack?.Invoke();
    }

    public override UniTask Death()
    {
        gameObject.SetActive(false);
        _truck?.EndGame();
        return UniTask.CompletedTask;
    }

}
