using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowEffect : MonoBehaviour {

    class GlowHit
    {
        public Vector3 _hitPoint;
        public Color _hitColour;
        public float _currentLifetime;
    }

    private static List<GlowHit> _hits = new List<GlowHit>();

    public static void RegisterHit(Vector3 hitPoint, Color hitColour)
    {
        _hits.Add(new GlowHit() {
            _hitPoint = hitPoint,
            _hitColour = hitColour,
            _currentLifetime = 0
        });
        if(_hits.Count > 100)
        {
            _hits.RemoveAt(0);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _hits.Count; i++)
        {
            _hits[i]._currentLifetime += Time.deltaTime;
            if(_hits[i]._currentLifetime > 5)
            {
                _hits.RemoveAt(i);
            }
        }
        Shader.SetGlobalVectorArray("_HitPoints", ConvertListToVector4(_hits));
        Shader.SetGlobalVectorArray("_HitColours", COnvertColoursToVec4(_hits));
    }

    private static Vector4[] COnvertColoursToVec4(List<GlowHit> hits)
    {
        Vector4[] result = new Vector4[100];
        for (int i = 0; i < 100; i++)
        {
            if (i < hits.Count)
            {
                result[i] = new Vector4(hits[i]._hitColour.r, hits[i]._hitColour.g, hits[i]._hitColour.b, hits[i]._hitColour.a);
            }
            else
            {
                result[i] = new Vector4();
            }
        }
        return result;
    }

    private static Vector4[] ConvertListToVector4(List<GlowHit> hits)
    {
        Vector4[] result = new Vector4[100];
        for (int i = 0; i < 100; i++)
        {
            if(i < hits.Count)
            {
                result[i] = new Vector4(hits[i]._hitPoint.x, hits[i]._hitPoint.y, hits[i]._hitPoint.z, hits[i]._currentLifetime);
            }
            else
            {
                result[i] = new Vector4();
            }
        }
        return result;
    }

}
