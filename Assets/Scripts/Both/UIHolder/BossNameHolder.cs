using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossNameHolder : MonoBehaviour
{
    public List<Button> ListBossBtn;
    public Color Selected;
    public Color Unselected;
    public Button Current;

    private void Start()
    {
        Select(ListBossBtn[0]);
    }

    public void Select(Button btn)
    {
        if (ReferenceEquals(btn, Current)) return;

        if (Current != null)
        {
            Current.GetComponent<Image>().color = Unselected;
        }

        Current = btn;
        Current.GetComponent<Image>().color = Selected;

        RoomController.Instance.BossChoiceServerRpc(ListBossBtn.IndexOf(Current));
    }
}
