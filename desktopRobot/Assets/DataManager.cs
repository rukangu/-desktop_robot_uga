using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public string userID;
    public static DataManager Instance;
    public DataLogger myLogger;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        myLogger = gameObject.GetComponent<DataLogger>();
        if (myLogger == null)
        {
            Debug.LogError("DataLogger not attached");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
