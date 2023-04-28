using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;
using unityutilities;
public class DataLogger : MonoBehaviour
{
    //uses the logger utility to log user data into file
    public float logInterval;
    //public string fileID;
    //public string logDirectory;
    [Header("Items to be logged")]
   // public string playerDataPath; // file
    public bool startLogging;
    public Transform player;
    float prev;
    public string filePath;
    // Start is called before the first frame update
    void Start()
    {
        startLogging = false; // must be set by the user/dev
        prev = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (startLogging)
        {
            // log player position here every interval seconds
            if(Time.time - prev > logInterval)
            {
                prev = Time.time;
                //log player position into file
                List<string> data = new List<string>();
                data = new List<string>() {
                    player.position.ToString(), SceneManager.GetActiveScene().name
                };
                unityutilities.Logger.LogRow(filePath, data);
            }
        }
    }

    public void InitializeLogging(Transform player_in, float interval, string userID, string logDirectory, string userIDPath)
    {
        logInterval = interval;
        player = player_in;
        //activated once per test study, in gamemanager when devmode finishes entering the file name
        //gets information from playerStats, which should now contain the file name
        if (!startLogging)
        {
            //path = @"D:\Andrew\git_repo\BalanceStudy\Data\balanceStudyIDs.txt";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            filePath = logDirectory + "/" + userID + "_movement";
            if (!File.Exists(userIDPath))
            {
                //create file and write ID into it.
                using (StreamWriter sw = new StreamWriter(userIDPath))
                {
                    sw.WriteLine(userID);
                    sw.Flush();
                }
            }
            else
            {
                // Read each line of the file into a string array. Each element
                // of the array is one line of the file.
                string[] lines = File.ReadAllLines(userIDPath);

                //check if the generated string is in the file
                bool notNew = true;
                while (notNew)
                {
                    notNew = false;
                    foreach (string line in lines)
                    {
                        if (userID == line)
                        {
                            notNew = true;
                            userID = userID + "copy";//make it unique
                            break;
                        }
                    }
                }
                using (StreamWriter w = File.AppendText(userIDPath))
                {
                    w.WriteLine(userIDPath);
                    w.Flush();
                }
            }
            startLogging = true;
        }
    }
}
