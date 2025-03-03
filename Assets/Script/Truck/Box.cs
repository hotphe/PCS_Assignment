using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Entity
{
    private Truck _truck;

    public void SetTruck(Truck truck)
    {
        _truck = truck;
    }

    public override UniTask Death()
    {
        gameObject.SetActive(false);
        _truck?.DestroyBox(this);
        return UniTask.CompletedTask;
    }

}
