using UnityEngine;
using UnityEngine.InputSystem;

public class BallMovement : MonoBehaviour
{
    public enum Mode { disabled, waiting, after, enabled }
    public Mode mode;

    [Header("Ball settings")]
    public Rigidbody rb;
    public float forwardForce;

    public Vector3 rotationAxis;
    public float rotationForce;

    void Start()
    {
        mode = Mode.disabled;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mode == Mode.disabled || mode == Mode.waiting) return;

        if (transform.position.y < 0.3f) rb.AddForce(forwardForce * Time.deltaTime * 100, 0, 0);
        else
        {
            if (Keyboard.current.aKey.isPressed) rb.AddTorque(rotationAxis * rotationForce * Time.deltaTime, ForceMode.Force);
            if (Keyboard.current.dKey.isPressed) rb.AddTorque(rotationAxis * -rotationForce * Time.deltaTime, ForceMode.Force);
        }
    }

    public float getPosX()
    {
        return transform.position.x;
    }

    public void enableBall(Quaternion angle, float force)
    {
        mode = Mode.enabled;
        rb.AddForce(forwardForce * force * (angle * Vector3.right), ForceMode.Impulse);
    }

    public void waitBall(float time)
    {
        if (mode == Mode.enabled)
        {
            mode = Mode.waiting;
            Invoke("afterBall", time);
        }
    }

    public void afterBall()
    {
        mode = Mode.after;
    }
    
    public void disableBall()
    {
        mode = Mode.disabled;
    }
}
