using UnityEngine;

public class AnimationGears : MonoBehaviour
{
    public GameObject Gear1;
    public GameObject Gear2;
    public GameObject Gear3;
    public GameObject Curtain;

    public void GearRotate_Sound()
    {
        AudioManager.Instance.EventManager.MainGear_Play.Post(Gear1);
        AudioManager.Instance.EventManager.MainGear_Play.Post(Gear2);
        AudioManager.Instance.EventManager.MainGear_Play.Post(Gear3);
        Debug.Log("MainGear_Play playing");
    }
}
