﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Centralized location to check for various conditions that determine what should or should not be updated in the game.
// When the instance of this class is accessed, you may use the public hashtable to search conditions.
// Syntax for a search would be stateConditions["Key"], which will return the corresponding value
public class PersistantStateData : MonoBehaviour
{
    // Global Data
    public static PersistantStateData persistantStateData;  // Static self-reference. Used to enforce singleton.
    public int updateCount;         // A count of how many times the hashtable has been updated after game launched. Should be incremented when modifying hashtable. Need not be preserved after closing application.
    public Hashtable stateConditions;   // Hashtable containing key/value pairs of state conditions (probably limited to string/bool pairs).
    public float globalTime;       // The total elapsed time in seconds
    public bool pauseGlobalTimer = false;   // Turn on to pause the global timer

    public bool realPSD = false;
    // Variables used to save and load game data
    public string savePath;
    

    public GameObject loadingScreen;
    public CanvasGroup blackFade;

    private bool doLoad = false;
    private bool doneLoad = false;
    private bool doFade = false;
    private bool doneFade = false;
    private bool startedAsync = false;
    private string targetFile = "";

    private AsyncOperation async;

    private float fadeDuration = 2f;
    private float fadeRate;

    private float delayInitial;
    private float delayDuration = 10.0f;

    // When scene with this loads, initialize the static variable to object with this script if there is none. Object is persistant through scenes.
    // Otherwise if persistantStateData is already loaded into the game/scene, don't overwrite it and delete this object. This enforces singleton status.
    void Awake()
    {
        //Debug.Log("PSD Awake called");
        if (persistantStateData == null)
        {
            //Debug.Log("PSD static self reference is null");
            DontDestroyOnLoad(gameObject);
            realPSD = true;
            persistantStateData = this;

            stateConditions = new Hashtable();
            globalTime = 0f;
            PopulateStateConditions();
            updateCount = 1;
            savePath = Path.Combine(Application.persistentDataPath, "saves" + Path.DirectorySeparatorChar);
        }
        else if (persistantStateData != this)
        {
            //Debug.Log("PSD static self reference not null. Destroying this.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fadeRate = Time.fixedDeltaTime / fadeDuration;
    }

    private void Update()
    {
        if(!pauseGlobalTimer)
        {
            globalTime += Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            //Debug.Log("Hello");
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            doFade = true;
        }

        if (startedAsync)
        {
            if (Time.time > delayInitial + delayDuration)
            {
                doFade = false;
                doneFade = false;
                doLoad = false;
                doneLoad = false;
                async.allowSceneActivation = true;
            }
        }
    }

    private void FixedUpdate()
    {
        FadeChecker();
    }

    // fills the persistantStateConditions with the various conditions. We can consider passing in arguments for initialization when considering save/load functionality.
    private void PopulateStateConditions()
    {
        if(realPSD)
        {
            //Debug.Log("Populating PSD with variables");
        }
        else
        {
            //Debug.Log("Populating false PSD");
        }
        

        ////////////////////////////////////////////////////////////
        ////////////////////    Tooltip flags   ////////////////////
        ////////////////////////////////////////////////////////////
        stateConditions.Add("TutorialFirstDialogueFinished", false); // Check if opening dialogue done, then trigger look tutorial
        stateConditions.Add("TutorialLookFinished", false); // check if look tutorial done, then trigger move tutorial
        stateConditions.Add("TutorialMovementFinished", false); // check if move tutorial done, then trigger vertical move tutorial
        stateConditions.Add("TutorialVerticalFinished", false); // check if vertical tutorial done, then remove vertical move tutorial
        stateConditions.Add("TutorialWithinShrineRange", false); // check if player within ui marker range of shrine, then trigger ui marker tutorial
        stateConditions.Add("TutorialFirstInteraction", false); // check if player talking with shrine, then trigger dialogue tutorial
        stateConditions.Add("TutorialTalkedWithShrine", false); // ????
        stateConditions.Add("TutorialInteractFinished", false); // ????
        stateConditions.Add("TutorialDialogueSkipped", false); // check if player done with first converation, then trigger attempt clean tutorial
        stateConditions.Add("TutorialAttemptedClean", false); // check if player attempted to clean shrine, then end attempt clean tutorial
        stateConditions.Add("TutorialWithinShellsterRange", false); // check if player within shellster range, then end generate orb tutorial
        stateConditions.Add("TutorialHasEnoughOrbs", false); // check if player has enough orbs to deposit
        stateConditions.Add("TutorialGate", false); // check if player is within gate range

        /////////////////////////////////////////////////////////
        ////////////////////    Task flags   ////////////////////
        /////////////////////////////////////////////////////////
        stateConditions.Add("ShamusHasHat", false);
        stateConditions.Add("ShrineIsClean", false);
        stateConditions.Add("SwingTaskDone", false);
        stateConditions.Add("MerryGoRound", false);

        

        /////////////////////////////////////////////////////////////
        ////////////////////    Dialogue flags   ////////////////////
        /////////////////////////////////////////////////////////////

        stateConditions.Add("StartupShrineDialogueFinished", false);
        stateConditions.Add("StartupShrineDialogue", false);    // Plays dialogue after opening cutscene
        stateConditions.Add("StartupShrinePart2", false);       // Plays dialogue 3 seconds after previous opening dialogue
        stateConditions.Add("StartupShrineRepeatDirections", false);   // Plays every 1 minute after the other dialogue is finished, before player talks to shrine

        // Variables control dialogue of shrine the first time the player talks to it
        stateConditions.Add("ShrineFirstConversation", false);  // Allows player to initiate conversation
        stateConditions.Add("WaitingForCleanAttempt", false);   // Indicates that the shrine will respond once the player attempts to clean it.
        stateConditions.Add("ShrineFirstConversation2Primer", false);   // A primer that tells the shrine time listener to ready activation of conversation
        stateConditions.Add("ShrineFirstConversation2", false); // After player fails to clean shrine, this enables the conversation to continue
        stateConditions.Add("ShrineFirstConversationYes", false);   // If player agrees to help, this plays a dialogue
        stateConditions.Add("ShrineFirstConversationNo", false);    // If player refuses to help, this plays a dialogue
        stateConditions.Add("ShrineFirstConversationOver", false);  // Signifies the first conversation has been completed.

        // Variables control shrine dialogue after player has completed the Garden intro tasks
        stateConditions.Add("ShrineFirstTurnIn", false);  // Marks the first turn in for the tutorial area
        stateConditions.Add("ShrineFirstTurnInNo", false);  // Player says no to the first turn in
        stateConditions.Add("FirstTurnInCutsceneDialogue", false);  // Used to start Dialogue during trun in cutscene
        stateConditions.Add("GoForth", false);  // Used to enable dialogue telling player to leave the garden

        /////////////////////////////////////////////////////////////
        ////////////////////    Cutscene flags   ////////////////////
        /////////////////////////////////////////////////////////////

        stateConditions.Add("Cutscene1Started", false);


        //////////////////////////////////////////////////////////////////
        ////////////////////    Miscellaneous flags   ////////////////////
        //////////////////////////////////////////////////////////////////
        stateConditions.Add("CurrentScene", 2);
        stateConditions.Add("StartupFadeFinished", false);
        stateConditions.Add("OrbDepositInProgress", false);
        stateConditions.Add("DemoEnd", false);
        stateConditions.Add("DebugValue", false);
    }

    public void ResetPersistantStateData()
    {
        //Debug.Log("PSD Reset called");
        stateConditions.Clear();
        PopulateStateConditions();

    }

    public void ChangeStateConditions(string key, bool value)
    {
        //Debug.Log("PSD Value change attempt on " + key + ". Current Value: " + persistantStateData.stateConditions[key] + ". Target value: " + value);
        if((bool)stateConditions[key] != value)
        {
            stateConditions[key] = value;
            updateCount++;
            //Debug.Log("PSD Value changed");
        }
    }

    public void ChangeStateConditions(string key, int value)
    {
        if ((int)stateConditions[key] != value)
        {
            stateConditions[key] = value;
            updateCount++;
        }
    }

    public void ChangeStateConditions(string key, float value)
    {
        if ((float)stateConditions[key] != value)
        {
            stateConditions[key] = value;
            updateCount++;
        }
    }

    public void ChangeStateConditions(Hashtable kvPairs)
    {
        bool modified = false;
        foreach(DictionaryEntry de in kvPairs)
        {
            if(stateConditions[de.Key].GetType().Equals(de.Value.GetType()))
            {
                if (stateConditions[de.Key].ToString() != de.Value.ToString())
                {
                    //Debug.Log("MODIFIED PSD: " + de.Key + " changed from " + stateConditions[de.Key] + " to " + de.Value);
                    stateConditions[de.Key] = de.Value;
                    modified = true;
                }
            }
            
        }

        if (modified)
        {
            updateCount++;
        }
    }

    public string ParseFinalPathPortion(string path)
    {
        Debug.Log("Path separator is: " + Path.DirectorySeparatorChar);
        string[] splitPath = path.Split(Path.DirectorySeparatorChar);
        for(int i = 0; i < splitPath.Length; i++)
        {
            Debug.Log("Path parser part " + i + ": " + splitPath[i]);
        }
        return splitPath[splitPath.Length - 1];
    }

    public void SaveGame(string filename = "ShamusFile")
    {
        //Debug.Log("Saving game");
        int fileNum = 1;
        bool complete = false;

        while (!complete)
        {
            if (File.Exists(Path.Combine(savePath, fileNum + Path.DirectorySeparatorChar + filename + ".txt")))
            {
                fileNum++;
            }
            else
            {
                complete = !complete;
                string saveFile = Path.Combine(savePath, fileNum + Path.DirectorySeparatorChar + filename + ".txt");


                string fileContentString = "";
                foreach (string key in stateConditions.Keys)
                {
                    fileContentString += key + ": " + stateConditions[key] + "\r\n";
                }


                Directory.CreateDirectory(Path.Combine(savePath, "" + fileNum));
                using (FileStream fs = File.Create(saveFile))
                {
                    byte[] saveData = new System.Text.UTF8Encoding(true).GetBytes(fileContentString);
                    fs.Write(saveData, 0, saveData.Length);
                }

            }
        }
    }

    public void LoadFromMenuClick(int loadButtonIndex)
    {
        Debug.Log("Hey there, you're trying to load save data!");
        string loadButtonString = "LoadButton" + loadButtonIndex;
        GameObject loadButton = GameObject.Find(loadButtonString);
        targetFile = loadButton.transform.Find("SaveNumber").GetComponent<Text>().text;
        doFade = true;
    }

    public void LoadFile(string fileIndex)
    {
        Debug.Log("Called load file with index: " + fileIndex);
        
        List<string> fileLines = new List<string>();
        if (Directory.Exists(Path.Combine(savePath, fileIndex)))
        {
            string saveFile = Directory.GetFiles(Path.Combine(savePath, fileIndex))[0];
            using (FileStream fs = File.OpenRead(saveFile))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        fileLines.Add(sr.ReadLine());
                    }
                }
            }

            Hashtable psdEntries = new Hashtable();
            foreach (string entry in fileLines)
            {
                string entryName = entry.Split(':')[0].Trim();
                if (entry.Split(':')[1].Trim().ToLower().Equals("true") || entry.Split(':')[1].Trim().ToLower().Equals("false"))
                {
                    psdEntries.Add(entryName, System.Boolean.Parse(entry.Split(':')[1]));
                }
                else if (entry.Split(':')[1].Contains("."))
                {
                    psdEntries.Add(entryName, float.Parse(entry.Split(':')[1]));
                }
                else
                {
                    psdEntries.Add(entryName, int.Parse(entry.Split(':')[1]));
                }
            }
            ChangeStateConditions(psdEntries);
            doneLoad = true;
        }
    }

    void FadeChecker()
    {
        if (doFade)
        {

            if (loadingScreen == null)
            {
                Debug.Log("Retrieving loading screen");
                loadingScreen = GameObject.Find("Canvas").transform.Find("Loading Screen").gameObject;
            }
            if (blackFade == null)
            {
                Debug.Log("Retrieving black fade");
                blackFade = GameObject.Find("Canvas").transform.Find("Faded").gameObject.GetComponentInChildren<CanvasGroup>();
            }
            if (!blackFade.gameObject.activeSelf)
            {
                blackFade.gameObject.SetActive(true);
            }
            if (blackFade.alpha < 1 && !doneFade)
            {
                blackFade.alpha += fadeRate;
            }
            else
            {
                doneFade = true;
            }
            if (doneFade && !doLoad)
            {
                doLoad = true;
                LoadFile(targetFile);
            }
            else if (doneFade && !startedAsync && doneLoad)
            {
                loadingScreen.SetActive(true);
                delayInitial = Time.time;
                StartCoroutine(LoadAsynchronously((int)stateConditions["CurrentScene"]));
            }
        }
    }

    IEnumerator LoadAsynchronously(int sceneBuildNumber)
    {
        async = SceneManager.LoadSceneAsync(sceneBuildNumber);
        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        async.allowSceneActivation = false;
        startedAsync = true;
        while (!async.isDone)
        {
            //Debug.Log(async.progress);
            yield return null;
        }
    }
}
