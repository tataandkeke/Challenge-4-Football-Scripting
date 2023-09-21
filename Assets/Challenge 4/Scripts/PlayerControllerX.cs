using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500.0f;
    private float boostSpeed = 1000.0f;
    private GameObject focalPoint;
    public ParticleSystem smokeParticle;

    public bool hasPowerup;
    public bool playerHasSpeed = false;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;
    public int speedDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
        smokeParticle.transform.position = transform.position + new Vector3(0, -0.6f, 0);

        //When the player presses Space they dash foward
        if (Input.GetKeyDown(KeyCode.Space) && playerHasSpeed == false)
        {
            playerHasSpeed = true;
            playerRb.AddForce(focalPoint.transform.forward * boostSpeed * Time.deltaTime, ForceMode.Impulse);
            smokeParticle.Play();
            StartCoroutine("SpeedTimer");
        }

    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine("PowerupCooldown");
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    //This is controls the duration and cooldown of speed boost
    IEnumerator SpeedTimer()
    {
        yield return new WaitForSeconds(2);
        playerHasSpeed = false;
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position; 
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }



}
