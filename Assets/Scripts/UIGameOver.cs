using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGameOver : MonoBehaviour
{

    public int displayedPoints;
    public TextMeshProUGUI pointsUI;



    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameStateUpdated.AddListener(GameStateUpdated);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    public void GameStateUpdated(GameManager.GameState state)
    {
        if (state == GameManager.GameState.GameOver)
        {
            displayedPoints = 0;
            StartCoroutine(DisplayPointsCoroutine());
        }
    }

    IEnumerator DisplayPointsCoroutine()
    {
        while (displayedPoints < GameManager.Instance.Points)
        {
            displayedPoints++;
            pointsUI.text = displayedPoints.ToString();
            yield return new WaitForFixedUpdate();
        }

        displayedPoints = GameManager.Instance.Points;
        Debug.Log("" + displayedPoints);
        pointsUI.text = displayedPoints.ToString();

        yield return null;
    }


    public void PlayAgainButtonClicked()
    {
        GameManager.Instance.RestartGame();
    }


    public void ExitGameButtonClicked()
    {
        GameManager.Instance.ExitGame();
    }
}
