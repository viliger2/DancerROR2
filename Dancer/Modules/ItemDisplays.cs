using RoR2;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Dancer.Modules
{

    internal static class ItemDisplays
    {
        private static Dictionary<string, AssetReferenceGameObject> itemDisplayPrefabs = new Dictionary<string, AssetReferenceGameObject>();

        internal static void PopulateDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponent<ModelLocator>()
                .modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            ItemDisplayRuleSet.KeyAssetRuleGroup[] keyAssetRuleGroups = itemDisplayRuleSet.keyAssetRuleGroups;
            for (int i = 0; i < keyAssetRuleGroups.Length; i++)
            {
                ItemDisplayRule[] rules = keyAssetRuleGroups[i].displayRuleGroup.rules;
                for (int j = 0; j < rules.Length; j++)
                {
                    if (rules[j].followerPrefabAddress.RuntimeKeyIsValid())
                    {
                        var handle = Addressables.LoadResourceLocationsAsync(rules[j].followerPrefabAddress.AssetGUID);
                        var address = handle.WaitForCompletion();
                        if(address.Count > 0)
                        {
                            var assetName = System.IO.Path.GetFileNameWithoutExtension(address[0].PrimaryKey);
                            itemDisplayPrefabs[assetName.ToLower()] = rules[j].followerPrefabAddress;
                        }
                        Addressables.Release(handle);
                    }
                }
            }
        }

        internal static AssetReferenceGameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()) && itemDisplayPrefabs[name.ToLower()].RuntimeKeyIsValid())
            {
                return itemDisplayPrefabs[name.ToLower()];
            }
            return null;
        }
    }
}
