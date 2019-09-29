using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 startPosition;
    private Rigidbody2D rigidbody2D;
    public float moveSpeed = 5f;
    public float pullForce = 100f;
    public float rotateSpeed = 360f;
    private GameObject closestTower;
    private GameObject hookedTower;
    private bool isPulled = false;
    private ScenesManager uiControl;
    private AudioSource myAudio;
    private bool isCrashed = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        myAudio = this.gameObject.GetComponent<AudioSource>();
        uiControl = GameObject.Find("Canvas").GetComponent<ScenesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move the object
        rigidbody2D.velocity = -transform.up * moveSpeed;
        if (Input.GetKey(KeyCode.Z) && !isPulled)
        {
            if (closestTower != null && hookedTower == null)
            {
                hookedTower = closestTower;
            }
            if (hookedTower)
            {
                Debug.Log("Pull!");
                float distance = Vector2.Distance(transform.position, hookedTower.transform.position);
                //Gravitation toward tower
                Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                rigidbody2D.AddForce(pullDirection * newPullForce);

                //Angular velocity
                rigidbody2D.angularVelocity = -rotateSpeed / distance;
                isPulled = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            isPulled = false;
        }

        if (isCrashed)
        {
            if (!myAudio.isPlaying)
            {
                //Restart scene
                restartPosition();
                uiControl.restartGame();
            }
        }
        // else
        // {
        //     //Move the object
        //     rigidbody2D.velocity = -transform.up * moveSpeed;
        //     rigidbody2D.angularVelocity = 0f;
        // }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //Hide game object
            // this.gameObject.SetActive(false);
            //Play SFX
            if (!isCrashed)
            {
                myAudio.Play();
                rigidbody2D.velocity = new Vector3(0f, 0f, 0f);
                rigidbody2D.angularVelocity = 0f;
                isCrashed = true;
            }
            // uiControl.restartGame();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Levelclear!");
            uiControl.endGame();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            closestTower = collision.gameObject;

            //Change tower color back to green as indicator
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled) return;

        if (collision.gameObject.tag == "Tower")
        {
            closestTower = null;

            //Change tower color back to normal
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void restartPosition()
    {
        //Set to start position
        this.transform.position = startPosition;

        //Restart rotation
        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        //Set isCrashed to false
        isCrashed = false;

        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }

    }
}
