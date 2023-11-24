using System;
using System.Globalization;
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

        public static void BroadcastDeathPenalty(int penalty)
        {
            Debug.Log("(BroadcastingComponent.cs) Broadcasting new dp value: " + penalty);
            
            Networking.Broadcast(penalty, "death_penalty");
        }

        private void OnEnable()
        {
            Debug.Log("(BroadcastingComponent.cs)Broadcasting: Subscribed");
            Networking.GetInt += OnGotInt;

        }

        private void OnDisable()
        {
            Debug.Log("(BroadcastingComponent.cs)Broadcasting: Unsubscribed");
            Networking.GetInt -= OnGotInt;

        }

        private void OnGotInt(int data, string signature)
        {
            if (signature.Equals("death_penalty"))
            {
                Debug.Log("(BroadcastingComponent.cs) Signature matched!");
            }
            Debug.Log("(BroadcastingComponent.cs) Got an int for (" + signature + ") with data: " + data);
            UpdateDeathPenalty(data);
        }

        private void UpdateDeathPenalty(int deathPenalty)
        {
            Debug.Log("(BroadcastingComponent.cs)Updating the death penalty - Old gui value: " + Plugin.DeathPenalty + "new Value: " + deathPenalty);
            Debug.Log("(BroadcastingComponent.cs)Updating the death penalty - Old plug value: " + GUIManager._deathPenalty + "new Value: " + deathPenalty);
            Plugin.DeathPenalty = deathPenalty;
            GUIManager._deathPenalty = deathPenalty;
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