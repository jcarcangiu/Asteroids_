using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    // Create a pool class that holds the Key, the ID and the size of the pool.
    public class Pool
    {
        public string mKey;
        public JC_GameManager.PrefabID mPrefab;
        public int mSize;
    }

    // List of Pools for each type of object to Instantiate.
    public List<Pool> mPoolList = new List<Pool>();
    // Dictionary of Spawnable gameObject accesible anywhere.
    static Dictionary<string, Queue<GameObject>> mPoolDictionary = new Dictionary<string, Queue<GameObject>>();

    public static JC_ObjectPooler _singleton;

    // Start is called before the first frame update
    private void Awake()
    {
        if (_singleton == null)
        {
            // If the singleton is not assigned assign this to it.
            _singleton = this;

            // Clear Dictionary to not have conflicts between new keys.
            mPoolDictionary.Clear();

            // Don't reload the object when changing scene.
            DontDestroyOnLoad(gameObject);
        }

        if (_singleton != this)
        {
            // If something else that's not this is assigned to this variable destroy it.
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Scroll through all the items in the Pool list.
        foreach (Pool pool in mPoolList)
        {
            // Create a new Queue for each one;
            Queue<GameObject> tObjectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.mSize; i++)
            {
                // Double check that we're not trying to access a prefab that's been unassigned in the GameManager inspector.
                if (JC_GameManager._singleton.mSpawnablePrefabs[(int)pool.mPrefab] == null)
                {
                    Debug.LogError("Object corrisponding to key is not in the Array of spawnable prefabs in the GameManager");
                    return;
                }

                // And for each item in the queue instantiate a new gameObject from the GameManager;
                GameObject tObj = Instantiate(JC_GameManager._singleton.mSpawnablePrefabs[(int)pool.mPrefab], transform.position, transform.rotation);
                // Deactivate in the scene.
                tObj.SetActive(false);
                // Assign object to the end of the queue.
                tObjectPool.Enqueue(tObj);
            }

            // Add the gameObject to the dictionary.
            mPoolDictionary.Add(pool.mKey, tObjectPool);
        }
    }

    /// <summary>
    /// Activate Objects from the Pool, by finding it in the dictionary.
    /// </summary>
    /// <param name="vKey"> Dictionary Pool item key. </param>
    public static GameObject InstantiateFromPool(string vKey, Vector3 vPosition, Quaternion vRotation)
    {
        if (mPoolDictionary.ContainsKey(vKey))
        {
            // Remove item from the beginning of the queue.
            GameObject tObjToSpawn = mPoolDictionary[vKey].Dequeue();

            // Set active in the current scene.
            tObjToSpawn.SetActive(true);

            // Set Position and rotation of gameObject.
            tObjToSpawn.transform.position = vPosition;
            tObjToSpawn.transform.rotation = vRotation;

            // Requeue in the Dictionary;
            mPoolDictionary[vKey].Enqueue(tObjToSpawn);

            // Return the correct gameObject to reactivate the scene.
            return tObjToSpawn;
        }

        else
        {
            return null;
        }
    }
}
