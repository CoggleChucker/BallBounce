using UnityEngine;
using UnityEngine.InputSystem;

public class BounceTargetController : MonoBehaviour
{
    [Header("Target Prefab")]
    public GameObject targetPrefab;
    public InputAction moveInput;

    private Transform currentTarget;


    [Header("Movement")]
    public float moveSpeed = 5f;


    [Header("Side Limits (World X)")]
    public float minSide = -2.5f;
    public float maxSide = 2.5f;


    [Header("Forward Limits (World Z)")]
    public float minForward = -2f;
    public float maxForward = 7.5f;


    private const float targetHeight = 0.5f;

    private void Start()
    {
        SpawnTarget();
        moveInput.Enable();
    }

    private void OnDestroy()
    {
        moveInput.Disable();
    }

    void SpawnTarget()
    {
        Vector3 spawnPosition = new Vector3(0f, targetHeight, 0f);
        GameObject obj = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        currentTarget = obj.transform;
    }


    private void Update()
    {
        if (currentTarget == null)
            return;
        MoveTarget();
    }

    void MoveTarget()
    {
        Vector3 movement = new Vector3(moveInput.ReadValue<Vector2>().x, 0, moveInput.ReadValue<Vector2>().y);
        movement.Normalize();

        Vector3 newPosition = currentTarget.position + movement * moveSpeed * Time.deltaTime;

        // WORLD POSITION CLAMP
        newPosition.x =Mathf.Clamp(newPosition.x, minSide, maxSide);
        newPosition.z =Mathf.Clamp(newPosition.z, minForward, maxForward);
        
        // lock height
        newPosition.y = targetHeight;

        currentTarget.position = newPosition;
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }
}