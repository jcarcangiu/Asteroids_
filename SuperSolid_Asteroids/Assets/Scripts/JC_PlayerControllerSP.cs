using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_PlayerControllerSP : JC_SimulatedPhysics
{
    private Vector3 mKeyboardInput;
    private float horizInput;
    private float vertInput;

    // Update is called once per frame
    protected override void Update()
    {
        // Always apply movement;
        Move();
    }

    private void OnEnable()
    {
        // Stop the player from registering velocity prior to respawning.
        mVelocity = Vector3.zero;
        // Make sure no vector is applied to movement now.
        SetVelocity(mVelocity);
    }

    protected override void Move()
    {
        // Rotate the Player with the mouse. 
        Rotate();

        // Grab Keyboard Input.
        horizInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        // Store keyboard input in a separate Vector.
        mKeyboardInput = new Vector3(horizInput, vertInput, 0);

        // Get movement direction from keyboard input.
        SetVelocity(mKeyboardInput);

        // Call base class to apply velocity to the transform.
        base.Move();
    }

    protected override void SetVelocity(Vector3 vDirection)
    {
        // Calculate force amount and apply direction.
        mAppliedVelocity = vDirection * mMaxSpeed;
        // Add to the objects velocity.
        mVelocity += mAppliedVelocity;
        // Clamp value to the maximum speed of the object.
        mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxSpeed + 5);
    }

    protected override void Rotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, GetMouseOnScreen(), Time.deltaTime * mRotationSpeed);
    }

    /// <summary>
    /// Get a world coordinate reference of the mouse position on the screen.
    /// </summary>
    /// <returns></returns>
    private Quaternion GetMouseOnScreen()
    {
        // Store mouse position in world point.
        Vector3 tMousePos = new Vector3(GetMousePosition().x, GetMousePosition().y, 0);
        // Current player position.
        Vector3 tPlayerPos = new Vector3(transform.position.x, transform.position.y, 0);
        // Resulting angle between mouse pos and player pos.
        float tResultingAngle = GetRotationAngle(tPlayerPos, tMousePos - transform.position) - 90;

        // Set current Z of rotation to the resulting angle.
        return Quaternion.Euler(transform.rotation.x, transform.rotation.y, tResultingAngle);
    }

    /// <summary>
    /// Return resulting angle between two points in space.
    /// </summary>
    private float GetRotationAngle(Vector3 vPointA, Vector3 vPointB)
    {
        return Mathf.Atan2(vPointA.y - vPointB.y, vPointA.x - vPointB.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Get position of the mouse on the screen in world coordinates.
    /// </summary>
    private Vector3 GetMousePosition()
    {
        // Get the current position of the mouse on the screen.
        Vector3 tMouseInput = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        // Convert into world point.
        Vector3 tMousePos = Camera.main.ScreenToWorldPoint(tMouseInput);

        return tMousePos;
    }

    // Detect if the Player has collided with an Asteroid
    private void OnCollisionEnter(Collision vCollision)
    {
        // Unless the object the player has collided with is it's own bullet, remove a life an deactivate.
        if (!vCollision.gameObject.GetComponent<JC_BulletsSP>())
        {
            // Disable the player from the Game Manager.
            JC_GameManager._singleton.LooseLife();

            // Make so no more velocity is applied.
            mVelocity = Vector3.zero;
            SetVelocity(mVelocity);
        }
    }
}