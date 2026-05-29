using UnityEngine;
using System.Collections;

public class CricketBowlingController : MonoBehaviour
{
    public enum BowlingType
    {
        Straight,
        Swing,
        Spin
    }


    [Header("Ball Setup")]
    public GameObject ballPrefab;
    public Transform spawnPoint;


    [Header("Target Controller")]
    public BounceTargetController targetController;

    private Transform bounceTarget;


    [Header("Bowling")]
    public BowlingType bowlingType;

    public float ballSpeed = 20f;


    [Header("Swing / Spin")]
    public float movementAmount = 5f;

    [Range(0, 1)]
    public float movementMultiplier = 1f;


    float FinalMovementAmount
    {
        get
        {
            return movementAmount * movementMultiplier;
        }
    }


    [Header("After Bounce")]
    public float afterBounceSpeed = 15f;
    public float afterBounceHeight = 4f;

    private Rigidbody ballRb;
    private Vector3 lastMoveDirection;



    private void Start()
    {
        bounceTarget = targetController.GetTarget();
    }



    public void Bowl()
    {
        if (bounceTarget == null)
        {
            bounceTarget = targetController.GetTarget();
        }
        if (bounceTarget == null)
        {
            Debug.LogError("Bounce Target not spawned");
            return;
        }
        GameObject ball =Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody>();


        ballRb.isKinematic = true;
        ballRb.useGravity = false;


        StartCoroutine(MoveToBounce(ball.transform));
    }



    IEnumerator MoveToBounce(
        Transform ball)
    {
        Vector3 start = spawnPoint.position;


        // NOW COMES FROM TARGET PREFAB
        Vector3 end = bounceTarget.position;



        Vector3 control = (start + end) * 0.5f;



        if (bowlingType == BowlingType.Swing)
        {
            Vector3 forward = end - start;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = Quaternion.Euler(0, 90, 0)* forward;
            control += right* FinalMovementAmount;
        }


        float duration = Vector3.Distance(start, end)/ballSpeed;
        float timer = 0;

        Vector3 previousPosition = start;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            Vector3 pos = QuadraticBezier(start, control, end, t);

            lastMoveDirection = (pos - previousPosition).normalized;

            previousPosition = pos;
            ball.position = pos;
            yield return null;
        }


        ball.position = end;


        ContinueAfterBounce();
    }



    Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    void ContinueAfterBounce()
    {
        ballRb.isKinematic = false;
        ballRb.useGravity = true;

        Vector3 dir = new Vector3(lastMoveDirection.x, 0, lastMoveDirection.z).normalized;
        ballRb.linearVelocity = dir * afterBounceSpeed + Vector3.up * afterBounceHeight;

        if (bowlingType == BowlingType.Spin)
        {
            ApplySpin();
        }
    }

    void ApplySpin()
    {
        Quaternion spin = Quaternion.AngleAxis(FinalMovementAmount,Vector3.up);
        ballRb.linearVelocity = spin * ballRb.linearVelocity;
    }

    public void SetMultiplier(float multilierValue)
    {
        movementMultiplier = multilierValue;
    }

    public void SetBowlingTypeSpin()
    {
        bowlingType = BowlingType.Spin;
    }

    public void SetBowlingTypeSwing()
    {
        bowlingType = BowlingType.Swing;
    }

    public void SetMaxBallMovement(float maxBallMovement)
    {
        movementAmount = maxBallMovement;
    }
}