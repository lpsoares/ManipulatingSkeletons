using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CollideRim : MonoBehaviour
{

    Vector3 originalPos;

    public Material MaterialBase;
    public Material MaterialError;

    AudioSource audioSource;
    MeshRenderer mr;

    List<Vector3> playerPos = new List<Vector3>();

    bool record = false;

    void Start()
    {
        originalPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        audioSource = GetComponent<AudioSource>();
        mr = GetComponent<MeshRenderer>();
    }

    void Update() {
        
        if(record) {
            playerPos.Add(gameObject.transform.position);
        }

        // Cas cube is free falling
        if(gameObject.transform.position.y < 0.1) {
            ResetPosition();
            record = false;
            playerPos.Clear();
        }
    }

    GameObject new_line; 

    void OnCollisionEnter(Collision collision) {

        // Case cube colides with rim
        if(collision.gameObject.tag == "Rim"){
            if (collision.relativeVelocity.magnitude > 2) {
                audioSource.Play();
            }
            //mr.material = MaterialError;
            //collision.gameObject.GetComponent<MeshRenderer>().material = MaterialError;
            StartCoroutine(InvokeDelayed(3, collision.gameObject));
        }

        // Case cube colides with finish
        if(collision.gameObject.tag == "Finish"){

            
            record = false;
            
            //Color red = Color.red;
            Color tmp_color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            new_line = new GameObject("New Line");

            LineRenderer lineRenderer = new_line.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = tmp_color;
            lineRenderer.endColor = tmp_color;
            //lineRenderer.SetColors(tmp_color, tmp_color);
            
            //lineRenderer.SetWidth(0.003f, 0.003f);
            lineRenderer.startWidth = 0.003f;
            lineRenderer.endWidth = 0.003f;

            //lineRenderer.SetVertexCount(playerPos.Count);
            lineRenderer.positionCount = playerPos.Count;
            for (int i = 0; i < playerPos.Count; i++ )
                lineRenderer.SetPosition(i, playerPos[i]);
            
            // TESTAR TESTAR
            //lineRenderer.SetPositions(playerPos);
            
            //Add Components (not sure yet if all are needed)
            //Rigidbody rb = new_line.AddComponent<Rigidbody>();
            //rb.useGravity = true;
            //rb.isKinematic = true;
            //new_line.AddComponent<MeshFilter>();
            //BoxCollider bc = new_line.AddComponent<BoxCollider>();
            //bc.size = new Vector3(0.5f, 0.5f, 0.2f);

            //new_line.AddComponent<MeshRenderer>();
            //XRGrabInteractable xi = new_line.AddComponent<XRGrabInteractable>();

            ResetPosition();
            //Destroy(gameObject);

        }

    }

    // Case the Cube is out of the base
    void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Respawn"){
            playerPos.Clear();
            //Destroy(GetComponent<LineRenderer>());
            Destroy(new_line);
            record = true; // Start recording
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Rim")) { 
    //         // if (collision.relativeVelocity.magnitude > 2)
    //         //     audioSource.Play(); 
    //         mr.material = MaterialError;
    //         StartCoroutine(InvokeDelayed(3));
    //     }
    // }

    private IEnumerator InvokeDelayed(float delay, GameObject collision) {
        collision.GetComponent<MeshRenderer>().material = MaterialError;
        yield return new WaitForSeconds(delay);
        collision.GetComponent<MeshRenderer>().material = MaterialBase;
    }


    void ResetPosition() {
        // Release the object
        XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();
        interactable.interactionManager.CancelInteractableSelection(interactable);
        //Use CancelInteractableSelection(IXRSelectInteractable) instead.'

        // Reset speed, rotation and position
        //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.position = originalPos;
    }
}
