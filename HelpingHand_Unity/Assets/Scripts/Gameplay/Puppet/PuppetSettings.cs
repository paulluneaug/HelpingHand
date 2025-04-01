using UnityEngine;

using UnityUtility.CustomAttributes;

[CreateAssetMenu(fileName = "PuppetSettings", menuName = "Scriptable Objects/PuppetSettings")]
public class PuppetSettings : ScriptableObject
{
    public float TileSize => m_tileSize;

    /// <summary>
    /// Unit : Tile per second
    /// </summary>
    public float PuppetSpeed => m_puppetSpeed;

    /// <summary>
    /// Unit : Unity unit per second
    /// </summary>
    public float PuppetAbsoluteSpeed => m_puppetSpeed * m_tileSize;

    /// <summary>
    /// Unit : Turns per second
    /// </summary>
    public float PuppetRotationSpeed => m_puppetRotationSpeed;


    [SerializeField] private float m_tileSize = 1.0f;

    [HelpBox("Speed in tiles per seconds")]
    [SerializeField] private float m_puppetSpeed = 1.0f;
    [HelpBox("Speed in turns per seconds")]
    [SerializeField] private float m_puppetRotationSpeed = 3.0f;
}
