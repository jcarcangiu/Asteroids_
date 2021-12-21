using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JC_GameManager : MonoBehaviour
{
    public static JC_GameManager _singleton;

    private int mHighScore;
    private int mPlayerLives;

    [HideInInspector] public int mPlayerScore;
    [HideInInspector] public int mCurrLevel;

    private int mMaxLevel = 4;
    [Header("Start amount of astroids")]
    [SerializeField] private int mStartingAsteroids;

    // UI related variables.
    private Canvas mGameCanvas;
    private RectTransform mHUD;
    private Text mScore;
    private Text mHighestScore;
    private Text mLevelUI;
    private Image[] mLivesImages;
    private RectTransform mMainMenu;
    private Button mNewGameButton;

    // Player object.
    [HideInInspector] public GameObject mPlayer = null;
    [Header("Player Respawn Timer")]
    [SerializeField] private float mRespawnTimer = 2f;
    [Header("Player Spawn Position")]
    [SerializeField] private Vector3 mPlayerSpawnPosition = Vector3.zero;
    [Header("Player Spawn Rotation")]
    [SerializeField] private Vector3 mPlayerSpawnRotation = new Vector3(-180, 0, 0);

    [Header("Assign all the prefabs that need to be spawned")]
    public GameObject[] mSpawnablePrefabs;

    private void Awake()
    {
        if (_singleton == null)
        {
            // If the singleton is not assigned assign this to it.
            _singleton = this;
            // Don't reload the object when changing scene.
            DontDestroyOnLoad(gameObject);
        }

        if (_singleton != this)
        {
            // If something else that's not this is assigned to this variable destroy it.
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update.
    void Start()
    {
        // Assign all the UI varibles that belong to the HUD.
        InitialiseHUD();
        // Asign all the UI variables that belong to the Main Menu.
        InitialiseMainMenu();

        // Disable the HUD.
        OpenCloseHUD(false);

        // Start Game by opening the menu.
        SetState(GameState.Menu);
    }

    #region Spawning

    /// <summary>
    /// Prefab ID assigned to index of the mSpawnablePrefabs array.
    /// </summary>
    public enum PrefabID
    {
        Player,             // 0
        Bullet,             // 1
        AsteroidBig,        // 2
        AsteroidMedium,     // 3
        AsteroidSmall,      // 4
    }

    /// <summary>
    /// Spawns the correct object from the array mSpawnablePrefabs depending on their ID.
    /// </summary>
    /// <param name="vSpawnID"> PrefabID of the gameObject to be spawned. </param>
    /// <param name="vSpawnPosition"> The starting position where to spawn the gameObject. </param>
    /// <param name="vSpawnRotation"> The starting rotation where to spawn the gameObject. </param>
    /// <returns></returns>
    public static GameObject SpawnObjectWithID(PrefabID vSpawnID, Vector3 vSpawnPosition, Quaternion vSpawnRotation)
    {
        // Type cast the ID into an integer value.
        int tIntID = (int)vSpawnID;

        // Type casts the PrefabID enum into the array index, spawns at the assigned position and rotation.
        return JC_ObjectPooler.InstantiateFromPool(tIntID.ToString(), vSpawnPosition, vSpawnRotation);
    }

    /// <summary>
    /// Spawns the player and assigns the gameObjec to mPlayer.
    /// </summary>
    private void SpawnPlayer()
    {
        // If it hasn't been instantiated already.
        if (mPlayer == null)
            mPlayer = Instantiate(mSpawnablePrefabs[(int)PrefabID.Player], mPlayerSpawnPosition, Quaternion.Euler(mPlayerSpawnRotation));
    }

    /// <summary>
    /// Activates a set number of GameObjects from the Pool.
    /// </summary>
    /// <param name="vAmountOfAsteroids"></param>
    private void SpawnBigAsteroids(int vAmountOfAsteroids)
    {
        for (int i = 0; i < vAmountOfAsteroids; i++)
        {
            SpawnObjectWithID(PrefabID.AsteroidBig, RandomScreenPosition(), transform.rotation);
        }
    }

    /// <summary>
    /// Picks a random point in the screen.
    /// </summary>
    /// <returns></returns>
    private static Vector3 RandomScreenPosition()
    {
        // Get dimentions of the camera view.
        float tYSpawn = Camera.main.orthographicSize;
        float tXSpawn = tYSpawn * Camera.main.aspect;

        return new Vector3(Random.Range(-tXSpawn, tXSpawn), Random.Range(-tYSpawn, tYSpawn));
    }

    #endregion

    #region --- UI Management ---

    /// <summary>
    /// Assigns all the HUD related variables on the Canvas.
    /// </summary>
    public void InitialiseHUD()
    {
        // Find the only Canvas in the scene.
        mGameCanvas = FindObjectOfType<Canvas>();
        // Find the player HUD.
        mHUD = mGameCanvas.transform.Find("HUD").GetComponent<RectTransform>();
        // Find the HUD's Score.
        mScore = mHUD.transform.Find("Score").GetComponent<Text>();
        // Assign the images corrisponding to the lives.
        mLivesImages = mHUD.gameObject.GetComponentsInChildren<Image>();
        // Find the Text that displays the current level.
        mLevelUI = mHUD.Find("Level").GetComponent<Text>();
    }

    /// <summary>
    /// Assigns all the Main Menu related variables on the Canvas.
    /// </summary>
    public void InitialiseMainMenu()
    {
        // Find the Main Menu.
        mMainMenu = mGameCanvas.transform.Find("MainMenu").GetComponent<RectTransform>();
        // Find the Highest score Text.
        mHighestScore = mMainMenu.Find("HighScore").GetComponent<Text>();
        // Find the New game button.
        mNewGameButton = mMainMenu.transform.Find("NewGame").GetComponent<Button>();
        // Add a listener to on the onClick event.
        mNewGameButton.onClick.AddListener(NewGame);
    }

    /// <summary>
    /// Activates / deactivates the HUD.
    /// </summary>
    private void OpenCloseHUD(bool vOpen)
    {
        mHUD.gameObject.SetActive(vOpen);
    }

    /// <summary>
    /// Activates the Main menu canvas, whilst deactivating the HUB and viceversa.
    /// </summary>
    private void OpenCloseMenuAndHUD(bool vOpen)
    {
        // Reset all the lives icon to be active in the scene.
        foreach (Image img in mLivesImages)
        {
            // If they're not already active.
            if (!img.gameObject.activeInHierarchy)
                img.gameObject.SetActive(true);
        }

        // Open one while the other closes.
        mMainMenu.gameObject.SetActive(vOpen);
        mHUD.gameObject.SetActive(!vOpen);
    }

    #endregion

    #region --- GameStates ---

    /// <summary>
    /// Defines the various phases of the game loop logic.
    /// </summary>
    public enum GameState
    {
        None,
        Initialise,
        Menu,
        PrepareScene,
        Play,
        NextLevel,
    }

    // Initilise GameState variable to none.
    private GameState mCurrGameState = GameState.None;

    /// <summary>
    /// Switches between GameStates and runs the subsequent correct function.
    /// </summary>
    /// <param name="vNewGameState"> New GameState</param>
    public static void SetState(GameState vNewGameState)
    {
        if (_singleton.mCurrGameState != vNewGameState)
        {
            // Assign the new variable only if it's not the same as the previuos one.
            _singleton.mCurrGameState = vNewGameState;

            // When a switch of state happens reupdate the game loop.
            _singleton.ManageGameStates();
        }
    }

    /// <summary>
    /// Gets the current GameState.
    /// </summary>
    public static GameState GetGameState()
    {
        return _singleton.mCurrGameState;
    }

    /// <summary>
    /// Runs correct function depending on the GameState the game is currently in. Only Runs with SetState().
    /// </summary>
    private void ManageGameStates()
    {
        switch (GetGameState())
        {
            case GameState.Menu:
                // Open Main Menu.
                OpenCloseMenuAndHUD(true);
                //Add an event to the New Game button.
                break;

            case GameState.Initialise:
                Initialise();
                SetState(GameState.PrepareScene);
                break;

            case GameState.PrepareScene:
                StartCoroutine(PrepareScene(mCurrLevel, 0.5f));
                break;

            case GameState.Play:
                // Check Next Level every 10th of a second.
                StartCoroutine(CheckGameAreaEmpty());
                break;

            case GameState.NextLevel:
                StartCoroutine(PrepareScene(mCurrLevel, 0.5f));
                break;
        }
    }

    /// <summary>
    /// Reset to default values the current level and the amount of lives the player has.
    /// </summary>
    private void Initialise()
    {
        mCurrLevel = 0;
        mPlayerLives = 3;
        mPlayerScore = 0;
    }


    /// <summary>
    /// Run when New Game Button is pressed in the Main Menu.
    /// </summary>
    public void NewGame()
    {
        // Close Menu.
        if (mMainMenu.gameObject.activeInHierarchy)
            OpenCloseMenuAndHUD(false);

        // Reset the player stats.
        SetState(GameState.Initialise);
    }

    /// <summary>
    /// Reactivate the correct amount of gameOjects in the scene.
    /// </summary>
    private IEnumerator PrepareScene(int vCurrentLevel, float vDelay)
    {
        // Wheater it the first instance of this scene or a new level is being generate.
        while (GetGameState() == GameState.PrepareScene || GetGameState() == GameState.NextLevel)
        {
            // Wait for a little.
            yield return new WaitForSeconds(vDelay);

            // If this is the first time where're setting up the scene.
            if (GetGameState() == GameState.PrepareScene)
                // Reset all the lives icon to be active in the scene.
                foreach (Image img in mLivesImages)
                {
                    // If they're not already active.
                    if (!img.gameObject.activeInHierarchy)
                        img.gameObject.SetActive(true);
                }

            // If we've started the game or started again.
            if (vCurrentLevel == 0)
            {
                // Activate the Player.
                mPlayerLives = 3;

                // Check if the player has been Instantiated in the scene already or not, and spawn or respawn him.
                if (mPlayer == null)
                    SpawnPlayer();
                else
                    StartCoroutine(RespawnPlayer());                
            }

            // Activate the Big Asteroids.
            SpawnBigAsteroids(mStartingAsteroids * (mCurrLevel + 1));
            // Start main game loop logic.
            SetState(GameState.Play);
        }
    }

    /// <summary>
    /// Check if the player has detroyed all the Asteroids in the scene and if yes increase the level.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckGameAreaEmpty()
    {
        do
        {
            // While in play also update the Score UI.
            mScore.text = mPlayerScore.ToString();

            // Same for the Level UI.
            if (mLevelUI.text != mCurrLevel.ToString())
                mLevelUI.text = mCurrLevel.ToString();

            // Find all gameObjects of type JC_Asteroid.
            JC_AsteroidSP[] tAllAsteroids = FindObjectsOfType<JC_AsteroidSP>();

            // If there are none.
            if (tAllAsteroids.Length == 0)
            {
                // Until the last available level.
                if (mCurrLevel < mMaxLevel)
                {
                    // Increase the current level.
                    mCurrLevel++;
                    // Set Game State to switch to the next level.
                    SetState(GameState.NextLevel);
                }
            }

            // Do so only 10 times a second for performace.
            yield return new WaitForSeconds(0.1f);
        } while (GetGameState() == GameState.Play);
    }

    /// <summary>
    /// Deactivate all the objects in scene.
    /// </summary>
    private void ClearLevel()
    {
        //Find all objects that inherit from SimulatedPhysics.
        JC_SimulatedPhysics[] allActiveObjectInScene = FindObjectsOfType<JC_SimulatedPhysics>();

        // And set each one of them to be inactive.
        foreach (JC_SimulatedPhysics vActive in allActiveObjectInScene)
        {
            vActive.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Check if the player still has any lives left and if yes removes one.
    /// </summary>
    public void LooseLife()
    {
        // Remove one life from the player
        mPlayerLives--;
        // And momenteraly disable the player
        mPlayer.SetActive(false);
        
        // Depending on how many lives are left, remove the UI icon that represent them.
        if (mPlayerLives != 0)
            StartCoroutine(RespawnPlayer());

        if (mPlayerLives == 2)
            mLivesImages[0].gameObject.SetActive(false);

        if (mPlayerLives == 1)
            mLivesImages[1].gameObject.SetActive(false);

        // When it's Game Over.
        if (mPlayerLives == 0)
        { 
            // Remove last icon.
            mLivesImages[2].gameObject.SetActive(false);
            // Update the High Score;
            mHighestScore.text = "High Score: " + mPlayerScore;
            // Reset the level completely.
            ClearLevel();
            // And send back to the Main Menu.
            SetState(GameState.Menu);
        }
    }

    /// <summary>
    /// Respawn player after a set amount of time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnPlayer()
    {
        // If the player isn't active in the scene so he's dead.
        while (mPlayer.gameObject.activeInHierarchy == false)
        {
            // Wait for the respawn time.
            yield return new WaitForSeconds(mRespawnTimer);

            // Position the player in the starting position and rotation.
            mPlayer.transform.position = mPlayerSpawnPosition;
            mPlayer.transform.rotation = Quaternion.Euler(mPlayerSpawnRotation);

            // And reactivate.
            mPlayer.SetActive(true);
        }
    }

    #endregion
}