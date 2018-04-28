using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Projectile : MonoBehaviour, IProjectileReciever {

    public struct BulletLine
    {
        public List<Vector2> _points;
        public float _distance;
    }

    public struct ProjectileHit
    {
        public float _initalDamage;
        public int _numberOfBounces;
        public Vector2 _hitNormal;
    }

    [SerializeField]
    private LayerMask _bulletInteractionLayers;
    [SerializeField]
    private float _minInteractionDistance = 0.1f;
    [SerializeField]
    private float _bulletRadius = 1;
    [SerializeField]
    private Vector2 _velocity = new Vector2(1, 0);
    [SerializeField]
    private float _killDistance = 40;
    [SerializeField]
    private GameObject _hitGameObject;

    [SerializeField]
    private float _initalDamage;



    private float _totalElapsedDistance;
    private int _amountOfBounces;

	void Start () {

    }

    private void FixedUpdate()
    {
        VelocityStep(Time.fixedDeltaTime);
    }

    public void Launch(Vector2 intialVelocity)
    {
        _velocity = intialVelocity;
    }

    public BulletLine PredictDistance(Vector2 position, Vector2 velocity, float distance, float dT = 0)
    {
        if(_velocity.sqrMagnitude == 0)
        {
            return new BulletLine();
        }
        if(dT == 0)
        {
            dT = Time.fixedDeltaTime;
        }

        BulletLine line = new BulletLine();
        line._points = new List<Vector2>() { position };
        line._distance = distance;

        float elapsedDistance = 0;
        Vector2 currentPosition = position;
        Vector2 currentVelocity = velocity;

        do
        {
            float rayDistance = _bulletRadius + _minInteractionDistance;
            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, currentVelocity.normalized, rayDistance, _bulletInteractionLayers);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    continue;
                }
                Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, hit.normal);
                float reflectedOffset = rayDistance - hit.distance;

                line._points.Add(hit.point + hit.normal * _bulletRadius);

                currentVelocity = reflectedVelocity;
                currentPosition += reflectedVelocity.normalized * reflectedOffset;
                break;
            }
            currentPosition += currentVelocity.normalized * _minInteractionDistance;
            elapsedDistance += _minInteractionDistance;

        } while (elapsedDistance < distance);

        line._points.Add(currentPosition);

        return line;
    }

    private void VelocityStep(float dT)
    {
        float movedDistance = 0;
        do
        {
            float rayDistance = _bulletRadius + _minInteractionDistance;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, _velocity.normalized, rayDistance, _bulletInteractionLayers);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    continue;
                }
                Vector3 reflectedVelocity = Vector2.Reflect(_velocity, hit.normal);
                float reflectedOffset = rayDistance - hit.distance;
                ProjectileHit projectileHit = new ProjectileHit()
                {
                    _initalDamage = _initalDamage,
                    _numberOfBounces = _amountOfBounces,
                    _hitNormal = -hit.normal
                };
                Bolt.CustomEvent.Trigger(hit.collider.gameObject, "OnProjectileHit", projectileHit);

                IProjectileReciever[] recievers = hit.collider.GetComponents<IProjectileReciever>();
                foreach (IProjectileReciever rev in recievers)
                {
                    rev.OnProjectileHit(projectileHit);
                }

                GlowEffect.RegisterHit(hit.point);

                Instantiate(_hitGameObject, hit.point, Quaternion.LookRotation(hit.normal, Vector2.up));

                _amountOfBounces++;
                _velocity = reflectedVelocity;
                transform.position += reflectedVelocity.normalized * reflectedOffset;
                break;
            }
            transform.position += new Vector3(_velocity.x, _velocity.y).normalized * _minInteractionDistance;
            movedDistance += _minInteractionDistance;

        } while (movedDistance < _velocity.magnitude * dT);
        _totalElapsedDistance += movedDistance;
        if(_totalElapsedDistance > _killDistance) {
            Destroy(gameObject);
        }
    }

    

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public void OnProjectileHit(ProjectileHit hit)
    {
        Vector3 reflectedVelocity = Vector2.Reflect(_velocity, hit._hitNormal);
        _velocity = reflectedVelocity;
    }
}
