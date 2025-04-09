using System.Threading;

using UnityEngine;

public class GameLagger : MonoBehaviour
{
    [SerializeField] private int m_targetFramerate = 60;

    private void Update()
    {
        Thread.Sleep((int)((1.0f / m_targetFramerate) * 1000));
    }
}
