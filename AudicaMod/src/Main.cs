using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Harmony;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;

namespace HtiScoreVisualizerMod
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color c, JsonSerializer serializer)
        {
            writer.WriteValue($"#{c.r:X2}{c.g:X2}{c.b:X2}");
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;
            int hexValue = Int32.Parse(s.Replace("#", ""), NumberStyles.HexNumber);

            var color = new Color();
            color.r = (byte)((hexValue >> 16) & 0xFF);
            color.g = (byte)((hexValue >> 8) & 0xFF);
            color.b = (byte)((hexValue) & 0xFF);
            color.a = 255;

            return color;
        }
    }

    public class HtiScoreVisualizer : MelonMod
    {
        public static class BuildInfo
        {
            public const string Name = "HitScoreVisualizer";  // Name of the Mod.  (MUST BE SET)
            public const string Author = "Mettra"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "0.2.0"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }

        struct CuttoffColorPair
        {
            public float cutoff;
            public Color color;
        };

        static List<CuttoffColorPair> colors;
        static bool nextPopupIsScore = false;
        static UInt32 nextMaxScore = 1;
        static UInt32 nextMaxScoreSub = 0;

        public override void OnApplicationStart()
        {
            string path = Application.dataPath + "/../Mods/Config/HitScoreVisualizer.json";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Application.dataPath + "/../Mods/Config");
                string defaultJsonFile =
@"[
    {
        ""cutoff"": 0.4,
        ""color"": ""FF00FF""
    },
    {
        ""cutoff"": 0.7,
        ""color"": ""00FF4E""
    },
    {
        ""cutoff"": 0.95,
        ""color"": ""00FFDE""
    },
    {
        ""cutoff"": 1.0,
        ""color"": ""FFD800""
    }
]";

                File.WriteAllText(path, defaultJsonFile);
            }


            colors = JsonConvert.DeserializeObject<List<CuttoffColorPair>>(File.ReadAllText(path), new ColorConverter());
            colors.Sort((left, right) => left.cutoff.CompareTo(right.cutoff));


        }

        [HarmonyPatch(typeof(Target), "CompleteTarget", new Type[] {})]
        private static class Target_CompleteTarget
        {
            private static bool Prefix(Target __instance)
            {
                nextPopupIsScore = true;
                var cue = __instance.mCue;
                var behavior = cue.behavior;

                nextMaxScoreSub = 0;

                if (behavior == Target.TargetBehavior.Standard || behavior == Target.TargetBehavior.Vertical || behavior == Target.TargetBehavior.Horizontal || behavior == Target.TargetBehavior.ChainStart)
                {
                    nextMaxScore = 2000;
                }
                else if (behavior == Target.TargetBehavior.Hold)
                {
                    nextMaxScore = 2000;
                    nextMaxScoreSub = 1000;
                }
                else if (behavior == Target.TargetBehavior.Chain)
                {
                    nextMaxScore = 125;
                }
                else if (behavior == Target.TargetBehavior.Melee)
                {
                    nextMaxScore = 2000;
                }
                else
                {
                    nextMaxScore = 0;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(TextPopupPool), "CreatePopup", new Type[] { typeof(Vector3), typeof(Quaternion), typeof(Vector3), typeof(string), typeof(string) })]
        private static class TextPopupPool_CreatePopup
        {
            private static bool Prefix(TextPopupPool __instance, Vector3 position, Quaternion rotation, Vector3 scale, string text, string extraText)
            {
                if (nextPopupIsScore)
                {
                    nextPopupIsScore = false;

                    if (nextMaxScore == 0)
                    {
                        return true;
                    }

                    UInt32 score = 0;
                    if (!UInt32.TryParse(text, out score))
                    {
                        return true;
                    }

                    score -= nextMaxScoreSub;

                    float scorePercent = score / (float)nextMaxScore;
                    if (scorePercent < 0)
                    {
                        scorePercent = 0;
                    }
                    else if (scorePercent > 1)
                    {
                        scorePercent = 1;
                    }

                    var idx = __instance.mIndex;
                    var popup = __instance.mPopups[idx];

                    Color foundColor = Color.white;
                    foreach (var c in colors)
                    {
                        if (scorePercent <= c.cutoff)
                        {
                            foundColor = c.color;
                            break;
                        }
                    }

                    popup.text.SetFaceColor(foundColor);
                }

                return true;
            }
        }
    }
}



