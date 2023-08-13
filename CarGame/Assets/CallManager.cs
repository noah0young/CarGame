using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CallManager : MonoBehaviour
{
    [SerializeField] private int numCallsAfterHangUp = 3;
    private AudioSource audio;
    [SerializeField] private int totalRings = 5;
    [SerializeField] private float timeBetweenRings = .5f;
    [SerializeField] private GameObject callUI;
    [SerializeField] private TMP_Text callerName;
    [SerializeField] private List<string> possibleCallers;
    private string curCaller;
    [SerializeField] private float minCallWait = .5f;
    [SerializeField] private float maxCallWait = 4f;
    [SerializeField] private GenerateConversation callConversations;
    private Shake cameraShake;
    private IEnumerator callVibrationsMethod;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        cameraShake = Camera.main.GetComponent<Shake>();
        callVibrationsMethod = null;
    }

    public void RandomCall()
    {
        curCaller = Utilities.GetRandomFromList<string>(possibleCallers);
        CallFrom(curCaller);
    }

    public void CallFrom(string name)
    {
        if (callVibrationsMethod == null)
        {
            callUI.SetActive(true);
            curCaller = name;
            callerName.text = curCaller;
            callVibrationsMethod = CallVibrations();
            StartCoroutine(callVibrationsMethod);
        }
    }

    private IEnumerator CallVibrations()
    {
        for (int i = 0; i < totalRings; i++)
        {
            cameraShake.start = true;
            audio.Play();
            yield return new WaitForSeconds(timeBetweenRings);
        }
        audio.Stop();
        callVibrationsMethod = null;
        callUI.SetActive(false);
    }

    public void HangUp()
    {
        StartCoroutine(Recall(curCaller));
        if (callVibrationsMethod != null)
        {
            StopCoroutine(callVibrationsMethod);
        }
        audio.Stop();
        callUI.SetActive(false);
    }

    private IEnumerator Recall(string prevCaller)
    {
        yield return new WaitForSeconds(Random.Range(minCallWait, maxCallWait));
        CallFrom(prevCaller);
    }

    public void Answer()
    {
        callConversations.StartConversation(curCaller);
        if (callVibrationsMethod != null)
        {
            StopCoroutine(callVibrationsMethod);
        }
        audio.Stop();
        callUI.SetActive(false);
    }
}
