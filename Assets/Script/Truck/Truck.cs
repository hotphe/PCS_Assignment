using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private GameObject _jumpBlocker;
    [SerializeField] private Transform _boxContainer;
    [Tooltip("상자가 파괴되었을 때 각 상자들의 위치를 알맞게 이동시키는데 걸리는 시간")]
    [SerializeField] private float _repositionDuration = 0.1f;

    private Hero _hero;
    private List<Box> _boxes = new List<Box>();
    private List<Box> _aliveBoxes = new List<Box>();
    private List<IResetable> _resetables = new List<IResetable>();

    private void Awake()
    {
        _hero = GetComponentInChildren<Hero>();
        AddBox(GetComponentsInChildren<Box>());
        ResetState();
    }

    /// <summary>
    /// 히어로 사망 시 처리
    /// </summary>
    public void EndGame()
    {
        _jumpBlocker.SetActive(false);
    }

    public void AddBox(params Box[] boxes)
    {
        if (boxes.Length == 0)
            return;
        _boxes.AddRange(boxes);
        _aliveBoxes.AddRange(boxes);
        foreach (Box box in _boxes)
        {
            box.SetTruck(this);
            if (box.TryGetComponent<IResetable>(out var comp))
                _resetables.Add(comp);
        }
        Reposition();
    }
    public void RemoveBox(Box box)
    {
        _boxes.Remove(box);
        _aliveBoxes.Remove(box);
        if(box.TryGetComponent<IResetable>(out var comp))
            _resetables.Remove(comp);
        Reposition();
    }

    public void DestroyBox(Box box)
    {
        _aliveBoxes.Remove(box);
        Reposition();
    }

    public void Reposition()
    {
        float yPos = 0;

        foreach(var box in _aliveBoxes)
        {
            float height = box.Height;
            box.transform.DOLocalMoveY(yPos + height / 2.0f, _repositionDuration);
            yPos += height;
        }
        _hero.transform.DOLocalMoveY(yPos + _hero.Height / 2.0f, _repositionDuration);
    }
   
    public void ResetState()
    {
        _aliveBoxes.Clear();
        _resetables.ForEach(x => x.ResetData());
        _boxes.ForEach(x=>x.gameObject.SetActive(true));
        _aliveBoxes.AddRange(_boxes);
        
        Reposition();
    }



}