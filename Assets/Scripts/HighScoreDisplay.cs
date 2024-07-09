using UnityEngine;
//using UnityEngine.UI;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highScoreText1;
    public TextMeshProUGUI highScoreText2;
    public TextMeshProUGUI highScoreText3;

    void Start()
    {
        int[] scores = LoadScores();
        highScoreText1.text = "1. " + scores[0].ToString();
        highScoreText2.text = "2. " + scores[1].ToString();
        highScoreText3.text = "3. " + scores[2].ToString();
    }

    private int[] LoadScores()
    {
        int[] scores = new int[3];
        scores[0] = PlayerPrefs.GetInt("HighScore1", 0);
        scores[1] = PlayerPrefs.GetInt("HighScore2", 0);
        scores[2] = PlayerPrefs.GetInt("HighScore3", 0);
        return scores;
    }
}
