using System;

using UnityEngine;

using UnityUtility.CustomAttributes;

[Serializable]
public class PuppetAnimatorParameterContainer
{
    public int IsWalking => m_isWalkingParameterHash;
    public int HoldsObject => m_holdsObjectParameterHash;
    public int LookUp => m_lookUpParameterHash;
    public int Strike => m_strikeParameterHash;
    public int Victory => m_victoryParameterHash;
    public int Defeat => m_defeatParameterHash;

    [Title("Animator Parameters", bold:false)]
    [SerializeField] private string m_isWalkingParameterName = "IsWalking";
    [SerializeField] private string m_holdsObjectParameterName = "HoldsObject";
    [SerializeField] private string m_lookUpParameterName = "LookUp";
    [SerializeField] private string m_strikeParameterName = "Strike";
    [SerializeField] private string m_victoryParameterName = "Victory";
    [SerializeField] private string m_defeatParameterName = "Defeat";

    [NonSerialized] private int m_isWalkingParameterHash = -1;
    [NonSerialized] private int m_holdsObjectParameterHash = -1;
    [NonSerialized] private int m_lookUpParameterHash = -1;
    [NonSerialized] private int m_strikeParameterHash = -1;
    [NonSerialized] private int m_victoryParameterHash = -1;
    [NonSerialized] private int m_defeatParameterHash = -1;

    public void Init()
    {
        m_isWalkingParameterHash = Animator.StringToHash(m_isWalkingParameterName);
        m_holdsObjectParameterHash = Animator.StringToHash(m_holdsObjectParameterName);
        m_lookUpParameterHash = Animator.StringToHash(m_lookUpParameterName);
        m_strikeParameterHash = Animator.StringToHash(m_strikeParameterName);
        m_victoryParameterHash = Animator.StringToHash(m_victoryParameterName);
        m_defeatParameterHash = Animator.StringToHash(m_defeatParameterName);
    }
}
