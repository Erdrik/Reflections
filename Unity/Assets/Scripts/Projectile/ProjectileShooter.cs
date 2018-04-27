using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour {

    [SerializeField]
    private Projectile _projectilePrefab;

    public void ShootProjectile(Vector2 velocity)
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(velocity);
    }
	
}
