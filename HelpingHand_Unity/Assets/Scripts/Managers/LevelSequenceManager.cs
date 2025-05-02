using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityUtility.CustomAttributes;
using UnityUtility.SceneReference;

[Serializable]
public class LevelSequenceManager
{
    private enum LevelSequenceManagerState
    {
        NotStarted,
        Running,
        WaitingForSceneLoading,
        Finished,
    }

    private enum LevelSequenceManagerMode
    {
        Single,
        Sequence,
    }


    [Title("Levels")]
    [SerializeField] private LevelSequenceManagerMode m_mode;
    [SerializeField] private SceneReference[] m_levelScenes;


    // Cache
    [NonSerialized] private LevelSequenceManagerState m_currentState;

    [NonSerialized] private int m_nextSceneToLoadIndex;

    [NonSerialized] private int m_nextLevelIndex;
    [NonSerialized] private LevelManager m_currentLevel;
    [NonSerialized] private LevelManager[] m_levelSequence;

    [NonSerialized] private int m_levelsCompletedCount;

    [NonSerialized] private int m_loadedScenesCount;
    [NonSerialized] private int m_loadingScenesCount;

    [NonSerialized] private Puppet m_puppet;

    public void Initialize(Puppet puppet)
    {
        m_puppet = puppet;
        if (m_mode == LevelSequenceManagerMode.Single)
        {
            return;
        }

        m_levelSequence = new LevelManager[m_levelScenes.Length];

        m_nextSceneToLoadIndex = 0;
        m_levelsCompletedCount = 0;
        m_loadedScenesCount = 0;
        m_loadingScenesCount = 0;

        m_currentState = LevelSequenceManagerState.NotStarted;
    }

    public void Start()
    {
        if (m_mode == LevelSequenceManagerMode.Single)
        {
            return;
        }

        m_currentState = LevelSequenceManagerState.WaitingForSceneLoading;

        // Pre load the first scene
        LoadNextScene();

    }

    public void Update(float deltaTime)
    {
        if (m_mode == LevelSequenceManagerMode.Single)
        {
            return;
        }

        if (m_currentState == LevelSequenceManagerState.Running && m_currentLevel.IsFinished)
        {
            StartNextLevel();
            return;
        }
    }

    public void RegisterLevel(LevelManager level)
    {
        if (m_mode == LevelSequenceManagerMode.Single)
        {
            level.StartLevel(m_puppet);
            return;
        }

        int levelIndex = m_levelScenes.FindIndex((scene => scene.ScenePath.Equals(level.gameObject.scene.path)));
        m_levelSequence[levelIndex] = level;

        MoveLevels(level, levelIndex);

        if (m_currentState == LevelSequenceManagerState.WaitingForSceneLoading)
        {
            _ = TryStartNextLevel();
        }
    }

    private void MoveLevels(LevelManager registeredLevel, int levelIndex)
    {
        if (levelIndex == 0)
        {
            registeredLevel.MoveLevel(Vector3.zero);
            return;
        }

        LevelManager previousLevel = m_levelSequence[levelIndex - 1];
        if (previousLevel != null)
        {
            registeredLevel.MoveLevel(previousLevel.EndAnchor);
        }
        else
        {
            registeredLevel.MoveLevel(Vector3.down * 50);
            return;
        }

        for (int i = levelIndex + 1; i < m_levelSequence.Length; ++i)
        {
            LevelManager level = m_levelSequence[i];
            if (level == null)
            {
                break;
            }

            level.MoveLevel(m_levelSequence[i - 1].EndAnchor);
        }
    }

    private void StartNextLevel()
    {
        if (m_currentLevel != null)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(m_currentLevel.gameObject.scene);
            unloadOperation.completed += OnUnloadingOperationCompleted;
        }
        LoadNextScene();

        if (m_nextLevelIndex >= m_levelSequence.Length)
        {
            EndLevelSequence();
            return;
        }

        _ = TryStartNextLevel();
    }

    private bool TryStartNextLevel()
    {
        m_currentLevel = m_levelSequence[m_nextLevelIndex];
        if (m_currentLevel == null)
        {
            if (m_loadingScenesCount == 0)
            {
                Debug.LogError($"No scene loading but the {nameof(LevelSequenceManager)} is waiting for a scene to load");
                return false;
            }

            m_currentState = LevelSequenceManagerState.WaitingForSceneLoading;
            Debug.LogWarning("The scene was not loaded in time");
            return false;
        }

        m_nextLevelIndex++;
        m_currentState = LevelSequenceManagerState.Running;

        m_currentLevel.StartLevel(m_puppet);
        return true;
    }

    private void EndLevelSequence()
    {
        m_currentState = LevelSequenceManagerState.Finished;
    }

    private void LoadNextScene()
    {
        if (m_nextSceneToLoadIndex >= m_levelScenes.Length)
        {
            return;
        }

        SceneReference sceneToLoad = m_levelScenes[m_nextSceneToLoadIndex++];

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        loadOperation.completed += OnLoadingOperationCompleted;

        m_loadingScenesCount++;
    }

    private void OnLoadingOperationCompleted(AsyncOperation operation)
    {
        operation.completed -= OnLoadingOperationCompleted;

        m_loadingScenesCount--;
        m_loadedScenesCount++;
    }

    private void OnUnloadingOperationCompleted(AsyncOperation operation)
    {
        operation.completed -= OnUnloadingOperationCompleted;

        m_loadedScenesCount--;
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
