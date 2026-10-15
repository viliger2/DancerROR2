using Dancer.Modules.Components;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Dancer.Modules
{

    internal static class Projectiles
    {
        internal static GameObject dancerRibbonProjectile;

        internal static void RegisterProjectiles()
        {
            dancerRibbonProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion(), "RibbonProjectile");
            SphereCollider sphereCollider = dancerRibbonProjectile.GetComponent<SphereCollider>();
            sphereCollider.radius = 1.25f;

            ProjectileController projectileController = dancerRibbonProjectile.GetComponent<ProjectileController>();
            GameObject gameObject = PrefabAPI.InstantiateClone(Assets.mainAssetBundle.LoadAsset<GameObject>("RibbonBall"), "RibbonProjectileGhost");
            if (!gameObject.GetComponent<NetworkIdentity>())
            {
                gameObject.AddComponent<NetworkIdentity>();
            }
            if (!gameObject.GetComponent<ProjectileGhostController>())
            {
                gameObject.AddComponent<ProjectileGhostController>();
            }
#pragma warning disable CS0618 // Type or member is obsolete
            projectileController.ghostPrefab = gameObject;
#pragma warning restore CS0618 // Type or member is obsolete
            projectileController.ghostPrefabAddress = new AssetReferenceT<GameObject>("");

            ProjectileSimple projectileSimple = dancerRibbonProjectile.GetComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 150f;

            ProjectileDamage projectileDamage = dancerRibbonProjectile.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageTypeCombo.GenericSpecial;
            projectileDamage.damageType.AddModdedDamageType(Content.DamageTypes.Ribbon);

            ProjectileSingleTargetImpact projectileSingleTarget = dancerRibbonProjectile.GetComponent<ProjectileSingleTargetImpact>();
            projectileSingleTarget.enemyHitSound = Assets.whip1HitSoundEvent;
            projectileSingleTarget.hitSound = Assets.lungeHitSoundEvent;

            dancerRibbonProjectile.AddComponent<ProjectileSpawnRibbonController>();
            Prefabs.projectilePrefabs.Add(dancerRibbonProjectile);
        }
    }
}
