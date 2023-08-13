using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private string levelName = "True Scene";
    [SerializeField] private string menuName = "Title";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene(levelName);
    }

    public void OnReturnToMenu()
    {
        SceneManager.LoadScene(menuName);
    }
}
