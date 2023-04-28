using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void loadFKScene()
    {
        SceneManager.LoadScene("RobotDemo1");
    }
    public void loadIKScene()
    {
        SceneManager.LoadScene("RobotDemo2");
    }
}
