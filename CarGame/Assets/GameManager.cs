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
            Debug.Log("Start Random Conversation");
            randomConversation.StartRandomConversation();
            yield return new WaitUntil(() => randomConversation.IsDone());
        }
    }
}
