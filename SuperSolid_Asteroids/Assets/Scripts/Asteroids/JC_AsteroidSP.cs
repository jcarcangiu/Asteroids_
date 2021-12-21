using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_AsteroidSP : JC_SimulatedPhysics
{
    //Give each asteroid it's own value.
    protected Vector3 mDirection;

    protected virtual void Awake()
    {
        // Initialise random generator.
        Random.InitState(gameObject.GetInstanceID());
    }

    /// <summary>
    /// Creates two new asteroids when destroyed by bullets.
    /// </summary>
    protected void CreateAsteroidsOnCollision(JC_GameManager.PrefabID vPrefabID, Collision vCollision)
    {
        // Calculate the angle of the collision.
        Vector3 tDirection = vCollision.contacts[0].point - transform.position;

        // Grab the opposite normalize vector to the angle of the collision.
        tDirection = -tDirection.normalized * Mathf.Cos(30);
        tDirection = new Vector3(tDirection.x, tDirection.y, 0);

        // Depending on the ID the the object to be spawned.
        switch (vPrefabID)
        {
            case JC_GameManager.PrefabID.AsteroidMedium:
                
                // Generate first asteroid in one direction and the other one in a mirrored direction.
                GameObject tFirstMediumAsteroid = JC_GameManager.SpawnObjectWithID(vPrefabID, transform.position, transform.rotation);
                tFirstMediumAsteroid.GetComponent<JC_AsteroidMediumSP>().mDirection = tDirection;

                GameObject tSecondMediumAsteroid = JC_GameManager.SpawnObjectWithID(vPrefabID, transform.position, transform.rotation);
                tSecondMediumAsteroid.GetComponent<JC_AsteroidMediumSP>().mDirection = Vector3.Reflect(-tDirection, -transform.position.normalized);

                break;

            case JC_GameManager.PrefabID.AsteroidSmall:

                // Generate first asteroid in one direction and the other one in a mirrored direction.
                GameObject tFirstSmallAsteroid = JC_GameManager.SpawnObjectWithID(vPrefabID, transform.position, transform.rotation);
                tFirstSmallAsteroid.GetComponent<JC_AsteroidSmallSP>().mDirection = tDirection;

                GameObject tSecondSmallAsteroid = JC_GameManager.SpawnObjectWithID(vPrefabID, transform.position, transform.rotation);
                tSecondSmallAsteroid.GetComponent<JC_AsteroidSmallSP>().mDirection = Vector3.Reflect(-tDirection, -transform.position.normalized);

                break;
        }
    }
}
