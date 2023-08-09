using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPoints : MonoBehaviour
{

    int displayedPoints = 0;

    public TextMeshProUGUI pointsLabel;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnPointsUpdated.AddListener(UpdatePoints);
        GameManager.Instance.OnGameStateUpdated.AddListener(GameStateUpdated);
    }


    public void UpdatePoints()
    {
        StartCoroutine(UpdatePointsCoroutine());
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPointsUpdated.RemoveListener(UpdatePoints);
        GameManager.Instance.OnGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    IEnumerator UpdatePointsCoroutine()
    {
        while (displayedPoints < GameManager.Instance.Points)
        {
            displayedPoints++;
            pointsLabel.text = displayedPoints.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    public void GameStateUpdated(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.GameOver)
        {
            displayedPoints = 0;
            pointsLabel.text = displayedPoints.ToString();
        }
    }
}
