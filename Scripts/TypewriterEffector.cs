using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class TypewriterEffector : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public float waitingSeconds = Constants.DEFAULT_WAITING_SECONDS;

    private Coroutine typingCoroutine;
    private bool isTyping;
    public AudioSource vocal;
    
    public void StartTyping(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(text));
        
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        textDisplay.text = text;
        textDisplay.maxVisibleCharacters = 0;

        for(int i = 0; i <= text.Length; i++)
        {
            if (Input.GetMouseButton(0))
            {
                textDisplay.text = text;
                if (vocal.isPlaying)
                {
                    vocal.Stop();
                }
                yield return new WaitForSeconds(waitingSeconds);
            }
            textDisplay.maxVisibleCharacters = i;
            yield return new WaitForSeconds(waitingSeconds);
        }
        isTyping = false;
    }

    public void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        textDisplay.maxVisibleCharacters = textDisplay.text.Length;
        /*if (vocal.isPlaying)
        {
            vocal.Stop();
        }*/
        isTyping = false;
    }

    public bool IsTyping()
    {
        return isTyping;
    }
}
