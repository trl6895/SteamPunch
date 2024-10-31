using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraActions : MonoBehaviour
{
    // Fields ============================================================================

    // References -------------------------------------------------------------
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Camera mainCamera;

    // Values -----------------------------------------------------------------
    private float shakeDuration = 0.0f;
    private float shakeIntensity = 0.0f;

    // Methods ===========================================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0.0f)
        {
            virtualCamera.ForceCameraPosition((Vector3)Random.insideUnitCircle * shakeIntensity + mainCamera.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 1.0f));

            shakeDuration -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Makes the camera shake
    /// </summary>
    /// <param name="duration">How long the camera will shake for</param>
    /// <param name="intensity">How intense the shaking will be</param>
    public void Shake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
    }
}
