using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 inputVektor = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));
        inputVektor = inputVektor.normalized;

        Vector3 movementVektor = inputVektor * speed * Time.fixedDeltaTime;
        rb.MovePosition(movementVektor + transform.position);

    }
}
