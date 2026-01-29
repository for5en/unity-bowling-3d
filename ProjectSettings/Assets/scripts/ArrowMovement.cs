using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowMovement : MonoBehaviour
{
    public enum Mode { disabled, waiting, after, enabled }
    public Mode mode;

    public float rotationSpeed = 90f; // stopnie na sekundę
    private float directionAngle = 0f;
    public float scaleSpeed = 1f;
    private float scaleValue = 0.1f;
    private Vector3 tempScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mode = Mode.disabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == Mode.disabled || mode == Mode.waiting || mode == Mode.after) return;

        scaleValue = Mathf.PingPong(Time.time * scaleSpeed, 0.15f) + 0.05f;
        directionAngle = Mathf.PingPong(Time.time * rotationSpeed, 60f) - 30f; // od -30 do +30 stopni
        tempScale = transform.localScale;
        tempScale.x = scaleValue;
        transform.localScale = tempScale;
        transform.rotation = Quaternion.Euler(0f, directionAngle, 0f);
    }

    public Quaternion getDirection()
    {
        return transform.rotation;
    }

    public float getForce()
    {
        return scaleValue * 8f;
    }


    public void enableArrow()
    {
        mode = Mode.enabled;
        gameObject.SetActive(true);
        directionAngle = 0f;
    }

    public void waitArrow(float time)
    {
        if (mode == Mode.enabled)
        {
            mode = Mode.waiting;
            Invoke("afterArrow", time);
        }
    }

    public void afterArrow()
    {
        mode = Mode.after;
    }
    
    public void disableArrow()
    {
        mode = Mode.disabled;
        gameObject.SetActive(false);
    }
}
