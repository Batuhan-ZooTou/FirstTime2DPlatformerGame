using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable<T>
{
    public void TakeDamage(int damage);
}
public interface IKnockable<T>
{
    public void KnockBack(bool right);
}
