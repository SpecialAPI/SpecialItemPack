using System;
using System.Collections;
using Brave.BulletScript;
using UnityEngine;

public class MimicRainbowMiniguns1 : Script
{
    protected override IEnumerator Top()
    {
        for (int i = 0; i < 10; i++)
        {
            this.FireBurst((i % 2 != 0) ? "right gun" : "left gun");
            if (i % 3 == 2)
            {
                yield return this.Wait(6);
                this.QuadShot(this.AimDirection + UnityEngine.Random.Range(-60f, 60f), (!BraveUtility.RandomBool()) ? "right gun" : "left gun", UnityEngine.Random.Range(9f, 11f));
                yield return this.Wait(6);
            }
            yield return this.Wait(12);
        }
        yield break;
    }

    private void FireBurst(string transform)
    {
        float num = base.RandomAngle();
        float num2 = 22.5f;
        for (int i = 0; i < 16; i++)
        {
            base.Fire(new Offset(transform), new Direction(num + (float)i * num2, DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new SpecialGrossBullet());
        }
    }

    private void QuadShot(float direction, string transform, float speed)
    {
        for (int i = 0; i < 4; i++)
        {
            base.Fire(new Offset(transform), new Direction(direction, DirectionType.Absolute, -1f), new Speed(speed - (float)i * 1.5f, SpeedType.Absolute), new SpeedChangingGrossBullet("bigGross", speed, 120, -1, false));
        }
    }

    private const int NumBursts = 10;
    private const int NumBulletsInBurst = 16;
}

public class SpeedChangingGrossBullet : Bullet
{
    public SpeedChangingGrossBullet(float newSpeed, int term, int destroyTimer = -1) : base(null, false, false, false)
    {
        this.m_newSpeed = newSpeed;
        this.m_term = term;
        this.m_destroyTimer = destroyTimer;
    }

    public SpeedChangingGrossBullet(string name, float newSpeed, int term, int destroyTimer = -1, bool suppressVfx = false) : base(name, suppressVfx, false, false)
    {
        this.m_newSpeed = newSpeed;
        this.m_term = term;
        this.m_destroyTimer = destroyTimer;
    }

    protected override IEnumerator Top()
    {
        this.ChangeSpeed(new Speed(this.m_newSpeed, SpeedType.Absolute), this.m_term);
        if (this.m_destroyTimer < 0)
        {
            yield break;
        }
        yield return this.Wait(this.m_term + this.m_destroyTimer);
        this.Vanish(false);
        yield break;
    }

    public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
    {
        if (preventSpawningProjectiles)
        {
            return;
        }
        float num = base.RandomAngle();
        float num2 = 60f;
        for (int i = 0; i < 6; i++)
        {
            base.Fire(new Direction(num + num2 * (float)i, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ShotgrubManAttack1.GrubBullet());
        }
    }

    private float m_newSpeed;
    private int m_term;
    private int m_destroyTimer;
}

public class SpecialGrossBullet : Bullet
{
    public SpecialGrossBullet() : base("gross", false, false, false)
    {
    }

    public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
    {
        if (preventSpawningProjectiles)
        {
            return;
        }
        float num = base.RandomAngle();
        float num2 = 60f;
        for (int i = 0; i < 6; i++)
        {
            base.Fire(new Direction(num + num2 * (float)i, DirectionType.Absolute, -1f), new Speed(8f, SpeedType.Absolute), new ShotgrubManAttack1.GrubBullet());
        }
    }
}
