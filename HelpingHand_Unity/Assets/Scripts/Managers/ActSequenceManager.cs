using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityUtility.CustomAttributes;
using UnityUtility.SceneReference;

[Serializable]
public class ActSequenceManager
{
    private enum ActSequenceManagerState
    {
        NotStarted,
        Running,
        WaitingForSceneLoading,
        Finished,
    }

    private enum ActSequenceManagerMode
    {
        Single,
        Sequence,
    }

    public ActManager CurrentAct => m_currentAct;


    [Title("Acts")]
    [SerializeField] private ActSequenceManagerMode m_mode;
    [SerializeField] private SceneReference[] m_actsScenes;


    // Cache
    [NonSerialized] private ActSequenceManagerState m_currentState;

    [NonSerialized] private int m_nextSceneToLoadIndex;

    [NonSerialized] private int m_nextActIndex;
    [NonSerialized] private ActManager m_currentAct;
    [NonSerialized] private ActManager[] m_actSequence;

    public void Initialize()
    {
        if (m_mode == ActSequenceManagerMode.Single)
        {
            return;
        }

        m_actSequence = new ActManager[m_actsScenes.Length];

        m_nextSceneToLoadIndex = 0;

        m_currentState = ActSequenceManagerState.NotStarted;
    }

    public void StartSequence()
    {
        if (m_mode == ActSequenceManagerMode.Single)
        {
            return;
        }

        // Loads the first scene
        LoadNextScene();

    }

    public void StopSequence()
    {
        if (m_currentAct != null)
        {
            FinishCurrentAct();
        }
        Initialize();
        m_currentState = ActSequenceManagerState.NotStarted;
    }

    public void UpdateSequence(float deltaTime)
    {
        if (m_mode == ActSequenceManagerMode.Single)
        {
            return;
        }

        if (m_currentState == ActSequenceManagerState.Running && m_currentAct.IsFinished)
        {
            FinishCurrentAct();
            LoadNextScene();
            return;
        }
    }

    public void RegisterAct(ActManager act)
    {
        if (m_mode == ActSequenceManagerMode.Single)
        {
            m_currentAct = act;
            act.StartAct(GameManager.Instance.GetPuppet());
            return;
        }

        int levelIndex = m_actsScenes.FindIndex((scene => scene.ScenePath.Equals(act.gameObject.scene.path)));
        m_actSequence[levelIndex] = act;

        if (m_currentState == ActSequenceManagerState.WaitingForSceneLoading)
        {
            StartNextAct();
        }
    }

    private void FinishCurrentAct()
    {
        m_currentAct.Dispose();
        _ = SceneManager.UnloadSceneAsync(m_currentAct.gameObject.scene);
        m_currentAct = null;
    }

    private void StartNextAct()
    {
        if (m_nextActIndex >= m_actSequence.Length)
        {
            return;
        }
        m_currentAct = m_actSequence[m_nextActIndex++];
        m_currentState = ActSequenceManagerState.Running;

        m_currentAct.StartAct(GameManager.Instance.GetPuppet());
    }

    private void LoadNextScene()
    {
        if (m_nextSceneToLoadIndex >= m_actsScenes.Length)
        {
            EndActSequence();
            return;
        }

        m_currentState = ActSequenceManagerState.WaitingForSceneLoading;

        SceneReference sceneToLoad = m_actsScenes[m_nextSceneToLoadIndex++];

        _ = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }

    private void EndActSequence()
    {
        m_currentState = ActSequenceManagerState.Finished;
    }
}

public static class ArrayExtension
{
    public static int FindIndex<T>(this T[] array, Predicate<T> match)
    {
        for (int iElement = 0; iElement < array.Length; iElement++)
        {
            if (match(array[iElement]))
            {
                return iElement;
            }
        }
        return -1;
    }
}
