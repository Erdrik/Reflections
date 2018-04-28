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
        Projectile.BulletLine line = _projectilePrefab.PredictDistance(transform.position, -transform.right*10, 200);
        _lineRenderer.positionCount = line._points.Count;

        List<Vector3> cpoints = new List<Vector3>();
        foreach (Vector2 v in line._points)
        {
            cpoints.Add(new Vector3(v.x, v.y, 0));
        }

        _lineRenderer.SetPositions(cpoints.ToArray());
    }

    public void ShootProjectile(Vector2 velocity)
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(velocity);
    }
	
}
