using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip moveSFX;
    public AudioClip missSFX;
    public AudioClip matchSFX;
    public AudioClip gameOverSFX;


    public AudioSource SFXSource;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnPointsUpdated.AddListener(PointsUpdated);
        GameManager.Instance.OnGameStateUpdated.AddListener(GameStateUpdated);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPointsUpdated.RemoveListener(PointsUpdated);
        GameManager.Instance.OnGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    public void PointsUpdated()
    {
        SFXSource.PlayOneShot(matchSFX);
    }

    public void GameStateUpdated(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.GameOver)
        {
            SFXSource.PlayOneShot(gameOverSFX);
        }

        if (newState == GameManager.GameState.InGame)
        {
            SFXSource.PlayOneShot(matchSFX);
        }


    }

    public void Move()
    {
        SFXSource.PlayOneShot(moveSFX);

    }

    public void Miss()
    {
        SFXSource.PlayOneShot(missSFX);

    }
}
