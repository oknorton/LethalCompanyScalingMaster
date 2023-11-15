using Unity.Netcode;
using UnityEngine;

namespace LethalCompanyModV2.Component
{
    public class GUIManager
    {
        private TimeOfDay _tod;
        private bool _initialized;

        //per player values (modded)
        private string _playerCountQuotaModifier = "10";
        private string _baseQuota = "120";


        //base values
        private string _baseIncreaseInput = "80";
        private float _quotaIncreaseSteepness = 8.5f;

        private string _daysUntilDeadlineInput = "4";
        private bool _enableAutoUpdatedScaling = true;
        private float _tempDeathPenalty = 0.2f;
        private string _perPlayerCredits = "15";
        private int _totalStartingCredits = 60;
        
        private GUIStyle _currentStyle;
        private GUIStyle _textFieldStyle;
        private GUIStyle _titleLabelStyle;
        private GUIStyle _headerLabelStyle;
        private bool _showSideMenu;

       

        public void OnGUI()
        {
            if (_initialized == false)
            {
                InitStyles();
            }

            GUILayout.BeginArea(new Rect(10, 35, 500, 700));
            GUI.Box(new Rect(0, 0, 520, 700), "", _currentStyle);
            
            GUILayout.Label("LC - Better Quota Scaler", _headerLabelStyle);

            GUILayout.Label("Quota Settings", _titleLabelStyle);

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Starting Quota", GUILayout.Width(100));
                    GUILayout.Label("=", GUILayout.Width(15));
                    GUILayout.Label("Base Quota", GUILayout.Width(80));
                    GUILayout.Label("+", GUILayout.Width(15));
                    GUILayout.Label("(", GUILayout.Width(15));
                    GUILayout.Label("Player Count", GUILayout.Width(80));
                    GUILayout.Label("X", GUILayout.Width(15));
                    GUILayout.Label("Player Count Modifier", GUILayout.Width(125));
                    GUILayout.Label(")", GUILayout.Width(15));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    if (float.TryParse(_baseQuota, out float bqParsed) &&
                        float.TryParse(_playerCountQuotaModifier, out float pcParsed))
                    {
                        int startingQuota =
                            (int)(bqParsed + (NetworkManager.Singleton.ConnectedClients.Count * pcParsed));
                        GUILayout.Label(startingQuota.ToString(), GUILayout.Width(100));
                    }

                    GUILayout.Label("=", GUILayout.Width(15));
                    _baseQuota = GUILayout.TextField(_baseQuota, _textFieldStyle, GUILayout.Width(80));
                    GUILayout.Label("+", GUILayout.Width(15));
                    GUILayout.Label("(", GUILayout.Width(15));
                    GUILayout.Label(NetworkManager.Singleton.ConnectedClients.Count.ToString(), GUILayout.Width(80));
                    GUILayout.Label("X", GUILayout.Width(15));
                    _playerCountQuotaModifier = GUILayout.TextField(_playerCountQuotaModifier, _textFieldStyle, GUILayout.Width(125));
                    GUILayout.Label(")", GUILayout.Width(15));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            if (float.TryParse(_baseQuota, out float baseQuotaParsed) && float.TryParse(_playerCountQuotaModifier, out float playerCountQuotaModifierParsed))
            {
                int startingQuota = (int)(baseQuotaParsed + (NetworkManager.Singleton.ConnectedClients.Count * playerCountQuotaModifierParsed));
                GUILayout.Label("Starting Profit Quota = " + startingQuota);
            }

            GUILayout.Width(400);
            
            GUILayout.Label("Quota increase steepness: " + _quotaIncreaseSteepness);
            _quotaIncreaseSteepness =
                Mathf.Round(GUILayout.HorizontalSlider(_quotaIncreaseSteepness, 0f, 10f, GUILayout.Width(300)) * 2) / 2;

            GUILayout.Label("Quota increase base increase value: " + _tod.quotaVariables.baseIncrease);
            _baseIncreaseInput = GUILayout.TextField(_baseIncreaseInput, _textFieldStyle, GUILayout.Width(300));



            GUILayout.Label("Deadline settings", _titleLabelStyle);

            GUILayout.Label("Deadline: " + _tod.daysUntilDeadline);
            _daysUntilDeadlineInput =
                GUILayout.TextField(_daysUntilDeadlineInput, _textFieldStyle, GUILayout.Width(300));

            GUILayout.Label("Death Penalty Settings", _titleLabelStyle);

            // Slider for death penalty
            GUILayout.Label("Per Player Death Penalty: " + _tempDeathPenalty * 100 + "%");
            _tempDeathPenalty = Mathf.Round(GUILayout.HorizontalSlider(_tempDeathPenalty, 0f, 1f, GUILayout.Width(300)) * 100) /
                           100;
            GUILayout.Label("Max percentage of money you can lose with these settings " +
                            _tempDeathPenalty * 100 * NetworkManager.Singleton.ConnectedClients.Count + "%");

            GUILayout.Label("Credit/Funds settings", _titleLabelStyle);
            GUILayout.Label("Per Player Starting Credits: " + _perPlayerCredits);
            _perPlayerCredits = GUILayout.TextField(_perPlayerCredits, _textFieldStyle, GUILayout.Width(300));
            if (float.TryParse(_perPlayerCredits, out float perPlayerCreditsParsed))
            {
                 _totalStartingCredits =
                    (int)(NetworkManager.Singleton.ConnectedClients.Count * perPlayerCreditsParsed);
                GUILayout.Label("Total Starting credits with these settings " + _totalStartingCredits);
            }


            GUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = _enableAutoUpdatedScaling ? Color.green : Color.red;
                if (GUILayout.Button(
                        "Auto Update on player join: " + (_enableAutoUpdatedScaling ? "Enabled" : "Disabled"),
                        GUILayout.Width(250)))
                {
                    _enableAutoUpdatedScaling = !_enableAutoUpdatedScaling;
                    Plugin.AutoUpdateQuota = _enableAutoUpdatedScaling;
                }

                // Save button
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button("Save values", GUILayout.Width(250)))
                {
                    SaveValues();
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Show Calculated Values", GUILayout.Width(500)))
            {
                _showSideMenu = !_showSideMenu;
            }

            GUILayout.EndArea();

            if (_showSideMenu)
            {
                GUILayout.BeginArea(new Rect(560, 10, 200, 550));
                GUILayout.BeginVertical("box");
                GUILayout.Label("Quotas for the next rounds", _titleLabelStyle);

                for (int round = 1; round <= 10; round++)
                {
                    int quota = CalculateQuotaForRound(round);
                    GUILayout.Label("Round " + round + ": " + quota);
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }


        private int CalculateQuotaForRound(int round)
        {
            var instance = _tod;
            var quotaVariables = instance.quotaVariables;

            int profitQuota = quotaVariables.startingQuota;

            if (round == 1)
            {
                return profitQuota;
            }

            for (int i = 1; i < round; i++)
            {
                float num2 = Mathf.Clamp(
                    1f + i * (i / quotaVariables.increaseSteepness),
                    0f, 10000f);
                num2 = quotaVariables.baseIncrease * num2;

                profitQuota = (int)Mathf.Clamp(profitQuota + num2, 0f, 1E+09f);
            }

            return profitQuota;
        }


        private void SaveValues()
        {
            Debug.Log("NON parsed values= " + _baseQuota + " + "  + _playerCountQuotaModifier + " X " + NetworkManager.Singleton.ConnectedClients.Count);

            if (float.TryParse(_baseQuota, out float baseQuotaParsed) && float.TryParse(_playerCountQuotaModifier,
                    out float playerCountQuotaModifierParsed))
            {
                Debug.Log("Parsed values= " + baseQuotaParsed + " + "  + playerCountQuotaModifierParsed + " X " + NetworkManager.Singleton.ConnectedClients.Count);
                int startingQuota = (int)(baseQuotaParsed + (NetworkManager.Singleton.ConnectedClients.Count * playerCountQuotaModifierParsed));

                _tod.quotaVariables.startingQuota = startingQuota;
                _tod.profitQuota = startingQuota;
            }

            if (float.TryParse(_baseIncreaseInput, out float baseIncrease))
            {
                _tod.quotaVariables.baseIncrease = baseIncrease;
            }

            if (int.TryParse(_daysUntilDeadlineInput, out int daysUntilDeadline))
            {
                Plugin.DeadlineAmount = daysUntilDeadline;
                _tod.quotaVariables.deadlineDaysAmount = daysUntilDeadline;
            }

            _tod.quotaVariables.increaseSteepness = _quotaIncreaseSteepness;
            Plugin.DeathPenalty = _tempDeathPenalty;
            Terminal objectOfType = Object.FindObjectOfType<Terminal>();
            objectOfType.groupCredits = _totalStartingCredits;
            
            Plugin.UpdateAndSyncValues();
        }
        private void InitStyles()
        {
            _tod = TimeOfDay.Instance;
            if (_currentStyle == null)
            {
                _currentStyle = new GUIStyle(GUI.skin.box);
                _currentStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0f, 0f, 1.0f));
                _currentStyle.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
            }

            if (_textFieldStyle == null)
            {
                _textFieldStyle = new GUIStyle(GUI.skin.textField);
            }

            if (_titleLabelStyle == null)
            {
                _titleLabelStyle = new GUIStyle(GUI.skin.label);
                _titleLabelStyle.fontSize = 15;
                _titleLabelStyle.alignment = TextAnchor.MiddleLeft;
            }
            if (_headerLabelStyle == null)
            {
                _headerLabelStyle = new GUIStyle(GUI.skin.label);
                _headerLabelStyle.fontSize = 24;
                _headerLabelStyle.alignment = TextAnchor.MiddleCenter;
            }
            _initialized = true;

        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}