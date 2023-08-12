using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateConversation : MonoBehaviour
{
    private List<Conversation> possibleConversations;
    private Conversation curConversation;
    private IEnumerator curConversationMethod;
    [SerializeField] private List<GameObject> possibleMessagePopUp;

    // Start is called before the first frame update
    private void Start()
    {
        curConversation = null;
        List<Message> testMessages = new List<Message>();
        testMessages.Add(new Message("Hello world!"));
        possibleConversations = new List<Conversation>();
        possibleConversations.Add(new Conversation(testMessages));
        StopCurConversation();
    }

    public void StartRandomConversation()
    {
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
            GameObject popup = Utilities.GetRandomFromList<GameObject>(possibleMessagePopUp);
            TMP_Text popupText = popup.GetComponentInChildren<TMP_Text>();
            popupText.text = message.GetText();
            popup.GetComponentInChildren<Image>().color = message.GetBackgroundColor();
            popup.SetActive(true);
            yield return new WaitForSeconds(curConversation.GetTimeSaid());
            popup.SetActive(false);
            yield return new WaitForSeconds(curConversation.GetTimeBetween());
        }
        curConversation = null;
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
    private float minTimeSaid = 1f;
    private float maxTimeSaid = 2f;

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

    public Message(string text) : this(text, Color.cyan) { }

    public Message(string text, Color backgroundColor)
    {
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
}