using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharController : MonoBehaviour {

    public Transform rayStart;
    public GameObject crystalEffect;

    private Rigidbody rb;
    private bool walkingRight = true;
    private Animator anim;
    private GameManager gameManager;
    private float gameSpeed = 2.5f;
    public AudioClip hitEffect;
    private bool gamePaused = false;

    private AudioSource audioSource;

    // Use this for initialization
    void Awake() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            gamePaused = true;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            gamePaused = false;
        }
    }

    private void FixedUpdate()
    {

        if (!gameManager.gameStarted) {
            return;
        } else {
            anim.SetTrigger("gameStarted");
        }

        rb.transform.position = transform.position + transform.forward * gameSpeed * Time.deltaTime;
        gameSpeed *= 1.0001f;
    }


    // Update is called once per frame
    void Update() 
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Input.GetMouseButtonDown(0) && (gamePaused == false) && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Switch();
            }
        }

        RaycastHit hit;

        if (!Physics.Raycast(rayStart.position, -transform.up, out hit, Mathf.Infinity)) {
            anim.SetTrigger("isFalling");
            Debug.Log("Falling");

        } else {
            anim.SetTrigger("notFallingAnymore");
        }

        if (transform.position.y < -2) {
            gameManager.EndGame();
        }

    }

    private void Switch() {
        if (!gameManager.gameStarted) {
            return;
        }

        walkingRight = !walkingRight;

        if (walkingRight)
            transform.rotation = Quaternion.Euler(0, 45, 0);
        else
            transform.rotation = Quaternion.Euler(0, -45, 0);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crystal") {

            gameManager.IncreaseScore();

            GameObject g = Instantiate(crystalEffect, rayStart.transform.position, Quaternion.identity);
            Destroy(g, 2);
            Destroy(other.gameObject);

            audioSource.PlayOneShot(hitEffect);

        }
    }

}
