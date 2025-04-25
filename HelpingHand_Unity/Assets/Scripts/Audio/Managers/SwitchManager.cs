using UnityEngine;
using System.Collections.Generic;
using AK.Wwise;

public class WwiseSwitchManager : MonoBehaviour
{
    public static WwiseSwitchManager Instance { get; private set; }

    [Header("Locomotion Switches")]
    public Switch idleSwitch;
    public Switch runSwitch;
    public Switch walkSwitch;
    public Switch stairsSwitch;

    [Header("Material Switches")]
    public Switch dirtSwitch;
    public Switch grassSwitch;
    public Switch gravelSwitch;
    public Switch rockSwitch;
    public Switch woodSwitch;

    private Dictionary<LocomotionType, Switch> locomotionSwitches;
    private Dictionary<MaterialType, Switch> materialSwitches;

    [Header("Default Target Object")]
    public GameObject targetObject;

    public enum LocomotionType
    {
        Idle,
        Run,
        Walk,
        Stairs
    }

    public enum MaterialType
    {
        Dirt,
        Grass,
        Gravel,
        Rock,
        Wood
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (targetObject == null)
            targetObject = gameObject;

        InitSwitchDictionaries();
    }

    private void InitSwitchDictionaries()
    {
        locomotionSwitches = new Dictionary<LocomotionType, Switch>
        {
            { LocomotionType.Idle, idleSwitch },
            { LocomotionType.Run, runSwitch },
            { LocomotionType.Walk, walkSwitch },
            { LocomotionType.Stairs, stairsSwitch }
        };

        materialSwitches = new Dictionary<MaterialType, Switch>
        {
            { MaterialType.Dirt, dirtSwitch },
            { MaterialType.Grass, grassSwitch },
            { MaterialType.Gravel, gravelSwitch },
            { MaterialType.Rock, rockSwitch },
            { MaterialType.Wood, woodSwitch }
        };
    }

    public void SetLocomotionSwitch(LocomotionType type, GameObject target = null)
    {
        if (locomotionSwitches.TryGetValue(type, out var switchValue))
        {
            switchValue.SetValue(target ?? targetObject);
        }
        else
        {
            Debug.LogWarning($"[WwiseSwitchManager] Locomotion switch not found for: {type}");
        }
    }

    public void SetMaterialSwitch(MaterialType material, GameObject target = null)
    {
        if (materialSwitches.TryGetValue(material, out var switchValue))
        {
            switchValue.SetValue(target ?? targetObject);
        }
        else
        {
            Debug.LogWarning($"[WwiseSwitchManager] Material switch not found for: {material}");
        }
    }
}
