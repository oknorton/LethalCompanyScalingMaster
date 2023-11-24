using System;
using LC_API.ServerAPI;
using LethalCompanyModV2.Component;
using UnityEngine;

namespace LethalCompanyScalingMaster.Component
{
    public class BroadcastingComponent : MonoBehaviour
    {
        private static BroadcastingComponent _instance;

        public static BroadcastingComponent Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("(BroadcastingComponent.cs)BroadcastingComponent");
                    _instance = go.AddComponent<BroadcastingComponent>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        public static void BroadcastDeathPenalty()
        {
            Debug.Log("(BroadcastingComponent.cs)Broadcasting new value!");
            Networking.Broadcast(0.33f, "death_penalty");
        }

        private void OnEnable()
        {
            Debug.Log("(BroadcastingComponent.cs)Broadcasting: Subscribed");
            Networking.GetFloat += OnGotFloat;

        }

        private void OnDisable()
        {
            Debug.Log("(BroadcastingComponent.cs)Broadcasting: Unsubscribed");
            Networking.GetFloat -= OnGotFloat;

        }

        private void OnGotFloat(float data, string signature)
        {
            Debug.Log("(BroadcastingComponent.cs)Got a float for (" + signature + ") with data: " + data);
            UpdateDeathPenalty(data);
        }

        private void UpdateDeathPenalty(float deathPenalty)
        {
            Debug.Log("(BroadcastingComponent.cs)Updating the death penalty - Old value: " + Plugin.DeathPenalty +
                      "new Value: " + deathPenalty);
            Plugin.DeathPenalty = deathPenalty;
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.Log("Destroying the Broadcasting component");
                Destroy(gameObject);
            }
        }
    }
}