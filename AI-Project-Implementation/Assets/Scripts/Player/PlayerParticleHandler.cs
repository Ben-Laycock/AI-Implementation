using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleHandler : MonoBehaviour
{
    [Header("Booster Particles")]
    [SerializeField] private List<GameObject> mBoosterParticles;
    private List<BoosterParticleInformation> mBoosterParticleSystems;

    private void Start()
    {

        mBoosterParticleSystems = new List<BoosterParticleInformation>();

        for (int i = 0; i < mBoosterParticles.Count; i++)
        {
            ParticleSystem system = mBoosterParticles[i].GetComponent<ParticleSystem>();
            ParticleSystem.MainModule module = system.main;
            mBoosterParticleSystems.Add(new BoosterParticleInformation(system, module.startLifetime.constant));
        }

    }

    public void ChangeBoosterParticles(float currentPlayerMovementSpeed, float maxPlayerMovementSpeed)
    {
        for (int i = 0; i < mBoosterParticleSystems.Count; i++)
        {
            float mBoosterParticleLifeTime = Remap(currentPlayerMovementSpeed, 0.0f, maxPlayerMovementSpeed, mBoosterParticleSystems[i].mOriginalStartTime, mBoosterParticleSystems[i].mOriginalStartTime * 1.7f);
            float mBoosterParticleSpeed = Remap(currentPlayerMovementSpeed, 0.0f, maxPlayerMovementSpeed, 0.2f, 1);

            ParticleSystem.MainModule module = mBoosterParticleSystems[i].mParticleSystem.main;
            module.startLifetime = mBoosterParticleLifeTime;
            module.startSpeed = mBoosterParticleSpeed;
        }
    }

    public float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
}

public class BoosterParticleInformation
{
    public ParticleSystem mParticleSystem;
    public float mOriginalStartTime = 0.0f;

    public BoosterParticleInformation(ParticleSystem system, float startTime)
    {
        mParticleSystem = system;
        mOriginalStartTime = startTime;
    }
}