using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Unity.VisualScripting;

public class GenerateConversation : MonoBehaviour
{
    public GenerateConversation forceStopConversation;
    public List<Conversation> possibleConversations;
    private Conversation curConversation;
    private IEnumerator curConversationMethod;
    [SerializeField] private List<GameObject> possibleMessagePopUp;
    [SerializeField] private GameObject popupPrefab;
    private bool noTalking = false;
    [SerializeField] private bool isText = false;
    private AudioSource textPing;
    private Shake cameraShake;

    // Start is called before the first frame update
    private void Start()
    {
        StopCurConversation();
        if (isText)
        {
            textPing = GetComponent<AudioSource>();
            cameraShake = Camera.main.GetComponent<Shake>();
        }
    }

    private void SetNoTalking(bool shouldTalk)
    {
        noTalking = shouldTalk;
        if (noTalking)
        {
            Debug.Log("No Talking");
            StopCurConversation();
        }
    }

    public void StartConversation(int conNum)
    {
        if (noTalking)
        {
            return;
        }
        if (forceStopConversation != null)
        {
            forceStopConversation.SetNoTalking(true);
        }
        if (conNum >= possibleConversations.Count)
        {
            throw new System.Exception("Conversation out of bounds");
        }
        StopCurConversation();
        curConversation = possibleConversations[conNum];
        curConversationMethod = RunConversation();
        StartCoroutine(curConversationMethod);
    }

    public void StartConversation(string name)
    {
        for (int i = 0; i < possibleConversations.Count; i++)
        {
            if (name.Contains(possibleConversations[i].Copy().GetNext().GetName()))
            {
                StartConversation(i);
                return;
            }
        }
    }

    public void StartRandomConversation()
    {
        if (noTalking)
        {
            return;
        }
        StopCurConversation();
        curConversation = Utilities.GetRandomFromList<Conversation>(possibleConversations).Copy();
        curConversationMethod = RunConversation();
        StartCoroutine(curConversationMethod);
    }

    private IEnumerator RunConversation()
    {
        while (!curConversation.Finished())
        {
            Message message = curConversation.GetNext();
            GameObject popupLoc = Utilities.GetRandomFromList<GameObject>(possibleMessagePopUp);
            GameObject popup = Instantiate(popupPrefab, popupLoc.transform.parent);
            popup.transform.position = popupLoc.transform.position;
            Popup popupComponent = popup.GetComponent<Popup>();
            popupComponent.speakerName = message.GetName();
            popupComponent.text = message.GetText();
            popupComponent.color = message.GetBackgroundColor();
            if (isText)
            {
                textPing.Play();
                cameraShake.start = true;
            }
            yield return new WaitForSeconds(curConversation.GetTimeSaid());
            popup.SetActive(false);
            yield return new WaitForSeconds(curConversation.GetTimeBetween());
        }
        curConversation = null;
        if (forceStopConversation != null)
        {
            forceStopConversation.SetNoTalking(false);
            Debug.Log("No Talking End");
        }
    }

    private void StopCurConversation()
    {
        foreach (GameObject popup in possibleMessagePopUp)
        {
            popup.SetActive(false);
        }
        if (curConversationMethod != null)
        {
            StopCoroutine(curConversationMethod);
            curConversationMethod = null;
        }
        if (forceStopConversation != null)
        {
            forceStopConversation.SetNoTalking(false);
        }
        curConversation = null;
    }

    public bool IsDone()
    {
        return curConversation == null;
    }
}

public class Conversation
{
    private List<Message> messages;
    private int messageIndex = 0;
    private float minTimeBetween = 3f;
    private float maxTimeBetween = 4f;
    private float minTimeSaid = 3f;
    private float maxTimeSaid = 4f;

    public Conversation(List<Message> messages)
    {
        this.messages = messages;
        messageIndex = 0;
    }

    public Message GetNext()
    {
        if (messageIndex >= messages.Count)
        {
            throw new System.Exception("No more messages exist");
        }
        Message message = messages[messageIndex];
        messageIndex += 1;
        return message;
    }

    public bool Finished()
    {
        return messageIndex >= messages.Count;
    }

    public float GetTimeBetween()
    {
        return Random.Range(minTimeBetween, maxTimeBetween);
    }

    public float GetTimeSaid()
    {
        return Random.Range(minTimeSaid, maxTimeSaid);
    }

    public Conversation Copy()
    {
        return new Conversation(messages);
    }
}

public class Message
{
    private string text;
    private Color backgroundColor;
    private string name;

    public Message(string text) : this(text, Color.cyan) { }

    public Message(string text, Color backgroundColor) : this(text, backgroundColor, null) { }

    public Message(string text, Color backgroundColor, string name)
    {
        this.name = name;
        this.text = text;
        this.backgroundColor = backgroundColor;
    }

    public string GetText()
    {
        return text;
    }

    public Color GetBackgroundColor()
    {
        return backgroundColor;
    }

    public string GetName()
    {
        return name;
    }
}