using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SinusoidalCameraShake : MonoBehaviour
{
    public float shakeDuration = 1;
    public int numberOfShakes = 8;
    public float translationStrength = 1;
    public float rotationStrength = .5f;

    float duration = 0;
    float timeElapsed = 0;

    List<Vector3> shakePositions;
    List<Vector3> shakeRotations;

    void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        timeElapsed = 0;
    }

    void Update()
    {
        UpdateShake();
    }

    public void Shake()
    {
        duration = shakeDuration;
        timeElapsed = 0;

        SetShakePositions();
        SetShakeRotations();
    }

    public void Shake(float transStrength, float rotStrength, float duration, int shakes)
    {
        shakeDuration = duration;
        numberOfShakes = shakes;
        translationStrength = transStrength;
        rotationStrength = rotStrength;

        Shake();
    }

    void UpdateShake()
    {
        if (duration > 0)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < shakeDuration)
            {
                float frequency = numberOfShakesCos(timeElapsed);
                int roundedFrequency = Mathf.RoundToInt(numberOfShakesSin(timeElapsed));

                ShakeCamera(timeElapsed, frequency, roundedFrequency);
            }
            else
            {
                timeElapsed = 0;
                duration = 0;
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    void ShakeCamera(float time, float frequency, int roundedFrequency)
    {
        int increment = Mathf.FloorToInt(Frequency(time));

        int index01 = roundedFrequency + increment;
        int index02 = (1 - roundedFrequency) + increment;

        float tStrength = DampenStrength(time, translationStrength);
        float rStrength = DampenStrength(time, rotationStrength);

        transform.localPosition = Vector3.Lerp(shakePositions[index01] * tStrength, shakePositions[index02] * tStrength, frequency);
        transform.localEulerAngles = Vector3.Lerp(shakeRotations[index01] * rStrength, shakeRotations[index02] * rStrength, frequency);
    }

    float numberOfShakesCos(float time)
    {
        float frequency = Mathf.Cos(Frequency(time, Mathf.PI));

        return 1 - NormalizeWave(frequency);
    }

    float numberOfShakesSin(float time)
    {
        float frequency = Mathf.Sin(Frequency(time, Mathf.PI));

        return 1 - NormalizeWave(frequency);
    }

    float Frequency(float time, float pi = 1)
    {
        float f = time * pi * numberOfShakes;
        f *= 2 - (time / shakeDuration);
        f /= shakeDuration;

        return f * 0.75f; // 0.75f makes sure we ocsilate through 4 points 
    }

    float DampenStrength(float time, float strength)
    {
        float frequency = (time * Mathf.PI) / shakeDuration;
        float easeInOut = Mathf.Cos(frequency);

        return NormalizeWave(easeInOut);
    }

    float NormalizeWave(float value)
    {
        return (value + 1) * .5f;
    }

    void SetShakePositions()
    {
        shakePositions = new List<Vector3>();

        int totalShakes = numberOfShakes - 1;

        for (int i = 0; i < totalShakes; i++)
        {
            shakePositions.Add(RandomShakePosition());
        }

        shakePositions.Add(Vector3.zero); // finish at 0
    }

    Vector3 RandomShakePosition()
    {
        Vector3 unitVec3D = Random.insideUnitSphere;
        unitVec3D *= translationStrength;

        return unitVec3D;
    }

    void SetShakeRotations()
    {
        shakeRotations = new List<Vector3>();

        int totalShakes = numberOfShakes - 1;

        for (int i = 0; i < totalShakes; i++)
        {
            shakeRotations.Add(RandomShakeRotation());
        }

        shakeRotations.Add(Vector3.zero); // finish at 0
    }

    Vector3 RandomShakeRotation()
    {
        Vector3 unitVec3D = Random.insideUnitSphere;
        float yForward = Mathf.Abs(unitVec3D.y); // make sure we are always facing forward
        unitVec3D *= rotationStrength;

        return unitVec3D;

    }
}
