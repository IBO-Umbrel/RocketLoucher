using UnityEngine;
using UnityEngine.InputSystem;
 
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{   
  [SerializeField] float thrustStrength = 100f;
  [SerializeField] float rotationStrength = 100f;

  // Audio and particle effects are intentionally paused for now.
  // [SerializeField] AudioClip mainEngineSFX;
  // [SerializeField] ParticleSystem mainEngineParticles;
  // [SerializeField] ParticleSystem rightThrustParticles;
  // [SerializeField] ParticleSystem leftThrustParticles;
 
  Rigidbody rb;
  Collider rocketCollider;
  // AudioSource audioSource;
 
  private void Awake() 
  {
    rb = GetComponent<Rigidbody>();
    rocketCollider = GetComponent<Collider>();
    rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
    // audioSource = GetComponent<AudioSource>();
  }
 
  private void FixedUpdate()
  {
    ProcessThrust();
    ProcessRotation();
  }
 
  private void ProcessThrust()
  {
    Keyboard keyboard = Keyboard.current;
    bool isControllingRocket = keyboard != null &&
      (keyboard.wKey.isPressed || keyboard.aKey.isPressed || keyboard.dKey.isPressed);

    if (isControllingRocket)
    {
      StartThrusting();
    }
  }
 
  private void StartThrusting()
  {
    rb.AddRelativeForce(Vector3.up * thrustStrength);

    // if (!audioSource.isPlaying)
    // {
    //   audioSource.PlayOneShot(mainEngineSFX);
    // }
    // if (!mainEngineParticles.isPlaying)
    // {
    //   mainEngineParticles.Play();
    // }
  }
 
  private void ProcessRotation()
  {
    Keyboard keyboard = Keyboard.current;
    if (keyboard == null)
    {
      return;
    }

    if (keyboard.dKey.isPressed && !keyboard.aKey.isPressed)
    {
      RotateRight();
    }
    else if (keyboard.aKey.isPressed && !keyboard.dKey.isPressed)
    {
      RotateLeft();
    }
    else
    {
      StopRotating();
    }
  }
 
  private void RotateRight()
  {
    ApplyRotation(rotationStrength);

    // if (!rightThrustParticles.isPlaying)
    // {
    //   leftThrustParticles.Stop();
    //   rightThrustParticles.Play();
    // }
  }
 
  private void RotateLeft()
  {
    ApplyRotation(-rotationStrength);

    // if (!leftThrustParticles.isPlaying)
    // {
    //   rightThrustParticles.Stop();
    //   leftThrustParticles.Play();
    // }
  }
 
  private void StopRotating()
  {
    // rightThrustParticles.Stop();
    // leftThrustParticles.Stop();
  }
 
  private void ApplyRotation(float rotationThisFrame)
  {
    float nextZRotation = rb.rotation.eulerAngles.z + rotationThisFrame * Time.fixedDeltaTime;
    rb.MoveRotation(Quaternion.Euler(0f, 0f, nextZRotation));
  }

  private void OnCollisionEnter(Collision collision)
  {
    LandingPad pad = collision.collider.GetComponentInParent<LandingPad>();
    if (pad == null)
    {
      return;
    }

    SnapToPad(pad);
  }

  private void SnapToPad(LandingPad pad)
  {
    Collider padCollider = pad.GetComponent<Collider>();
    if (padCollider == null || rocketCollider == null)
    {
      return;
    }

    Bounds padBounds = padCollider.bounds;
    Vector3 snapPosition = new Vector3(
      padBounds.center.x,
      padBounds.max.y + rocketCollider.bounds.extents.y,
      padBounds.center.z);

    rb.position = snapPosition;
    rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }
}
