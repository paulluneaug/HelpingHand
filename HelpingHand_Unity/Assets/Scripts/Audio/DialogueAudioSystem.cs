using System.Collections;

using UnityEngine;

using WwiseEvent = AK.Wwise.Event;

public class DialogueAudioSystem : MonoBehaviour
{
    private int narration = 0; //Compteur de phrase de dialogue
    private readonly object waitTime;
    public WwiseEvent DialLine;
    public WwiseEvent DialInterrupt;
    public WwiseEvent DialResume;
    private void Start()
    {
        PlayNarration();
    }

    public void PlayNarration()
    {
        if (narration == 4)
        {

        }
        else
        {
            _ = AkUnitySoundEngine.PostEvent("blabla", gameObject, (uint)AkCallbackType.AK_EndOfEvent, NarrationEnd, waitTime);
            //Callback : pour qu'Unity ait l'info de quand l'event se termine.
            //WaitTime : Délai avant que wwise envoie les infos à unity
            //Narration End : fonction jouée quand l'event est terminée
            narration++;
            //Dans ce cas il enchaîne les instructions avec un délai entre les instructions (un peu comme sequence container)

            //-> Pour jouer un son en passant par l'EventManager
            //AudioManager.Instance.EventManager.PostWithCallback(gameObject, AudioManager.Instance.EventManager.SquareRockLoop_Play, AkCallbackType.AK_EndOfEvent, NarrationEnd, waitTime);
        }
    }

    public void NarrationEnd(object in_cookie, AkCallbackType in_type, object in_info) //Variables utilisées par Wwise pour les events callback
    {
        _ = StartCoroutine("Wait");
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        PlayNarration();
    }

}