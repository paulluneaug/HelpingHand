using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public SlidersManager SlidersManager => m_sliderManager;
    public ActSequenceManager ActSequenceManager => m_actSequenceManager;


    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Label(bold: true)] private SlidersManager m_sliderManager;
    [Separator]
    [SerializeField, Label(bold: true)] private ActSequenceManager m_actSequenceManager;

    // Cache
    [NonSerialized] private Puppet m_puppet;


    public override void Initialize()
    {
        base.Initialize();
        m_actSequenceManager.Initialize(m_puppet);
    }

    protected override void Start()
    {
        base.Start();
        m_actSequenceManager.Start();
    }

    private void Update()
    {
         m_actSequenceManager.Update(Time.deltaTime);
    }

    #region Puppet
    public Puppet GetPuppet()
    {
        if (m_puppet == null)
        {
            Debug.LogError($"No puppet registered : Call {nameof(RegisterPuppet)}");
            return null;
        }
        return m_puppet;
    }

    public void RegisterPuppet(Puppet puppet)
    {
        if (m_puppet != null)
        {
            Debug.LogError("A puppet was already registered");
            return;
        }
        m_puppet = puppet;
    }

    public void UnregisterPuppet()
    {
        m_puppet = null;
    }

    #endregion

}
