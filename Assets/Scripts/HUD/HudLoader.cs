using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HudLoader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
    }
}
