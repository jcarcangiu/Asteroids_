using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class contains all simluated physic methods, used by all gameObject with Physics in the scene.
/// </summary>
public class JC_SimulatedPhysics : MonoBehaviour
{
    // Maximum movement speed.
    [Header("Maximum speed that can be reached")]
    [SerializeField] protected float mMaxSpeed = 15f;

    // Maximum rotation speed.
    [Header("Maximum rotation that can be reached")]
    [SerializeField] protected float mRotationSpeed;
    protected float mAngle;

    // Current object velocity.
    protected Vector3 mVelocity = Vector3.zero;
    protected Vector3 mAppliedVelocity = Vector3.zero;
    // Projected position after velocity is applied.
    protected Vector3 mTargetPosition = Vector3.zero;

    // Start is called before the first frame update.
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame.
    protected virtual void Update()
    {

    }

    /// <summary>
    /// When the gameObject reaches the borders of the screen, teleported it to the opposite side of the screen.
    /// </summary>
    /// <param name="vCurrentPosition"> Current position of the gameObject> </param>
    /// <returns></returns>
    private Vector3 ScreenWrap(Vector3 vCurrentPosition)
    {
        // Grab half the height of the ortographic Camera view.
        float tYSpawn = Camera.main.orthographicSize - 10;
        // Grab the width by multiplaying by the aspect ratio.
        float tXSpawn = (tYSpawn + 10) * Camera.main.aspect;

        // Store the current position of the player;
        Vector3 tNextPosition = vCurrentPosition;
        
        // If the object goes out of right bounds.
        if (vCurrentPosition.x > tXSpawn)
            tNextPosition.x -= 2 * tXSpawn;

        // If the object goes out of left bounds.
        else if (vCurrentPosition.x < -tXSpawn)
            tNextPosition.x += 2 * tXSpawn;

        // If the object goes out of upper bounds.
        if (vCurrentPosition.y > tXSpawn)
            tNextPosition.y -= 2 * tXSpawn;

        // If the object goes out of lower bounds.
        else if (vCurrentPosition.y < -tXSpawn)
            tNextPosition.y += 2 * tXSpawn;

        tNextPosition.z = 0;

        // Return the new position the object will be teleported to.
        return tNextPosition;
    }

    /// <summary>
    /// Lerps the position of the objects to a target position.
    /// </summary>
    protected virtual void Move()
    {
        // Always wrap the player position.
        transform.position = ScreenWrap(transform.position);

        // Add velocity to the player position.
        mTargetPosition = transform.position + mVelocity;
        mTargetPosition = new Vector3(mTargetPosition.x, mTargetPosition.y, 0);

        // Lerp current position to the next position.
        transform.position = Vector3.Lerp(transform.position, mTargetPosition, Time.deltaTime);
    }

    /// <summary>
    /// Calculates the velocity to apply to the gameObject.
    /// </summary>
    /// <param name="vDirection"> Direction of movement. </param>
    protected virtual void SetVelocity(Vector3 vDirection)
    {
        // Calculate force amount and apply direction.
        mAppliedVelocity = vDirection * mMaxSpeed;
        mAppliedVelocity = new Vector3(mAppliedVelocity.x, mAppliedVelocity.y, 0);

        // Add to the objects velocity.
        mVelocity += mAppliedVelocity;
        // Clamp value to the maximum speed of the object.
        mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxSpeed);
    }

    /// <summary>
    /// Slerps object rotation with parameters.
    /// </summary>
    protected virtual void Rotate()
    {
        float tAngle = mAngle * Time.deltaTime * mRotationSpeed;
        transform.rotation *= Quaternion.Euler(tAngle, tAngle, tAngle);
    }
}
