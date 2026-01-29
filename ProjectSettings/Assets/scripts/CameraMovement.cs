using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public enum CameraMode { Static, Follow, SmoothMove }
    public CameraMode mode;

    [Header("Ball settings")]
    public Transform ball;
    public Vector3 ballOffset;
    public Quaternion ballDirection;

    [Header("PinBox settings")]
    public Transform pinBox;
    public Vector3 pinBoxOffset;
    public Quaternion pinBoxDirection;

    [Header("Arrow settings")]
    public Transform arrow;
    public Vector3 arrowOffset;
    public Quaternion arrowDirection;

    [Header("SmoothMove settings")]
    Vector3 startPosition;
    Quaternion startRotation;
    Vector3 endPosition;
    Quaternion endRotation;
    public float moveSpeed = 1f;      // szybkość przesuwania
    public float rotationSpeed = 1f;  // szybkość obracania (użyta przy Slerp)
    private float t = 0f;             // postęp interpolacji

    Transform followTarget;
    Vector3 followOffset;
    Vector3 followPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        snapBall();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch(mode)
        {
            case CameraMode.Static:
                // camera stays in place
                break;
            case CameraMode.Follow:
                followPosition.x = followTarget.position.x + followOffset.x;
                followPosition.y = followTarget.position.y + followOffset.y;
                followPosition.z = followTarget.position.z + followOffset.z;
                transform.position = followPosition;
                break;
            case CameraMode.SmoothMove:
                // zwiększamy t w czasie
                t += Time.deltaTime * moveSpeed;

                // płynna interpolacja pozycji
                transform.position = Vector3.Lerp(startPosition, endPosition, t);

                // płynna interpolacja rotacji
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

                if (t >= 1f)
                {
                    t = 1f;
                }
                break;

        }
    }

    public void snapBall()
    {
        mode = CameraMode.Follow;
        followTarget = ball;
        followOffset = ballOffset;
        transform.rotation = ballDirection;
    }

    public void snapPinBox()
    {
        mode = CameraMode.SmoothMove;
        t = 0f;
        startPosition = transform.position;
        startRotation = transform.rotation;
        endPosition = pinBox.position + pinBoxOffset;
        endRotation = pinBoxDirection;
    }

    public void snapArrow()
    {
        mode = CameraMode.Static;
        transform.rotation = arrowDirection;
        transform.position = arrow.position + arrowOffset;
    }

    public int test()
    {
        return 2137;
    }
    
    public void follow(Transform transform, Quaternion direction, Vector3 offset)
    {
        
    }
}
