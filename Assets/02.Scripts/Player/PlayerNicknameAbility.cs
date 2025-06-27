using TMPro;
using UnityEngine;

public class PlayerNicknameAbility : PlayerAbility
{
    public TextMeshPro NickNameText;

    private void Start()
    {
        NickNameText.text = $"{_photonView.Owner.NickName}_{_photonView.Owner.ActorNumber}";

        if(_photonView.IsMine)
        {
            NickNameText.color = Color.blue;
        }
        else
        {
            NickNameText.color = Color.red;
        }
    }
}
