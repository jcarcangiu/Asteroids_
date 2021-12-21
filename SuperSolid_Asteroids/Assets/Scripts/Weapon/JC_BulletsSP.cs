using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_BulletsSP : JC_SimulatedPhysics
{
    [Header("Timer to disable the Bullet.")]
    [SerializeField] private float mDisableObjectTimer = 3f;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Start coroutine that removes bullet from the scene after a period.
        StartCoroutine(DisableGamobject());
    }

    private void OnEnable()
    {
        // When reactivating buller call Start().
        Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Shoot from the Weapon gameObject forward to the player.
        SetVelocity(-transform.up);
        // Move the bullet in that direction.
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Deactivate gameObject on collision with anything.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Disable the bullet gameObject after a certain time it's been in scene.
    /// </summary>
    private IEnumerator DisableGamobject()
    {
        while (true)
        {
            // Wait until a while before deactivating from the scene.
            yield return new WaitForSeconds(mDisableObjectTimer);
            gameObject.SetActive(false);
        }
    }
}
