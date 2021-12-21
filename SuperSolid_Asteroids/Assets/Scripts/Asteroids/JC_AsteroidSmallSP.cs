using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_AsteroidSmallSP : JC_AsteroidSP
{
    // Start is called before the first frame update
    protected override void Start()
    {
        // Assign the velocity of movement at start, mDirection is given by asteroid that generated it.
        mVelocity = mDirection;
        // Pick a random angle for the rotation.
        mAngle = Random.Range(-7.5f, 7.5f);
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
        SetVelocity(mDirection);
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
            // Deactivate Object.
            gameObject.SetActive(false);

            // Increase the current player score.
            JC_GameManager._singleton.mPlayerScore += 10;
        }

        // If colliding with anything else other than the bullets.
        else 
            // Set inactive.
            gameObject.SetActive(false);
    }
}