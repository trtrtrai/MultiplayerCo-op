using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthColorManage : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;

    [SerializeField] private List<float> healthRatio;
    [SerializeField] private List<Color> healthColor;

    public void SliderValueChange(float num)
    {
        if (slider is null || fill is null) return;

        for (int i = 0; i < healthRatio.Count; i++)
        {
            //Debug.Log(healthRatio[i] + " " + num);
            if (healthRatio[i] >= num)
            {
                fill.color = healthColor[i];
                return;
            }
        }
    }
}
