using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class UI_Player : MonoBehaviour
{
    public Image _staminaBar;
    private Player _player;

    private void Start()
    {
        StartCoroutine(WaitForPlayerAndInitialize());
    }


    private IEnumerator WaitForPlayerAndInitialize()
    {
        while (_player == null)
        {
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (Player player in players)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    _player = player;
                    break;
                }
            }
            
            if (_player == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        
        if (_player != null)
        {
            _player.GetAbility<PlayerStamina>()._onStaminaChanged += UpdateStamina;
            UpdateStamina();
        }
    }

    private void UpdateStamina()
    {
        if (_player != null && _player.Stat != null)
        {
            _staminaBar.fillAmount = _player.Stat.Stamina / _player.Stat.MaxStamina;
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.GetAbility<PlayerStamina>()._onStaminaChanged -= UpdateStamina;
        }
    }
}
