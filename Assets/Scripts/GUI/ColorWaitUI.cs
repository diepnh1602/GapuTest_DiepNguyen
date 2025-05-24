using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorWaitUI : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI txtAmount;

    public void SetData(ColorWait colorWait)
    {
        var c = ConfigManager.Instance.blockColorConfigs.Find(x => x.id == colorWait.id);
        img.color = c.color;
        txtAmount.text = colorWait.amount.ToString();
    }
}
