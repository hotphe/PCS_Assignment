using TMPro;
using UnityEngine;

public class DamagePrefab : MonoBehaviour
{
    public TextMeshProUGUI TMP{ get; private set; }
    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
    }
    public void SetText(string text)
    {
        TMP.SetText(text);
    }
}
