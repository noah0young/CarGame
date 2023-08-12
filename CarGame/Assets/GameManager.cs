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
    [SerializeField] private float randomConversationDelay = 4;
    [SerializeField] private float randomConversationMinTimeBetween = 4;
    [SerializeField] private float randomConversationMaxTimeBetween = 8;

    [Header("Story Conversations")]
    [SerializeField] private GenerateConversation storyConversation;
    [SerializeField] private List<int> minScoreForStory = new List<int>();

    [Header("Calls")]
    [SerializeField] private GenerateConversation callConversation;
    [SerializeField] private float callsMinTimeBetween = 6;
    [SerializeField] private float callsMaxTimeBetween = 12;

    [Header("Ads")]
    [SerializeField] private GenerateConversation adConversation;
    [SerializeField] private float adsMinTimeBetween = 10;
    [SerializeField] private float adsMaxTimeBetween = 20;

    // Start is called before the first frame update
    private void Start()
    {
        if (instance != null)
        {
            throw new System.Exception("There should only be one GameManager");
        }
        instance = this;
        instance.UpdateScoreText();
        StartCoroutine(StartRandomConversation());
        StartCoroutine(StartStoryConversations());
        StartCoroutine(StartCallsConversations());
        StartCoroutine(StartAdsConversations());
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
            yield return new WaitForSeconds(Random.Range(randomConversationMinTimeBetween, randomConversationMaxTimeBetween));
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
            yield return new WaitForSeconds(Random.Range(callsMinTimeBetween, callsMaxTimeBetween));
            callConversation.StartRandomConversation();
            yield return new WaitUntil(() => callConversation.IsDone());
        }
    }

    private IEnumerator StartAdsConversations()
    {
        yield return new WaitUntil(() => score >= minScoreForStory[2]);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(adsMinTimeBetween, adsMaxTimeBetween));
            adConversation.StartRandomConversation();
            yield return new WaitUntil(() => callConversation.IsDone());
        }
    }
}
