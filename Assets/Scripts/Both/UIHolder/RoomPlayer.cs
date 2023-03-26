using Assets.Scripts.Both.Scriptable;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : MonoBehaviour
{
    public GameObject UIContainer;
    public GameObject ButtonContainer;
    public bool OnUse;
    public bool IsOwner;

    public TMP_Text CharacterLabel;
    public Image CharacterImage;
    public TMP_Text PlayerName;

    public int CharacterIndex;
    public Button LeftButton;
    public Button RightButton;

    public Color OwnerColor = new Color(154 / 255f, 202 / 255f, 138 / 255f, 208 / 255f);

    private int CharacterCount;

    public void Next()
    {
        CharacterIndex++;
        if (CharacterIndex > CharacterCount - 1)
        {
            CharacterIndex = 0;
        }

        UpdateCharacterRpc();
    }

    public void Previous()
    {
        CharacterIndex--;
        if (CharacterIndex < 0)
        {
            CharacterIndex = CharacterCount - 1;
        }

        UpdateCharacterRpc();
    }

    private void UpdateCharacterRpc()
    {
        UpdateCharacter();

        RoomController.Instance.ChangedCharacterIndexServerRpc(CharacterIndex);
    }

    public void UpdatePlayerNameRpc(string name) //For input field UI
    {
        UpdatePlayerName(name);

        //Call Rpc
    }

    public void UpdateCharacter()
    {
        CharacterIndex = Math.Clamp(CharacterIndex, 0, CharacterCount);

        var name = Enum.GetName(typeof(CharacterClass), CharacterIndex);
        name = name.Replace("_model", "");

        CharacterImage.sprite = Resources.Load<Sprite>("Player/" + name);
        CharacterLabel.text = name;
    }

    public void UpdatePlayerName(string name)
    {
        //Check string

        PlayerName.text = name;
    }

    public void Initial()
    {
        CharacterCount = Enum.GetNames(typeof(CharacterClass)).Length;
        CharacterIndex = 0;

        UpdateCharacter();
    }
}
