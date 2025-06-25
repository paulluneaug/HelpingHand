using UnityEngine;

public class PuppetAudio : MonoBehaviour
{
    [Header("Wwise Events - Marionnette")]
    public AK.Wwise.Event strikeEvent;
    public AK.Wwise.Event walkEvent;
    public AK.Wwise.Event runEvent;
    public AK.Wwise.Event idleEvent;

    /// <summary>
    /// Joue un son Wwise en fonction du type d'animation.
    /// À appeler via Animation Event.
    /// </summary>
    /// <param name="soundName">Nom de l'événement à jouer : "Strike", "Walk", "Run", "Idle"</param>
    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "Strike":
                PlayWwiseEvent(strikeEvent);
                break;

            case "Walk":
                PlayWwiseEvent(walkEvent);
                break;

            case "Run":
                PlayWwiseEvent(runEvent);
                break;

            case "Idle":
                PlayWwiseEvent(idleEvent);
                break;

            default:
                Debug.LogWarning($"[MarionnetteAudio] Aucun son trouvé pour : {soundName}");
                break;
        }
    }

    private void PlayWwiseEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent != null)
        {
            _ = wwiseEvent.Post(gameObject);
        }
        else
        {
            Debug.LogWarning("[MarionnetteAudio] Wwise Event non assigné !");
        }
    }
}
