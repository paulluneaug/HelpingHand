using System.Collections.Generic;
using AK.Wwise;
using Sirenix.OdinInspector;
using UnityEngine;

public class SwitchManager : MonoBehaviour
{
    #region Switchs
    [InfoBox("➔<b>SetLocomotionSwitch(LocomotionType.Run, gameObject);</b>", InfoMessageType.None)]
    #region Locomotion Switches
    [FoldoutGroup("Locomotion Switches")][LabelWidth(100)] public Switch IdleSwitch;
    [FoldoutGroup("Locomotion Switches")][LabelWidth(100)] public Switch RunSwitch;
    [FoldoutGroup("Locomotion Switches")][LabelWidth(100)] public Switch WalkSwitch;
    [FoldoutGroup("Locomotion Switches")][LabelWidth(100)] public Switch StairsSwitch;
    #endregion

    #region Material Switches
    [InfoBox("➔<b>SetMaterialSwitch(MaterialType.Rock, gameObject);</b>", InfoMessageType.None)]
    [FoldoutGroup("Material Switches")][LabelWidth(100)] public Switch DirtSwitch;
    [FoldoutGroup("Material Switches")][LabelWidth(100)] public Switch GrassSwitch;
    [FoldoutGroup("Material Switches")][LabelWidth(100)] public Switch GravelSwitch;
    [FoldoutGroup("Material Switches")][LabelWidth(100)] public Switch RockSwitch;
    [FoldoutGroup("Material Switches")][LabelWidth(100)] public Switch WoodSwitch;
    #endregion
    #endregion Switchs

    #region Dictionnary
    private Dictionary<LocomotionType, Switch> m_locomotionSwitches;
    private Dictionary<MaterialType, Switch> m_materialSwitches;
    #endregion

    [Header("Default Target Object")] // GameObject cible pour appliquer les switches Wwise si aucune target n'est précisée dans les appels de fonction
    public GameObject TargetObject;

    #region Enums
    [SerializeField]
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
    #endregion

    public void InitSwitchDictionaries()
    {
        m_locomotionSwitches = new Dictionary<LocomotionType, Switch>
        {
            { LocomotionType.Idle, IdleSwitch },
            { LocomotionType.Run, RunSwitch },
            { LocomotionType.Walk, WalkSwitch },
            { LocomotionType.Stairs, StairsSwitch }
        };

        m_materialSwitches = new Dictionary<MaterialType, Switch>
        {
            { MaterialType.Dirt, DirtSwitch },
            { MaterialType.Grass, GrassSwitch },
            { MaterialType.Gravel, GravelSwitch },
            { MaterialType.Rock, RockSwitch },
            { MaterialType.Wood, WoodSwitch }
        };
    }

    public void SetLocomotionSwitch(LocomotionType type, GameObject target = null)
    {
        if (m_locomotionSwitches.TryGetValue(type, out var switchValue))
        {
            switchValue.SetValue(target ?? TargetObject);
        }
        else
        {
            Debug.LogWarning($"[WwiseSwitchManager] Locomotion switch not found for: {type}");
        }
    }

    public void SetMaterialSwitch(MaterialType material, GameObject target = null)
    {
        if (m_materialSwitches.TryGetValue(material, out var switchValue))
        {
            switchValue.SetValue(target ?? TargetObject);
        }
        else
        {
            Debug.LogWarning($"[WwiseSwitchManager] Material switch not found for: {material}");
        }
    }
}
