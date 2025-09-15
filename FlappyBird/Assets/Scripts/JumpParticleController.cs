using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class JumpParticleController : MonoBehaviour
{
[Tooltip("Het ParticleSystem dat af moet spelen bij springen. Als leeg, wordt het eerste ParticleSystem op dit object gebruikt.")]
public ParticleSystem jumpParticles;


void Reset()
{
// probeer automatisch het ParticleSystem te vinden
if (jumpParticles == null)
jumpParticles = GetComponentInChildren<ParticleSystem>();
}


void Awake()
{
if (jumpParticles == null)
jumpParticles = GetComponentInChildren<ParticleSystem>();


if (jumpParticles != null)
jumpParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
}


/// <summary>
/// Speel het jump-particle effect (one-shot).
/// </summary>
public void PlayOnce()
{
if (jumpParticles == null) return;
// speel als één burst, ook als de particle system in loop staat
jumpParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
jumpParticles.Play();
}


/// <summary>
/// Utility: speel een burst met X deeltjes.
/// </summary>
public void PlayBurst(int count)
{
if (jumpParticles == null) return;
var emitParams = new ParticleSystem.EmitParams();
jumpParticles.Emit(emitParams, count);
}
}