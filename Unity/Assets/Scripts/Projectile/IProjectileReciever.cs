using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileReciever {

    void OnProjectileHit(Projectile.ProjectileHit hit);
	
}
