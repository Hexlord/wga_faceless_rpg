using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaySoundOnClick : MonoBehaviour, IPointerEnterHandler
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    void onClick()
    {
        var sound = transform.FindPrecise("SoundClick", false);
        if (sound)
        {
            sound.GetComponent<AudioSource>().Play();
        }
    }

    void onHover()
    {
        var sound = transform.FindPrecise("SoundHover", false);
        if (sound)
        {
            sound.GetComponent<AudioSource>().Play();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover();
    }
}
