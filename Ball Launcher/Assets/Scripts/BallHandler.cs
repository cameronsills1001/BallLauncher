using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float respawnDelay;
    [SerializeField] private float detachDelay;

    private Camera mainCamera; //Camera has built in methods to convert touch space to world space
    private bool isDragging;
    private Rigidbody2D currentBallRigidBody;
    private SpringJoint2D currentBallSpringJoint;
    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidBody == null) { return; }

        if (Touch.activeFingers.Count == 0)
        {
            if(isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            
            return; //if the screen isn't being touched it should just return
        }

        isDragging = true;
        currentBallRigidBody.isKinematic = true;

        //multi touch
        Vector2 touchPosition = new Vector2();
        int effectiveTouchCount = 0;
        foreach (Touch touch in Touch.activeTouches)
        {
            Debug.Log(touch.screenPosition.ToString().Contains("Infinity"));
            if (!touch.screenPosition.ToString().Contains("Infinity"))
            {
                touchPosition += touch.screenPosition;
                effectiveTouchCount++;
            }
            
        }
        touchPosition /= effectiveTouchCount;
        
        
        /*
         * single touch
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue(); //touch space
        */
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition); //convert touch space to world space
        currentBallRigidBody.position = worldPosition;
        
        
    }

    private void LaunchBall()
    {
        currentBallRigidBody.isKinematic = false;
        currentBallRigidBody = null;

        Invoke(nameof(DetachBall), detachDelay);
        Invoke(nameof(SpawnNewBall), respawnDelay);
        
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentBallSpringJoint.connectedBody = pivot;
    }
}
