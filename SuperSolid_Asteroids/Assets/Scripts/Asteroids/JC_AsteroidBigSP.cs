using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_AsteroidBigSP : JC_AsteroidSP
{
    // Bigger asteroid has health.
    private int mHealth = 2;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Reassign since it's also called in OnEnable.
        mHealth = 2;
        // Assign random velocity of movement at start.
        mVelocity = new Vector3(Random.insideUnitCircle.normalized.x, Random.insideUnitCircle.normalized.y, 0);
        // Pick a random angle for the rotation.
        mAngle = Random.Range(-2, 2);
    }

    private void OnEnable()
    {
        // When reactivating the asteroid call Start().
        Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Apply the velocity assigned at start.
        SetVelocity(mVelocity);
        // Move the asteroid.
        Move();
        // Rotate on itself with a random angle.
        Rotate();
    }

    private void OnCollisionEnter(Collision vCollision)
    {        
        // If the asteroid collider with a bullet.
        if (vCollision.gameObject.GetComponent<JC_BulletsSP>())
        {
            // Removes half of the Health.
            mHealth--;

            // When it's all depleated.
            if (mHealth == 0)
            {
                // Enable two new asteroids of smaller size.
                CreateAsteroidsOnCollision(JC_GameManager.PrefabID.AsteroidMedium, vCollision);
                // Set inactive.
                gameObject.SetActive(false);
            }

            // Increase the current player score.
            JC_GameManager._singleton.mPlayerScore += 10;
        }

        // If colliding with anything else other than the bullets.
        else
        {
            // Set inactive.
            gameObject.SetActive(false);
        }
    }
}