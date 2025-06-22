using UnityEngine;

public class WwiseDynamicDialogueTester : MonoBehaviour
{
    // Pour référencer le dialogue event : pas possible de faire une variable comme un event classique
    // Il faut référencer l'ID stocké dans la soundbank
    private uint eDirectorVoice = AkUnitySoundEngine.AK_INVALID_UNIQUE_ID;

    [Header("States Wwise utilisés")]
    [SerializeField] private AK.Wwise.State Etat;
    [SerializeField] private AK.Wwise.State Objet;
    [SerializeField] private AK.Wwise.State Repetition;

    private void Start()
    {
        PlayDialogue();
    }

    public void PlayDialogue()
    {

        eDirectorVoice = AkUnitySoundEngine.GetIDFromString("DirectorVoice");
        // Vérifie que l'ID de l'event est valide
        if (eDirectorVoice == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            Debug.LogError("❌ L'ID de l'event 'DirectorVoice' est invalide. Vérifie le nom et la soundbank.");
            return;
        }

        // Ouvre une séquence dynamique avec un GameObject valide
        uint sequenceID = AkUnitySoundEngine.DynamicSequenceOpen(this.gameObject);
        AkPlaylist pPlaylist = AkUnitySoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        // Référence au chemin de dialogue dynamique avec des uints (arguments)
        uint[] aDirectorVoice = new uint[3] { Etat.Id, Objet.Id, Repetition.Id };

        uint nodeID = AkUnitySoundEngine.ResolveDialogueEvent(eDirectorVoice, aDirectorVoice, (uint)aDirectorVoice.Length);
        if (nodeID == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            Debug.LogError("❌ Échec de la résolution du Dialogue Event. Vérifie les states assignés.");
            _ = AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            _ = AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            return;
        }

        _ = pPlaylist.Enqueue(nodeID);

        _ = AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        _ = AkUnitySoundEngine.DynamicSequencePlay(sequenceID);
        _ = AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
    }
}
