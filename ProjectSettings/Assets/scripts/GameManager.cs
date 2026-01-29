using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GamePhase { UI, Arrow, Ball, Grade, Score, Wait }
    public GamePhase phase = GamePhase.UI;

    private GameObject temp;
    public CameraMovement mainCamera;
    public ArrowMovement arrow;
    public BallMovement ball;
    public GameObject[] pins;

    public GameObject gradeObj;
    public Text gradeText;
    public Text nameText;
    private Text winningText;


    private int round = 0;
    private bool secondThrow = false;
    private int prev = 0;
    private int playerNum = 0;
    private List<string> fallPins = new List<string>();
    private List<int[]> players = new List<int[]>();
    private List<string> playerNames = new List<string>();

    private bool scoreActive = false;
    private Text scoreTablet;
    public GameObject panel;
    
    public GameObject startMenuObj;
    public GameObject startButtonObj;
    public GameObject addButtonObj;
    public GameObject exitButtonObj;
    public Button exitButton;
    public Button startButton;
    public Button addButton;
    public bool addButtonActive;
    public InputField inputPlayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Nasłuchujemy na zmianę sceny
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Szukamy obiektu w nowej scenie
        temp = GameObject.Find("Main Camera");
        mainCamera = temp.GetComponent<CameraMovement>();
        temp = GameObject.Find("Arrow");
        mainCamera.arrow = temp.transform;
        arrow = temp.GetComponent<ArrowMovement>();
        temp = GameObject.Find("BowlingBall");
        mainCamera.ball = temp.transform;
        ball = temp.GetComponent<BallMovement>();
        temp = GameObject.Find("PinBox");
        mainCamera.pinBox = temp.transform;
        for (int i = 0; i <= 9; i++)
        {
            pins[i] = GameObject.Find("BowlingPin (" + i + ")");
        }
        gradeObj = GameObject.Find("GradeText");
        gradeText = gradeObj.GetComponent<Text>();
        temp = GameObject.Find("NameText");
        nameText = temp.GetComponent<Text>();
        panel = GameObject.Find("Panel");
        temp = GameObject.Find("ScoreTablet");
        scoreTablet = temp.GetComponent<Text>();
        temp = GameObject.Find("WinningText");
        winningText = temp.GetComponent<Text>();

        startMenuObj = GameObject.Find("StartMenu");
        startButtonObj = GameObject.Find("StartButton");
        addButtonObj = GameObject.Find("AddButton");
        startButton = startButtonObj.GetComponent<Button>();
        addButton = addButtonObj.GetComponent<Button>();
        temp = GameObject.Find("InputPlayer");
        inputPlayer = temp.GetComponent<InputField>();
        exitButtonObj = GameObject.Find("ExitButton");
        exitButton = exitButtonObj.GetComponent<Button>();

        mainCamera.snapArrow();
        gradeObj.SetActive(false);
        panel.SetActive(false);
        startMenuObj.SetActive(false);
        scoreActive = false;

        exitButtonObj.SetActive(true);
        exitButton.onClick.AddListener(() => exitGame());

        if (phase != GamePhase.UI)
        {
            UpdateName();
            UpdateScore();
            UpdatePins();
        }
        Debug.Log("awake");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("start");
        phase = GamePhase.UI;

        addButtonActive = true;
        startMenuObj.SetActive(true);
        startButtonObj.SetActive(false);
        //startMenuObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //Debug.Log("pressed");
            if (scoreActive)
            {
                panel.SetActive(false);
                scoreActive = false;
            }
            else
            {
                panel.SetActive(true);
                scoreActive = true;
            }
        }

        if (addButtonActive && Keyboard.current.enterKey.wasReleasedThisFrame)
        {
            AddPlayer();
            inputPlayer.ActivateInputField();
        }

        switch (phase)
        {
            case GamePhase.Arrow:
                if (arrow.mode == ArrowMovement.Mode.disabled)
                {
                    mainCamera.snapArrow();
                    arrow.enableArrow();
                }
                else if (arrow.mode == ArrowMovement.Mode.enabled)
                {
                    if (Keyboard.current.spaceKey.isPressed)
                    {
                        arrow.waitArrow(2f);
                    }
                }
                else if (arrow.mode == ArrowMovement.Mode.after)
                {
                    arrow.disableArrow();
                    phase = GamePhase.Ball;
                }
                break;

            case GamePhase.Ball:
                if (ball.mode == BallMovement.Mode.disabled)
                {
                    mainCamera.snapBall();
                    ball.enableBall(arrow.getDirection(), arrow.getForce());
                }
                else if (ball.mode == BallMovement.Mode.enabled)
                {
                    //pins.PinCount();
                    if (ball.getPosX() > 9f)
                    {
                        mainCamera.snapPinBox();
                        ball.waitBall(2f);
                    }
                    // warunek kiedy sie konczy
                }
                else if (ball.mode == BallMovement.Mode.after)
                {
                    // po oczekiwaniu
                    ball.disableBall();
                    phase = GamePhase.Grade;
                }
                break;
            case GamePhase.Grade:
                int grade = PinCount();
                if (!secondThrow) players[playerNum][round] = grade;
                else players[playerNum][round + 1] = grade;

                if (grade == 10 && !secondThrow) gradeText.text = "STRIKE!";
                else if (grade + prev == 10) gradeText.text = "SPARE";
                else if (grade == 0) gradeText.text = "LOSER";
                else gradeText.text = grade.ToString();

                gradeObj.SetActive(true);
                Invoke("nextPlayer", 2f);
                phase = GamePhase.Wait;
                break;
            case GamePhase.UI:
                addButton.onClick.AddListener(() => AddPlayer());
                startButton.onClick.AddListener(() => StartGame());
                Invoke("inputFocus", 0.0001f);
                phase = GamePhase.Wait;
                break;
        }
    }

    private void exitGame()
    {
        Application.Quit();
    }

    private void inputFocus()
    {
        inputPlayer.ActivateInputField();
    }
    
    private void AddPlayer()
    {
        if (inputPlayer.text != "")
        {
            playerNames.Add(inputPlayer.text);
            startButtonObj.SetActive(true);
        }
        inputPlayer.text = "";
    }

    private void StartGame()
    {
        for (int i = 0; i < playerNames.Count; i++)
        {
            int[] newArray = new int[20]; // wszystkie elementy domyślnie 0
            players.Add(newArray);
        }
        startMenuObj.SetActive(false);
        addButtonActive = false;
        phase = GamePhase.Arrow;
        SceneReset();
    }

    private int PinCount()
    {
        int ans = 0;
        fallPins.Clear();
        foreach (GameObject pin in pins)
        {
            Vector3 position = pin.transform.position + pin.transform.forward;
            if (position.y < 1.245)
            {
                ans++;
                fallPins.Add(pin.name);
            }
        }
        UpdateScore();
        return ans;
    }

    private void nextPlayer()
    {
        if (secondThrow) // drugi rzut
        {
            playerNum++;
            if (playerNum >= players.Count)
            {
                playerNum = 0;
                round += 2;
            }
            UpdateName();
            prev = 0;
            secondThrow = false;
            fallPins.Clear();
        }
        else // pierwszy rzut
        {
            prev = players[playerNum][round];
            if (players[playerNum][round] == 10) // byl strike
            {
                playerNum++;
                if (playerNum >= players.Count)
                {
                    playerNum = 0;
                    round += 2;
                }
                UpdateName();
                prev = 0;
                fallPins.Clear();
                secondThrow = false;
            }
            else secondThrow = true;
        }

        if (round > 18)
        {
            GameEnd();
            return;
        }


        SceneReset();
    }
    
    private void GameEnd()
    {
        phase = GamePhase.Wait;
        panel.SetActive(true);
        int mx = -1;
        int mxid = 0;
        for(int i=0; i<players.Count; i++)
        {
            if(mx < SumScore(i))
            {
                mx = SumScore(i);
                mxid = i;
            }
        }
        winningText.text = playerNames[mxid] + " won!";
    }

    private void UpdateName()
    {
        if (playerNum + 1 > playerNames.Count) return;
        nameText.text = playerNames[playerNum];
    }

    private void SceneReset()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        phase = GamePhase.Arrow;
    }

    private void UpdatePins()
    {
        foreach (string name in fallPins)
        {
            foreach (GameObject pin in pins)
            {
                if (pin.name == name) pin.SetActive(false);
            }
        }
    }

    private int SumScore(int num)
    {
        if (num + 1 > players.Count) return 0;
        //Debug.Log(players.Count);
        int ans = 0;
        for (int i = 0; i <= 18; i += 2)
        {
            if (players[num][i] == 10) // strike
            {
                if (i + 2 <= 19)
                {
                    if (players[num][i + 2] == 10)
                    {
                        if (i + 4 <= 19) ans += players[num][i + 4];
                        else ans += 10;
                    }
                    else
                    {
                        ans += players[num][i + 2] + players[num][i + 3];
                    }
                }
                else ans += 20;
                ans += 10;
            }
            else if (players[num][i] + players[num][i + 1] == 10) // spare
            {
                if (i + 2 <= 19) ans += players[num][i + 2];
                else ans += 10;
                ans += 10;
            }
            else
            {
                ans += players[num][i] + players[num][i + 1];
            }
        }
        
        return ans;
    }
    
    private void UpdateScore()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(" PLAYERS             1    2    3    4    5    6    7    8    9    10    SCORE\n");
        //scoreTablet.text += "    5    5    5    5\n";
        int num = 0;
        if (num + 1 > players.Count) return;
        foreach (string name in playerNames)
        {
            if (name.Length + 2 > 20)
            {
                for (int j = 0; j < 15; j++) sb.Append(name[j]);
                sb.Append("...: ");
            }
            else
            {
                sb.Append(name + ": ");
                for (int j = name.Length; j < 18; j++) sb.Append(" ");
            }

            for (int i = 0; i <= round; i += 2)
            {
                if (players[num][i] == 10) sb.Append(" X   ");
                else if (players[num][i] + players[num][i + 1] == 10) sb.Append(players[num][i] + " /  ");
                else sb.Append(players[num][i] + " " + players[num][i + 1] + "  ");
            }

            for (int i = 0; i < 9-round/2; i++) sb.Append("     ");
            sb.Append("   " + SumScore(num) + "\n");
            num++;
        }
        scoreTablet.text = sb.ToString();
    }

}
