using Assets.Scripts.Both.Creature.Status;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthColorManage : MonoBehaviour
{
    public Image Fill;

    [SerializeField] private List<float> healthRatio;
    [SerializeField] private List<Color> healthColor;

    public void SliderValueChange()
    {
        if (Fill is null) return;

        for (int i = 0; i < healthRatio.Count; i++)
        {
            //Debug.Log(healthRatio[i] + " " + num);
            if (healthRatio[i] >= Fill.fillAmount)
            {
                /*var cb = slider.colors;
                cb.normalColor = healthColor[i];
                slider.colors = cb;*/

                Fill.color = healthColor[i];
                return;
            }
        }
    }
}
