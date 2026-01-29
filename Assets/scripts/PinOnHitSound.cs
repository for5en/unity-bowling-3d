using UnityEngine;

public class PinOnHitSound : MonoBehaviour
{
    public float minVolume = 0.8f;
    public float maxVolume = 1.2f;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    public AudioClip[] hitSounds;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Pin")) return;

        float force = collision.relativeVelocity.magnitude;
        if (force < 1f) return;

        if (transform.position.y < -0.2f) return;

        if (hitSounds.Length == 0) return;
        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];

        float volume = Mathf.Clamp(force / 10f, minVolume, maxVolume);
        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(clip, volume);
    }
}
