using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateConversation : MonoBehaviour
{
    private List<Conversation> possibleConversations;
    private Conversation curConversation;
    private IEnumerator curConversationMethod;

    // Start is called before the first frame update
    private void Start()
    {
        curConversation = null;
        List<Message> testMessages = new List<Message>();
        testMessages.Add(new Message("Hello world!"));
        possibleConversations = new List<Conversation>();
        possibleConversations.Add(new Conversation(testMessages));
    }

    public void StartRandomConversation()
    {
        curConversation = Utilities.GetRandomFromList<Conversation>(possibleConversations);
        curConversationMethod = RunConversation();
        StartCoroutine(curConversationMethod);
    }

    private IEnumerator RunConversation()
    {
        yield return null;
        while (!curConversation.Finished())
        {
            
        }
        curConversation = null;
    }
}

public class Conversation
{
    private List<Message> messages;
    private int messageIndex = 0;

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