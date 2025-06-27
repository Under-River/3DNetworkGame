using UnityEngine;

public class PlayerMinimap : PlayerAbility
{
    public GameObject[] MinimapCamera;
    public SpriteRenderer Icon;

    private void Start()
    {
        if(_photonView.IsMine)
        {
            foreach(GameObject camera in MinimapCamera)
            {
                camera.SetActive(true);
            }
            Icon.color = Color.blue;
        }
        else
        {
            foreach(GameObject camera in MinimapCamera)
            {
                camera.SetActive(false);
            }
            Icon.color = Color.red;
        }
    }
}
