using System;
using System.Linq;

using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VirtualToggleGroup : MonoBehaviour
{
    [SerializeField] private bool m_allowSwitchOff = false;

    [NonSerialized] private VirtualToggle[] m_toggles;
    [NonSerialized] private Action<bool>[] m_togglesSubscribtions;

    private void Start()
    {
        m_toggles = GetComponentsInChildren<VirtualToggle>();
        m_togglesSubscribtions = new Action<bool>[m_toggles.Length];

        InitalizeTogglesState();

        for (int iToggle = 0; iToggle < m_toggles.Length; ++iToggle)
        {
            int toggleIndex = iToggle;
            Action<bool> toggleSubscription = (bool state) =>
            {
                OnTogglePressed(toggleIndex, state);
            };

            m_toggles[iToggle].OnValueChanged += toggleSubscription;
            m_togglesSubscribtions[iToggle] = toggleSubscription;
        }
    }

    private void OnDestroy()
    {
        for (int iToggle = 0; iToggle < m_toggles.Length; ++iToggle)
        {
            m_toggles[iToggle].OnValueChanged -= m_togglesSubscribtions[iToggle];
        }
    }

    private void InitalizeTogglesState()
    {
        if (m_toggles.Length == 0)
        {
            return;
        }

        int enabledToggleCount = m_toggles.Count(toggle => toggle.Value);

        switch (enabledToggleCount)
        {
            case 0:
                if (m_allowSwitchOff)
                {
                    break;
                }
                m_toggles[0].SetToggleValue(true);
                break;

            case 1:
                break;

            case > 1:
                bool oneToggleEnabled = false;
                for (int i = 0; i < m_toggles.Length; ++i)
                {
                    VirtualToggle toggle = m_toggles[i];
                    if (!toggle.Value)
                    {
                        continue;
                    }

                    if (oneToggleEnabled)
                    {
                        toggle.SetToggleValue(false);
                        continue;
                    }

                    oneToggleEnabled = true;
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();

        }
    }

    private void OnTogglePressed(int toggleIndex, bool newValue)
    {
        if (!newValue)
        {
            if (m_allowSwitchOff)
            {
                return;
            }

            m_toggles[toggleIndex].SetToggleValue(true);
            return;
        }

        for (int iToggle = 0; iToggle < m_toggles.Length; ++iToggle)
        {
            if (iToggle == toggleIndex)
            {
                continue;
            }

            VirtualToggle toggle = m_toggles[iToggle];
            Action<bool> toggleSubscription = m_togglesSubscribtions[iToggle];

            toggle.OnValueChanged -= toggleSubscription;
            toggle.SetToggleValue(false);
            toggle.OnValueChanged += toggleSubscription;
        }
    }
}
