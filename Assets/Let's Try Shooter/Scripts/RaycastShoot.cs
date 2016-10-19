using UnityEngine;
using System.Collections;

public class RaycastShoot : MonoBehaviour {

    public int gunDamage = 1;
    public float fireRate = .25f;
    public float waeponRange = 50f;
    public float hitForce = 100f;
    public Transform gunEnd; // be positioned just in front of the barrel or ‘end’ of the gun

    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
    //how long we want the laser to remain visible in the Game View for, once the player has fired.
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    //The LineRenderer takes an array of 2 or more points in 3D space and draws a straight line between them in the game view
    private float nextFire;
    //nextFire will hold the time at which the player will be allowed to fire again after firing.

    void Start ()
    {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();

        fpsCam = GetComponentInParent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
            //The bottom left of the viewport is defined as 0,0. The top right is defined as 1,1.
            RaycastHit hit;
            laserLine.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, waeponRange))
            {
                laserLine.SetPosition(1, hit.point);
                ShootableBox health = hit.collider.GetComponent<ShootableBox>();
                if (health != null)
                {
                    health.Damage(gunDamage); //declared in shootable script
                }
                if (hit.rigidbody != null)

                {
                    //allows us to control how much force should be added when we shoot an object.
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                    // outward direction of the surface we hit
                }
            }
            else
            {
                laserLine.SetPosition(1, fpsCam.transform.forward * waeponRange);
            }

        }
        
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
    }
}
