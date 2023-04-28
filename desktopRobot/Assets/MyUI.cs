using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MyUI : MonoBehaviour
{
    public Button StartButton;
    public TMP_InputField IDEntry;
    public Button submitButton;
    public static MyUI Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (StartButton != null)
        {
            StartButton.gameObject.SetActive(false);
            submitButton.interactable = false;
        }

    }

    public void completedTextEntry()
    {
        DataManager.Instance.userID = IDEntry.text;
        StartButton.gameObject.SetActive(true);
    }
    public void ActivateSubmitButton()
    {
        submitButton.interactable = true;
    }
    public void LoadScene1()
    {
        SceneManager.LoadScene("scene1");
    }
    public void LoadScene2()
    {
        SceneManager.LoadScene("scene2");
    }
    public void LoadWelcomingScene()
    {
        SceneManager.LoadScene("WelcomeScene");
    }
}
