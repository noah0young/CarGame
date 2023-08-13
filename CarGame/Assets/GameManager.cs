using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [Header("Score")]
    private static int score = 0;
    private float nextScoreIncrease = 0;
    [SerializeField] private float scoreIncRate = 2;
    [SerializeField] private TMP_Text scoreText;

    [Header("Random Conversations")]
    [SerializeField] private GenerateConversation randomConversation;
    [SerializeField] private AudioClip[] clips = new AudioClip[5];
    [SerializeField] private float randomConversationDelay = 4;
    [SerializeField] private float randomConversationMinTimeBetween = 4;
    [SerializeField] private float randomConversationMaxTimeBetween = 8;

    [Header("Story Conversations")]
    [SerializeField] private GenerateConversation storyConversation;
    [SerializeField] private List<int> minScoreForStory = new List<int>();

    [Header("Texts")]
    [SerializeField] private GenerateConversation textConversation;
    [SerializeField] private float textsDelay = 6;
    [SerializeField] private float textsMinTimeBetween = 3;
    [SerializeField] private float textsMaxTimeBetween = 10;

    [Header("Calls")]
    [SerializeField] private CallManager callManager;
    [SerializeField] private GenerateConversation callConversation;
    [SerializeField] private float callsMinTimeBetween = 3;
    [SerializeField] private float callsMaxTimeBetween = 6;

    // Start is called before the first frame update
    private void Start()
    {
        score = 0;
        if (instance != null)
        {
            throw new System.Exception("There should only be one GameManager");
        }
        instance = this;
        instance.UpdateScoreText();
        InitConversations();
        StartCoroutine(StartRandomConversation());
        StartCoroutine(StartStoryConversations());
        StartCoroutine(StartCallsConversations());
        StartCoroutine(StartTextConversations());
    }

    private void InitConversations()
    {
        // [RANDOM]
        randomConversation.possibleConversations = new List<Conversation>();
        List<Message> tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Mom! I'm hungry"
            , Color.cyan, "You", 3, clips[0]));
        tempMessages.Add(new Message(
            "Didn't you just eat 10 minutes ago?"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "..."
            , Color.cyan, "You"));
        randomConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Are we there yet!"
            , Color.cyan, "You", 3.5f, clips[2]));
        randomConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Mom, I need to pee"
            , Color.cyan, "You", 4, clips[1]));
        randomConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "I'm tired"
            , Color.cyan, "You", 1.5f, clips[3]));
        randomConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Moooooom"
            , Color.cyan, "You", 1f, clips[4]));
        tempMessages.Add(new Message(
            "What is it, kiddo?"
            , Color.magenta, "Mom"));
        randomConversation.possibleConversations.Add(new Conversation(tempMessages));

        // [STORY]
        storyConversation.forceStopConversation = randomConversation;
        storyConversation.possibleConversations = new List<Conversation>();
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Are you having fun, kiddo ? Look at all these cool landmarks we are passing! If you keep looking at that game of yours, you might miss one that you wish you saw!"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "It's the same site as always Mom, it's not like I’m missing anything, I just don’t understand why we are having to move again for the fifth time and I have to say bye to friends and this place again"
            , Color.cyan, "You"));
        tempMessages.Add(new Message(
            "I know you're bummed about leaving school again, but think of it like this, it's a whole new experience and you get to travel the country and meet whole new people!"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "Hmmm…"
            , Color.cyan, "You"));
        storyConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Alright kiddo car is all filled up with gas, ready to get back on the road ? We’re almost halfway through our journey"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
        "Yea, Mom, I’m on my way… Wonder if we’ll stay long enough for me to make friends or get to know the teacher's name. Wish Dad had picked a different career to go into…"
        , Color.cyan, "You"));
        tempMessages.Add(new Message(
            "Look kiddo, I know your bumbed about having to move over and over, but it’s part of your Dad’s job.If it weren’t for him, we wouldn’t be able to go on these trips and see these wonderful places.Plus he is saving lots of lives at the hospitals he goes to, even if it does mean we have to move constantly"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
        "But I don’t wanna see all these new places, I just want a place that we can sit down and call home, every time we move I always have to make new friends and then say goodbye when Dad moves!"
        , Color.cyan, "You"));
        tempMessages.Add(new Message(
        "..."
        , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
        "..."
        , Color.cyan, "You"));
        storyConversation.possibleConversations.Add(new Conversation(tempMessages));
        storyConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Alright we finally made it to the hotel, ready to go to bed kiddo ?"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "Yeah yeah, I know we have to get up early tomorrow for the trip, so can you please just let me sleep"
            , Color.cyan, "You"));
        tempMessages.Add(new Message(
            "Listen kiddo, after what you said in the car ride today, I’ve been thinking about how you feel and why you are upset about us moving for the 5th time.I know you want a place to stay, but due to Dad’s job we always are going to have to move, it's just part of the job y’know?"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "I know… it's just I wish that we could just stay in one place and not have to move so that I can make good friends and be able to have a place I can call home instead of always needing to move and go places."
            , Color.cyan, "You"));
        tempMessages.Add(new Message(
            "I can tell that you’re upset so here’s what I’ll do, how about once we reach our destination, I’ll talk to your Dad about how you feel and propose the idea to him, is that ok with you?"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "Really??? Do you mean it???"
            , Color.cyan, "You"));
        tempMessages.Add(new Message(
            "But that doesn’t mean the road trips are going to end, when your dad decides to take time off, we are going to road trip to wherever we want and you are tagging along with us"
            , Color.magenta, "Mom"));
        tempMessages.Add(new Message(
            "Alright Mom, thank you for understanding."
            , Color.cyan, "You"));
        tempMessages.Add(new Message(
            "No problem kiddo"
            , Color.magenta, "Mom"));
        storyConversation.possibleConversations.Add(new Conversation(tempMessages));

        // [CALLS]
        callConversation.possibleConversations = new List<Conversation>();
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Just saying hi"
            , Color.grey, "Dad"));
        tempMessages.Add(new Message(
            "Are you guys close to the hotel?"
            , Color.grey, "Dad"));
        tempMessages.Add(new Message(
            "What's your ETA?"
            , Color.grey, "Dad"));
        callConversation.possibleConversations.Add(new Conversation(tempMessages));

        // [TEXTS]
        textConversation.possibleConversations = new List<Conversation>();
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "See you soon champ"
            , Color.green, "Dad"));
        textConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Have a safe trip"
            , Color.green, "Jimmy"));
        textConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Where are you?"
            , Color.green, "Steve"));
        textConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "Enjoy the road!!!"
            , Color.green, "Clara"));
        textConversation.possibleConversations.Add(new Conversation(tempMessages));
        tempMessages = new List<Message>();
        tempMessages.Add(new Message(
            "See anything cool?"
            , Color.green, "Johnathan"));
        textConversation.possibleConversations.Add(new Conversation(tempMessages));
    }
    private void Update()
    {
        nextScoreIncrease += Time.deltaTime * scoreIncRate;
        if (nextScoreIncrease > 1)
        {
            score += (int)nextScoreIncrease;
            nextScoreIncrease -= 1;
            instance.UpdateScoreText();
        }
    }
    public static void AddToScore(int amount)
    {
        score += amount;
        instance.UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
    private IEnumerator StartRandomConversation()
    {
        yield return new WaitForSeconds(randomConversationDelay);
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(randomConversationMinTimeBetween, randomConversationMaxTimeBetween));
            randomConversation.StartRandomConversation();
            yield return new WaitUntil(() => randomConversation.IsDone());
        }
    }
    private IEnumerator StartStoryConversations()
    {
        for (int i = 0; i < minScoreForStory.Count; i++)
        {
            yield return new WaitUntil(() => score >= minScoreForStory[i]);
            storyConversation.StartConversation(i);
        }
    }
    private IEnumerator StartCallsConversations()
    {
        yield return new WaitUntil(() => score >= minScoreForStory[1]);
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(callsMinTimeBetween, callsMaxTimeBetween));
            callManager.RandomCall();
            yield return new WaitUntil(() => callConversation.IsDone());
        }
    }
    private IEnumerator StartTextConversations()
    {
        yield return new WaitUntil(() => score >= minScoreForStory[0]);
        yield return new WaitForSeconds(textsDelay);
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(textsMinTimeBetween, textsMaxTimeBetween));
            textConversation.StartRandomConversation();
            yield return new WaitUntil(() => randomConversation.IsDone());
        }
    }
}
