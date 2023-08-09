using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartScreen : MonoBehaviour
{

    public void StartButtonClicked(){
        GameManager.Instance.StartGame();
    }
}
