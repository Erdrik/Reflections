using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour {

    [SerializeField]
    private Projectile _projectilePrefab;
    [SerializeField]
    private LineRenderer _lineRenderer;

    private void Start()
    {
        
    }

    private void Update()
    {
    }

    public void ShootProjectile(Vector2 velocity)
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        FloorGlowEffect.RegisterBullet(projectile);
        projectile.Launch(velocity);
    }
	
}
