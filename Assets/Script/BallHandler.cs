using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject ball;
    [SerializeField] private Rigidbody2D pivot;

    private Rigidbody2D currentBallRigidBody;
    private SpringJoint2D currentBallSpringJoin;
    private float detachBallTime = 0.15f;
    private float respawnTime = 2f;
    private bool isDragging;
    private GameObject ballInstance;

    // Start is called before the first frame update
    void Start()
    {
        SpawnNewBall();   
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBallRigidBody == null) { return; }
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }
            return;
        }

        isDragging = true;

        currentBallRigidBody.isKinematic = true;

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidBody.position = worldPosition;

      

    }

    private void SpawnNewBall()
    {
        ballInstance = Instantiate(ball, pivot.position, Quaternion.identity);
        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoin = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoin.connectedBody = pivot;
    }
    void LaunchBall()
    {
        currentBallRigidBody.isKinematic = false;
        currentBallRigidBody = null;
        isDragging = false;

        Invoke(nameof(DetachBall), detachBallTime);
        
    }

    private void DetachBall()
    {
        currentBallSpringJoin.enabled = false;
        currentBallSpringJoin = null;
        Invoke(nameof(DestroyBall), 1.5f);
        Invoke(nameof(SpawnNewBall), respawnTime);
        
    }
    private void DestroyBall()
    {
        Destroy(ballInstance);
    }
}
