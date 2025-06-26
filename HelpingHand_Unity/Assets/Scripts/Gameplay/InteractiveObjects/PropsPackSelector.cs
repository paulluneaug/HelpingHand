using System;

using UnityEngine;

using UnityUtility.CustomAttributes;

public class PropsPackSelector : MonoBehaviour
{
    private enum SelectedPack
    {
        None,
        Town,
        Tavern,
        Armory,
        Market,
        Lair,
    }

    [Title("Variable references")]
    [SerializeField] private BaseVariable<bool> m_active;
    [SerializeField] private BaseVariable<bool> m_forceTownProps;
    [SerializeField] private BaseVariable<bool> m_forceLairProps;
    [SerializeField] private BaseVariable<float> m_selectedPackFader;

    [Separator]
    [SerializeField] private EntityState m_townPropsSelected;
    [SerializeField] private EntityState m_tavernPropsSelected;
    [SerializeField] private EntityState m_armoryPropsSelected;
    [SerializeField] private EntityState m_marketPropsSelected;
    [SerializeField] private EntityState m_lairPropsSelected;

    // Cache
    [NonSerialized] private SelectedPack m_selectedPack;


    private void Awake()
    {
        SetSelectedPack(SelectedPack.None);

        m_active.AddListener(OnIsActiveChanged);
        OnIsActiveChanged(m_active.Value);
    }

    private void OnEnable()
    {
        m_forceTownProps.AddListener(OnBoolVariableChanged);
        m_forceLairProps.AddListener(OnBoolVariableChanged);
    }

    private void OnDisable()
    {
        m_forceTownProps.RemoveListener(OnBoolVariableChanged);
        m_forceLairProps.RemoveListener(OnBoolVariableChanged);
    }

    private void OnIsActiveChanged(bool active)
    {
        UpdateSelectedPack();

        if (!active)
        {
            m_selectedPackFader.RemoveListener(OnFaderValueChanged);
            return;
        }
        m_selectedPackFader.AddListener(OnFaderValueChanged);
    }

    private void OnDestroy()
    {
        m_active.RemoveListener(OnIsActiveChanged);
        OnIsActiveChanged(false);
    }

    private void OnBoolVariableChanged(bool newValue)
    {
        UpdateSelectedPack();
    }

    private void OnFaderValueChanged(float newValue)
    {
        UpdateSelectedPack();
    }

    private void UpdateSelectedPack()
    {
        // if (!m_active.Value)
        // {
        //     SetSelectedPack(SelectedPack.None);
        //     return;
        // }

        if (m_forceLairProps.Value)
        {
            SetSelectedPack(SelectedPack.Lair);
            return;
        }

        if (m_forceTownProps.Value)
        {
            SetSelectedPack(SelectedPack.Town);
            return;
        }

        SelectedPack selectedPack = GetSelectedPackFromFaderValue(m_selectedPackFader.Value);
        if (m_selectedPack != selectedPack)
        {
            SetSelectedPack(selectedPack);
            return;
        }
    }

    private void SetSelectedPack(SelectedPack selectedPack)
    {
        m_selectedPack = selectedPack;
        UpdateEntityStates();
    }

    private void UpdateEntityStates()
    {
        m_townPropsSelected.Value = m_selectedPack == SelectedPack.Town;
        m_tavernPropsSelected.Value = m_selectedPack == SelectedPack.Tavern;
        m_armoryPropsSelected.Value = m_selectedPack == SelectedPack.Armory;
        m_marketPropsSelected.Value = m_selectedPack == SelectedPack.Market;
        m_lairPropsSelected.Value = m_selectedPack == SelectedPack.Lair;
    }

    private SelectedPack GetSelectedPackFromFaderValue(float value)
    {
        return value switch
        {
            < 0.1f => SelectedPack.None, 
            <= 0.3333f => SelectedPack.Tavern,
            <= 0.6666f => SelectedPack.Armory,
            > 0.6666f => SelectedPack.Market,
            _ => throw new NotImplementedException(),
        };
    }
}
