using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


//간단 싱글톤
public class DamagePopupManager : MonoBehaviour
{
    private static DamagePopupManager _instance;
    public static DamagePopupManager Instance
    {
        get
        {
            return _instance;
        }
    }
    [SerializeField] private DamagePrefab _damagePrefab;
    [SerializeField] private AnimationCurve _damageAnimationCurve;
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private float _endValue;
    [SerializeField] private float _duration;
    private int _capacity = 10;
    private int _poolMaxSize = 1000;
    private IObjectPool<DamagePrefab> _pool;

    private void Awake()
    {
        if(_instance != null) 
            Destroy(_instance);
        _instance = this;

        _pool = new ObjectPool<DamagePrefab>(
            CreateDamagePrefab,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            _capacity,
            _poolMaxSize
            );

    }


    public void Popup(Vector3 position, float damage)
    {
        var prefab = _pool.Get();

        prefab.SetText(Mathf.FloorToInt(damage).ToString());

        float rndX = UnityEngine.Random.Range(-_startPosition.x, _startPosition.x);
        float rndY = UnityEngine.Random.Range(-_startPosition.y, _startPosition.y);

        prefab.transform.position = Camera.main.WorldToScreenPoint(position) + new Vector3(rndX,rndY);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(prefab.transform.DOLocalMoveY(_endValue, _duration)
            .SetRelative()
            .SetEase(_damageAnimationCurve));
            //.SetEase(Ease.OutElastic));

        sequence.Join(prefab.TMP.DOFade(0, 1f));

        sequence.OnComplete(() =>
        {
            prefab.TMP.alpha = 1f;
            _pool.Release(prefab);
        });
    }

    private DamagePrefab CreateDamagePrefab()
    {
        var prefab = Instantiate(_damagePrefab,transform);
        if (prefab == null)
            return null;
        prefab.gameObject.SetActive(false);
        return prefab;
    }

    private void OnTakeFromPool(DamagePrefab damagePrefab)
    {
        damagePrefab.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(DamagePrefab damagePrefab)
    {
        damagePrefab.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(DamagePrefab damagePrefab) 
    {
        Destroy(damagePrefab.gameObject);
    }

}
