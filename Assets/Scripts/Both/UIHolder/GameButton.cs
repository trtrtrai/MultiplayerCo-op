using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    public void OutGamePlay()
    {
        GameController.Instance.ToRoomScene();
    }
}
