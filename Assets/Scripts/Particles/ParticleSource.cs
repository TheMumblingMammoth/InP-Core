using UnityEngine;
public class ParticleSource : MonoBehaviour{
    [SerializeField] Color color;
    void FixedUpdate()
    {
        Particle.Spawn(transform, 1f, color);
    }
    
}