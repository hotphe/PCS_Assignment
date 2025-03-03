using Cysharp.Threading.Tasks;
using PCS.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeChangeListener : MonoBehaviour
{
    [SerializeField] private UIAdjuster _uiAdjuster;
    private void OnRectTransformDimensionsChange()
    {
        UniTask.Create(async () =>
        {
            await UniTask.DelayFrame(2);
            _uiAdjuster.Apply(new Vector2(Screen.width, Screen.height));
        }).Forget();
    }
}
