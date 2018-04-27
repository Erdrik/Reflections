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
    private float _bulletRadius = 1;
    [SerializeField]
    private Vector2 _velocity = new Vector2(1, 0);
    [SerializeField]
    private List<Color> _graidentColours;

    [SerializeField]
    private float _initalDamage;
    


    private int _amountOfBounces;

	void Start () {
        BulletLine prediction = PredictDistance(10);

    }
    Gradient CreateBulletLineGradient(BulletLine line)
    {
        if(_graidentColours.Count < 1)
        {
            _graidentColours.Add(Color.white);
        }
        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Fixed;

        List<GradientColorKey> keys = new List<GradientColorKey>();
        float offset = 0;
        for (int i = 1; i < line._points.Count; i++)
        {
            Vector2 lastPoint = line._points[i - 1];
            Vector2 currentPoint = line._points[i];

            float distance = Vector2.Distance(lastPoint, currentPoint);
            float relativeDistance = distance / line._distance;

            Color keyColour = _graidentColours[_graidentColours.Count - 1];

            if( i < _graidentColours.Count)
            {
                keyColour = _graidentColours[i-1];
            }

            keys.Add(new GradientColorKey(keyColour, offset + relativeDistance));
            offset += relativeDistance;
        }

        gradient.colorKeys = keys.ToArray();

        return gradient;
    }

    private void FixedUpdate()
    {
        VelocityStep(Time.fixedDeltaTime);
    }

    public void Launch(Vector2 intialVelocity)
    {
        _velocity = intialVelocity;
    }

    public BulletLine PredictDistance(float distance, float dT = 0)
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
        line._points = new List<Vector2>() { transform.position };
        line._distance = distance;

        float elapsedDistance = 0;
        Vector2 currentPosition = transform.position;
        Vector2 currentVelocity = _velocity;

        while (elapsedDistance < distance)
        {
            elapsedDistance += currentVelocity.magnitude * dT;
            float rayDistance = _bulletRadius + currentVelocity.magnitude * dT;
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentVelocity.normalized, rayDistance, _bulletInteractionLayers);
            if (hit && hit.collider.gameObject != gameObject)
            {
                
                Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, hit.normal);
                float reflectedOffset = rayDistance - hit.distance;

                line._points.Add(hit.point + hit.normal * _bulletRadius);
                currentVelocity = reflectedVelocity;
                currentPosition += reflectedVelocity.normalized * reflectedOffset;
            }
            else
            {
                currentPosition += currentVelocity * dT;
            }
        }

        line._points.Add(currentPosition);

        return line;
    }

    private void VelocityStep(float dT)
    {
        float movedDistance = 0;
        do
        {
            float rayDistance = _bulletRadius + dT;
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
                _amountOfBounces++;
                _velocity = reflectedVelocity;
                transform.position += reflectedVelocity.normalized * reflectedOffset;
                break;
            }
            transform.position += new Vector3(_velocity.x, _velocity.y).normalized * dT;
            movedDistance += dT;

        } while (movedDistance < _velocity.magnitude * dT);
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
