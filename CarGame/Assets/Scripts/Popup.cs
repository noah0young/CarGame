using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Popup : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text textText;
    [SerializeField] private Image uiImage;
    public string speakerName { set
        {
            nameText.text = value;
        }
    }
    public string text
    {
        set
        {
            textText.text = value;
        }
    }
    public Color color
    {
        set
        {
            uiImage.color = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
