using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Text score;
    private int count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = GameObject.Find("Score").GetComponent<Text>();
        count = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncScore()
    {
        count++;
        score.text = count.ToString();
    }
}
