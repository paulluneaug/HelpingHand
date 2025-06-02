using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu]
public class AudioEvent : ScriptableObject
{
    [SerializeField]
    [LabelWidth(75)]
    private AK.Wwise.Event m_wwiseEvent;

    [SerializeField]
    [LabelWidth(75)]
    private RepetitionState m_repetition;

    [SerializeField]
    [HorizontalGroup("Object", MarginRight = 5)]
    [LabelWidth(75)]
    private ObjetState m_objet;

    [SerializeField]
    [HorizontalGroup("Object", Width = 60)]
    [HideLabel]
    private EtatState m_etat;

    [SerializeField]
    [LabelWidth(75)]
    private NarraState m_narra;

    public async UniTask Play(GameObject target = null, CancellationToken cancellationToken = default)
    {
        if (m_wwiseEvent != null && m_wwiseEvent.IsValid())
        {
            await AudioManager.Instance.PostWwiseEventAsync(m_wwiseEvent, target, cancellationToken);
        }
        else
        {
            await AudioManager.Instance.PlayDialogueWithStatesAsync(m_repetition.ToString(), m_etat.ToString(), m_objet.ToString(), m_narra.ToString(), target, cancellationToken);
        }
    }
}
