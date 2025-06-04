using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainCameraSetter : MonoBehaviour
{
    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
