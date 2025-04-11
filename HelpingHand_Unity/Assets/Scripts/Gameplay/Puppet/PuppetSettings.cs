using UnityEngine;

using UnityUtility.CustomAttributes;

[CreateAssetMenu(fileName = "PuppetSettings", menuName = "Scriptable Objects/PuppetSettings")]
public class PuppetSettings : ScriptableObject
{
    public float TileSize => m_tileSize;
    public float ActionDuration => m_puppetActionDuration;

    /// <summary>
    /// Unit : Unity unit per second
    /// </summary>
    public float MovementSpeed => m_tileSize / m_puppetActionDuration; // A movement action moved the puppet by a tile

    /// <summary>
    /// Unit : Turn per second
    /// </summary>
    public float RotationSpeed => m_puppetActionDuration / 4.0f; // A rotation action rotates the puppet by a quarter turn


    [SerializeField] private float m_tileSize = 1.0f;

    [HelpBox("In seconds")]
    [SerializeField] private float m_puppetActionDuration = 1.0f;
}
