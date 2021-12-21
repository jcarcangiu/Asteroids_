using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JC_Weapons : MonoBehaviour
{
    [Header("Weapon Cooldown")]
    [SerializeField] private float mCooldownTime = 0.5f;
    public float mTimer  = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            // Activate the timer whilst shooting.
            mTimer += Time.deltaTime;
            Shoot();   
        }

        if (mTimer > mCooldownTime)
        {
            // Reset the timer.
            mTimer = 0;
        }
    }

    /// <summary>
    /// Shoots at an interval a bullet prefab in the direction pointed by the mouse.
    /// </summary>
    private void Shoot()
    {        
        // When the timer is over.
        if (mTimer > mCooldownTime)
        {
            //Instantiate the bullet and grab the bullet component.
            JC_BulletsSP tBullet = JC_GameManager.SpawnObjectWithID(JC_GameManager.PrefabID.Bullet, transform.position, transform.rotation).GetComponent<JC_BulletsSP>();
            // Reset timer again.
            mTimer = 0;
        }    
    }
}