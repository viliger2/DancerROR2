﻿using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using RoR2.Audio;
using System.Collections.Generic;
using RoR2.UI;
using RoR2.Projectile;
namespace Ridley.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        internal static GameObject footDragEffect;
        internal static GameObject groundDragEffect;
        internal static GameObject vitalPrefab;
        // particle effects
        internal static GameObject swordSwingEffect;
        internal static GameObject swordHitImpactEffect;

        internal static GameObject punchSwingEffect;
        internal static GameObject punchImpactEffect;

        internal static GameObject fistBarrageEffect;

        internal static GameObject bombExplosionEffect;
        internal static GameObject bazookaExplosionEffect;
        internal static GameObject bazookaMuzzleFlash;
        internal static GameObject dustEffect;

        internal static GameObject muzzleFlashEnergy;
        internal static GameObject swordChargeEffect;
        internal static GameObject swordChargeFinishEffect;
        internal static GameObject minibossEffect;

        internal static GameObject spearSwingEffect;

        internal static GameObject nemSwordSwingEffect;
        internal static GameObject nemSwordHeavySwingEffect;
        internal static GameObject nemSwordHitImpactEffect;

        internal static GameObject shotgunTracer;
        internal static GameObject energyTracer;

        // custom crosshair
        internal static GameObject bazookaCrosshair;

        // tracker
        internal static GameObject trackerPrefab;

        internal static GameObject exposeTrackerPrefab;

        // networked hit sounds
        internal static NetworkSoundEventDef sword1HitSoundEvent;
        internal static NetworkSoundEventDef sword2HitSoundEvent;
        internal static NetworkSoundEventDef sword3HitSoundEvent;
        internal static NetworkSoundEventDef hit2SoundEvent;
        internal static NetworkSoundEventDef jab1HitSoundEvent;
        internal static NetworkSoundEventDef jab2HitSoundEvent;
        internal static NetworkSoundEventDef jab3HitSoundEvent;
        internal static NetworkSoundEventDef punchHitSoundEvent;
        internal static NetworkSoundEventDef nemSwordHitSoundEvent;

        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        internal static List<EffectDef> effectDefs = new List<EffectDef>();

        // cache these and use to create our own materials
        public static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        public static Material commandoMat;

        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ridley.ridleysurvivorassets"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ridley.HenryBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ridley.RidleyBank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }


            sword1HitSoundEvent = CreateNetworkSoundEventDef("SwordHit1");
            sword2HitSoundEvent = CreateNetworkSoundEventDef("SwordHit2");
            sword3HitSoundEvent = CreateNetworkSoundEventDef("SwordHit3");
            jab1HitSoundEvent = CreateNetworkSoundEventDef("JabHit1");
            jab2HitSoundEvent = CreateNetworkSoundEventDef("JabHit2");
            jab3HitSoundEvent = CreateNetworkSoundEventDef("JabHit3");
            hit2SoundEvent = CreateNetworkSoundEventDef("Hit2");
            punchHitSoundEvent = CreateNetworkSoundEventDef("PunchHit");
            
            GameObject b = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectileghosts/sunderghost"), "RidleyGroundDrag");
            if (b)
            {
                GameObject.Destroy(b.GetComponent<ProjectileGhostController>());

                var effect = b.AddComponent<EffectComponent>();
                effect.applyScale = true;
                effect.effectIndex = EffectIndex.Invalid;
                effect.parentToReferencedTransform = true;
                effect.positionAtReferencedTransform = true;
                groundDragEffect = b;
            }
            /*
            GameObject q = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectileghosts/sunderghost"), "RidleyFootDrag");
            if (q)
            {
                q.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                GameObject.Destroy(q.GetComponent<GameObject>());
                GameObject.Destroy(q.GetComponent<ShakeEmitter>());
                GameObject.Destroy(q.GetComponent<ProjectileGhostController>());
                foreach (AkEvent a in q.GetComponents<AkEvent>())
                    GameObject.Destroy(a);
                var effect = q.AddComponent<EffectComponent>();
                effect.applyScale = true;
                effect.effectIndex = EffectIndex.Invalid;
                effect.parentToReferencedTransform = true;
                effect.positionAtReferencedTransform = true;
                footDragEffect = q;
            }
            */
            dustEffect = LoadEffect("HenryDustEffect");
            bombExplosionEffect = LoadEffect("BombExplosionEffect", "HenryBombExplosion");
            bazookaExplosionEffect = LoadEffect("HenryBazookaExplosionEffect", "HenryBazookaExplosion");
            bazookaMuzzleFlash = LoadEffect("HenryBazookaMuzzleFlash");

            muzzleFlashEnergy = LoadEffect("NemryMuzzleFlashEnergy", true);
            minibossEffect = mainAssetBundle.LoadAsset<GameObject>("NemryMinibossIndicator");

            swordChargeFinishEffect = LoadEffect("SwordChargeFinishEffect");
            swordChargeEffect = mainAssetBundle.LoadAsset<GameObject>("SwordChargeEffect");
            swordChargeEffect.AddComponent<ScaleParticleSystemDuration>().particleSystems = swordChargeEffect.GetComponentsInChildren<ParticleSystem>();
            swordChargeEffect.GetComponent<ScaleParticleSystemDuration>().initialDuration = 1.5f;

            ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.5f;
            shakeEmitter.radius = 200f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 40f,
                cycleOffset = 0f
            };

            shakeEmitter = bazookaExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.4f;
            shakeEmitter.radius = 100f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 30f,
                cycleOffset = 0f
            };

            swordSwingEffect = Assets.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = Assets.LoadEffect("ImpactHenrySlash");

            punchSwingEffect = Assets.LoadEffect("HenryFistSwingEffect", true);
            //punchImpactEffect = Assets.LoadEffect("ImpactHenryPunch");
            // on second thought my effect sucks so imma just clone loader's
            punchImpactEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXLoader"), "ImpactHenryPunch");
            punchImpactEffect.AddComponent<NetworkIdentity>();

            AddNewEffectDef(punchImpactEffect);
            //EffectAPI.AddEffect(punchImpactEffect);

            fistBarrageEffect = Assets.LoadEffect("FistBarrageEffect", true);
            fistBarrageEffect.GetComponent<ParticleSystemRenderer>().material.shader = hotpoo;

            bazookaCrosshair = PrefabAPI.InstantiateClone(LoadCrosshair("ToolbotGrenadeLauncher"), "HenryBazookaCrosshair", false);
            CrosshairController crosshair = bazookaCrosshair.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];
            bazookaCrosshair.transform.Find("StockCountHolder").gameObject.SetActive(false);
            bazookaCrosshair.transform.Find("Image, Arrow (1)").gameObject.SetActive(true);
            crosshair.spriteSpreadPositions[0].zeroPosition = new Vector3(32f, 34f, 0f);
            crosshair.spriteSpreadPositions[2].zeroPosition = new Vector3(-32f, 34f, 0f);
            bazookaCrosshair.transform.GetChild(1).gameObject.SetActive(false);

            trackerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"), "HenryTrackerPrefab", false);
            trackerPrefab.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            trackerPrefab.transform.Find("Core Pip").localScale = new Vector3(0.15f, 0.15f, 0.15f);

            trackerPrefab.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            trackerPrefab.transform.Find("Core, Dark").localScale = new Vector3(0.1f, 0.1f, 0.1f);

            exposeTrackerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/TemporaryVisualEffects/MercExposeEffect"), "ExposeEffectPrefab", false);

            foreach (SpriteRenderer i in trackerPrefab.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (i)
                {
                    i.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                    i.color = Color.white;
                }
            }

            shotgunTracer = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("HenryBulletTracer", true);

            if (!shotgunTracer.GetComponent<EffectComponent>()) shotgunTracer.AddComponent<EffectComponent>();
            if (!shotgunTracer.GetComponent<VFXAttributes>()) shotgunTracer.AddComponent<VFXAttributes>();
            if (!shotgunTracer.GetComponent<NetworkIdentity>()) shotgunTracer.AddComponent<NetworkIdentity>();

            foreach (LineRenderer i in shotgunTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    Material bulletMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    bulletMat.SetColor("_TintColor", new Color(0.68f, 0.58f, 0.05f));
                    i.material = bulletMat;
                    i.startColor = new Color(0.68f, 0.58f, 0.05f);
                    i.endColor = new Color(0.68f, 0.58f, 0.05f);
                }
            }

            AddNewEffectDef(shotgunTracer);
            //EffectAPI.AddEffect(shotgunTracer);

            spearSwingEffect = Assets.LoadEffect("NemrySpearSwingEffect");

            nemSwordSwingEffect = Assets.LoadEffect("NemrySwordSwingEffect", true);
            nemSwordHeavySwingEffect = Assets.LoadEffect("NemryHeavySwordSwingEffect", true);
            nemSwordHitImpactEffect = Assets.LoadEffect("ImpactNemrySlash");

            energyTracer = CreateTracer("TracerHuntressSnipe", "NemryEnergyTracer");

            LineRenderer line = energyTracer.transform.Find("TracerHead").GetComponent<LineRenderer>();
            Material tracerMat = UnityEngine.Object.Instantiate<Material>(line.material);
            line.startWidth *= 0.25f;
            line.endWidth *= 0.25f;
            // this did not work.
            //tracerMat.SetColor("_TintColor", new Color(78f / 255f, 80f / 255f, 111f / 255f));
            line.material = tracerMat;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            GameObject newTracer = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            /*NetworkSoundEventCatalog.getSoundEventDefs += delegate (List<NetworkSoundEventDef> list)
            {
                list.Add(networkSoundEventDef);
            };*/
            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);
            //EffectAPI.AddEffect(newEffect);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}