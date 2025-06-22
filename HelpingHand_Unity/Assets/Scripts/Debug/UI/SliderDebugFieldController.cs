using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SliderDebugFieldController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_label;
    [SerializeField] private Slider m_slider;

    public void Init(string labelName)
    {
        m_label.text = labelName;
    }

    public void Init(string labelName, float startValue)
    {
        Init(labelName);
        m_slider.value = startValue;

    }

    public float GetValue()
    {
        return m_slider.value;
    }
}
