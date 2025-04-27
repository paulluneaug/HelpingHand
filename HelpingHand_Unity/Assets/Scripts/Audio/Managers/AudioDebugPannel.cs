using Sirenix.OdinInspector;

using UnityEditor;

using UnityEngine;

[ExecuteInEditMode]
public class AudioDebugPanel : MonoBehaviour
{
    [BoxGroup("References", centerLabel: true)]
    [SerializeField] private AudioManager m_audioManager;

    private bool m_showStates = true;
    private bool m_showSwitches = true;
    private bool m_showRTPCs = true;

    private void OnGUI()
    {
        if (m_audioManager == null)
        {
            EditorGUILayout.HelpBox("Assign AudioManager reference.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);

        // ================= STATES =================
        m_showStates = EditorGUILayout.Foldout(m_showStates, "Game States");
        if (m_showStates)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (m_audioManager.StateManager != null)
            {
                EditorGUILayout.LabelField("Current GameState:", m_audioManager.StateManager.CurrentGameState.ToString(), EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Current MusicState:", m_audioManager.StateManager.CurrentMusicState.ToString(), EditorStyles.boldLabel);

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Set GameState:", EditorStyles.boldLabel);
                foreach (GameState state in System.Enum.GetValues(typeof(GameState)))
                {
                    if (GUILayout.Button($"Set {state}"))
                    {
                        m_audioManager.StateManager.SetGameState(state);
                    }
                }

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Set MusicState:", EditorStyles.boldLabel);
                foreach (MusicState musicState in System.Enum.GetValues(typeof(MusicState)))
                {
                    if (GUILayout.Button($"Set {musicState}"))
                    {
                        m_audioManager.StateManager.SetMusicState(musicState);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No StateManager assigned.");
            }

            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        // ================= SWITCHES =================
        m_showSwitches = EditorGUILayout.Foldout(m_showSwitches, "Switches");
        if (m_showSwitches)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (m_audioManager.SwitchManager != null)
            {
                EditorGUILayout.LabelField("Switch Settings", EditorStyles.boldLabel);

                GUILayout.Space(5);

                EditorGUILayout.LabelField("Set Locomotion Switch:");
                foreach (SwitchManager.LocomotionType locomotionType in System.Enum.GetValues(typeof(SwitchManager.LocomotionType)))
                {
                    if (GUILayout.Button($"Set {locomotionType} Locomotion"))
                    {
                        m_audioManager.SwitchManager.SetLocomotionSwitch(locomotionType);
                    }
                }

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Set Material Switch:");
                foreach (SwitchManager.MaterialType materialType in System.Enum.GetValues(typeof(SwitchManager.MaterialType)))
                {
                    if (GUILayout.Button($"Set {materialType} Material"))
                    {
                        m_audioManager.SwitchManager.SetMaterialSwitch(materialType);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No SwitchManager assigned.");
            }

            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        // ================= RTPC =================
        m_showRTPCs = EditorGUILayout.Foldout(m_showRTPCs, "RTPCs");
        if (m_showRTPCs)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (m_audioManager.RTPCManager != null)
            {
                //audioManager.RTPCManager.DrawDebugGUI();
            }
            else
            {
                EditorGUILayout.LabelField("No RTPC Manager assigned.");
            }

            GUILayout.EndVertical();
        }
    }
}
