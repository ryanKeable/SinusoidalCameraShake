using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SinusoidalCameraShake : MonoBehaviour
{
    public float shakeDuration = 1;
    public int numberOfShakes = 8;
    public float translationStrength = 1;
    public float rotationStrength = .5f;

    const float rotStrengthModifier = 1.5f;
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

        SetShakeDeltas();
    }

    public void Shake(float transStrength, float duration, int shakes)
    {
        shakeDuration = duration;
        numberOfShakes = shakes;
        translationStrength = transStrength;
        rotationStrength = transStrength * rotStrengthModifier;

        Shake();
    }
    public void Shake(float transStrength, float duration, int shakes, float rotStrength)
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

                ShakeCamera(timeElapsed);
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

    void ShakeCamera(float time)
    {
        // this is the Sinusoidal Shake engine
        // uses models in thie graph: https://www.desmos.com/calculator/t3eg3bc3xa

        // which shake we are upto
        int shakeIncrement = Mathf.FloorToInt(Frequency(time));

        // at each increment we need to determine the deltas to lerp between
        // because we are using an Sinusoidal wave to drive our lerping we swap delta targets per shake increment using a sin wave as a LUT
        int lerpDir = Mathf.RoundToInt(numberOfShakesSin(timeElapsed)); // returns 0 or 1 (forwards or backwards)
        int deltaTarget01 = lerpDir + shakeIncrement;
        int deltaTarget02 = (1 - lerpDir) + shakeIncrement;

        // reduce strength of shake over time
        float tStrength = DampenStrength(time, translationStrength);
        float rStrength = DampenStrength(time, rotationStrength);

        // ocsilates beteen 0 and 1 over the durtation with decay
        float lerpFrequency = numberOfShakesCos(timeElapsed);

        transform.localPosition = Vector3.Lerp(shakePositions[deltaTarget01] * tStrength, shakePositions[deltaTarget02] * tStrength, lerpFrequency);
        transform.localEulerAngles = Vector3.Lerp(shakeRotations[deltaTarget01] * rStrength, shakeRotations[deltaTarget02] * rStrength, lerpFrequency);
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
        float speed = time * pi * numberOfShakes;
        float decay = 2 - (time / shakeDuration);
        float frequency = (speed * decay) / shakeDuration;
        frequency *= 0.75f; // 0.75f makes sure we ocsilate through 4 points 

        return frequency;
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

    void SetShakeDeltas()
    {
        shakeRotations = new List<Vector3>();
        shakePositions = new List<Vector3>();

        int totalShakes = numberOfShakes - 2; // do not count bookends

        shakeRotations.Add(Vector3.zero); // start at 0
        shakePositions.Add(Vector3.zero); // start at 0

        for (int i = 0; i < totalShakes; i++)
        {
            shakeRotations.Add(RandomRotationDelta());
            shakePositions.Add(RandomPositionDelta());
        }

        shakeRotations.Add(Vector3.zero); // finish at 0
        shakePositions.Add(Vector3.zero); // finish at 0
    }

    Vector3 RandomRotationDelta()
    {
        Vector3 unitVec3D = Random.insideUnitSphere;
        float yForward = Mathf.Abs(unitVec3D.y); // make sure we are always facing forward
        unitVec3D *= rotationStrength;

        return unitVec3D;
    }

    Vector3 RandomPositionDelta()
    {
        Vector3 unitVec3D = Random.insideUnitSphere;
        unitVec3D *= translationStrength;

        return unitVec3D;
    }
}
