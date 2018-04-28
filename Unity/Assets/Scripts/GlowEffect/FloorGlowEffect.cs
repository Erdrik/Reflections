using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGlowEffect : MonoBehaviour {


    public Transform _playerTransform;
    public Color _playerColour;
    public Material _floorMaterial;

    private static List<Projectile> _bulletTransform = new List<Projectile>();

    public static void RegisterBullet(Projectile p)
    {
        _bulletTransform.Add(p);
        if(_bulletTransform.Count > 99)
        {
            _bulletTransform.RemoveAt(0);
        }
    }

    private void Update()
    {
        PruneBullets();
        Vector4[] positions = new Vector4[100];
        Vector4[] colours = new Vector4[100];
        float[] bools = new float[100];

        positions[0] = _playerTransform.position;
        colours[0] = _playerColour;
        bools[0] = 1;

        for (int i = 1; i < 100; i++)
        {
            if(i-1 < _bulletTransform.Count)
            {
                positions[i] = _bulletTransform[i-1].transform.position;
                colours[i] = _bulletTransform[i-1]._gridColour;
                bools[i] = 1;
            }
            else
            {
                bools[i] = 0;
            }
        }


        _floorMaterial.SetVectorArray("_positions", positions);
        _floorMaterial.SetVectorArray("_colours", colours);
        _floorMaterial.SetFloatArray("_using", bools);

    }

    private void PruneBullets()
    {
        for (int i = 0; i < _bulletTransform.Count; i++)
        {
            if(_bulletTransform[i] == null)
            {
                _bulletTransform.RemoveAt(i);
            }
        }
    }

}
