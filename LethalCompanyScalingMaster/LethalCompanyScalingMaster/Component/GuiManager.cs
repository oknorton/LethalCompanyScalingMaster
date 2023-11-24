using System.Linq;
using LethalCompanyScalingMaster;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace LethalCompanyModV2.Component
{
    public class GUIManager
    {
        public static TimeOfDay _tod;
        private bool _initialized;

        //per player values (modded)
        public static string _playerCountQuotaModifier = "10";
        public static string _baseQuota = "120";


        //base values
        public static string _baseIncreaseInput = "80";
        public static float _quotaIncreaseSteepness = 8.5f;
        public static int _deathPenalty = 20;

        public static string _daysUntilDeadlineInput = "4";
        private bool _enableAutoUpdatedScaling = true;
        private string _perPlayerCredits = "15";
        public static int _totalStartingCredits = 60;

        private GUIStyle _currentStyle;
        private GUIStyle _textFieldStyle;
        private GUIStyle _titleLabelStyle;
        private GUIStyle _headerLabelStyle;
        private bool _showSideMenu;


        public void OnGUI()
        {
            if (_initialized == false)
            {
                Init();
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
                            (int)(bqParsed + (Plugin.GetConnectedPlayers() * pcParsed));
                        GUILayout.Label(startingQuota.ToString(), GUILayout.Width(100));
                    }

                    GUILayout.Label("=", GUILayout.Width(15));
                    _baseQuota =
                        FilterNumericInput(GUILayout.TextField(_baseQuota, _textFieldStyle, GUILayout.Width(80)));
                    GUILayout.Label("+", GUILayout.Width(15));
                    GUILayout.Label("(", GUILayout.Width(15));
                    GUILayout.Label(Plugin.GetConnectedPlayers().ToString(), GUILayout.Width(80));
                    GUILayout.Label("X", GUILayout.Width(15));
                    _playerCountQuotaModifier = FilterNumericInput(
                        GUILayout.TextField(_playerCountQuotaModifier, _textFieldStyle, GUILayout.Width(125)));
                    GUILayout.Label(")", GUILayout.Width(15));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            if (float.TryParse(_baseQuota, out float baseQuotaParsed) && float.TryParse(_playerCountQuotaModifier,
                    out float playerCountQuotaModifierParsed))
            {
                int startingQuota = (int)(baseQuotaParsed +
                                          (Plugin.GetConnectedPlayers() *
                                           playerCountQuotaModifierParsed));
                GUILayout.Label("Starting Profit Quota = " + startingQuota);
            }

            GUILayout.Width(400);

            GUILayout.Label("Quota increase steepness: " + _quotaIncreaseSteepness);
            _quotaIncreaseSteepness =
                Mathf.Round(GUILayout.HorizontalSlider(_quotaIncreaseSteepness, 0f, 10f, GUILayout.Width(300)) * 2) / 2;

            GUILayout.Label("Quota increase base increase value: " + _tod.quotaVariables.baseIncrease);
            _baseIncreaseInput =
                FilterNumericInput(GUILayout.TextField(_baseIncreaseInput, _textFieldStyle, GUILayout.Width(300)));


            GUILayout.Label("Deadline settings", _titleLabelStyle);

            GUILayout.Label("Deadline: " + _tod.daysUntilDeadline);
            _daysUntilDeadlineInput = FilterNumericInput(
                GUILayout.TextField(_daysUntilDeadlineInput, _textFieldStyle, GUILayout.Width(300)));


            GUILayout.Label("Death Penalty Settings", _titleLabelStyle);
            float deathPenaltySlider = GUILayout.HorizontalSlider(_deathPenalty / 100.0f, 0f, 1f, GUILayout.Width(300));
            _deathPenalty = Mathf.RoundToInt(deathPenaltySlider * 100);
            var _deathPenaltyDisplay = _deathPenalty.ToString() + "%";
            GUILayout.Label("Death Penalty: " + _deathPenaltyDisplay);


            GUILayout.Label("Per Player Death Penalty: " +_deathPenaltyDisplay);

            GUILayout.Label("Max percentage of money you can lose is " + ((_deathPenalty) * Plugin.GetConnectedPlayers()).ToString("0.00") + "%");


            GUILayout.Label("Credit/Funds settings", _titleLabelStyle);
            GUILayout.Label("Per Player Starting Credits: " + _perPlayerCredits);
            _perPlayerCredits =
                FilterNumericInput(GUILayout.TextField(_perPlayerCredits, _textFieldStyle, GUILayout.Width(300)));
            if (float.TryParse(_perPlayerCredits, out float perPlayerCreditsParsed))
            {
                _totalStartingCredits =
                    (int)(Plugin.GetConnectedPlayers() * perPlayerCreditsParsed);
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
                    Plugin.SaveValues();
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

        private string FilterNumericInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "0";
            }

            string filteredInput = new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());

            if (string.IsNullOrWhiteSpace(filteredInput))
            {
                return "0";
            }

            return filteredInput;
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


        public void Init()
        {
            _tod = TimeOfDay.Instance;
            Plugin.DeathPenalty = 80 / Plugin.GetConnectedPlayers();
            _deathPenalty = 80 / Plugin.GetConnectedPlayers();
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