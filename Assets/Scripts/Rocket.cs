using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip finishSound;
    [SerializeField] ParticleSystem mainParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] float levelLoadDelay = 2f;

    Rigidbody rigidBody;
    AudioSource audioSource;
    
    enum State { Alive , Dying , Transcending}
    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
  
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (state != State.Alive)
        {
            return;  // ignore collisions when dead.
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //Do nothing;
                 break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDyingSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(finishSound);
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void StartDyingSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        explosionParticles.Play();
        Invoke("LoadFirstScene", levelLoadDelay);
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            Thrust();
        }
        else
        {
            audioSource.Stop();
            mainParticles.Stop();
        }
    }

    private void Thrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!mainParticles.isPlaying)
        {
            mainParticles.Play();
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}

