using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.FrozenBox.Utils.Editor
{
    public class ConfigSettingsProvider<TConfig> where TConfig : ScriptableObject
    {
        protected const string RootPath = "Project/";
        protected const string AssetsPath = "Assets/";
        private static readonly string CachedPathKey = $"CACHED_PATH_{typeof(TConfig).Name}";
        
        protected static SettingsProvider CreateSettingsProvider_Internal(string settingsPath) 
            => CreateSettingsProvider_Internal(settingsPath, $"{AssetsPath}{typeof(TConfig).Name}.asset");

        protected static SettingsProvider CreateSettingsProvider_Internal(string settingsPath, string defaultAssetPath)
        {
            UnityEditor.Editor? inlineEditor = null;
            var isExpanded = true;
            
            return new SettingsProvider("Project/TagsSystem", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    var config = TryFindOrCreate(defaultAssetPath);
                    var newConfig = EditorGUILayout.ObjectField("Current Config", config, typeof(TConfig), false) as TConfig;
                    if (newConfig != config) {
                        var assets = PlayerSettings.GetPreloadedAssets()?.ToList() ?? new List<Object>();
                        if (newConfig != null) {
                            assets.Add(newConfig);
                            CacheConfigPath(newConfig);
                        }
                        if (config != null) assets.Remove(config);
                        PlayerSettings.SetPreloadedAssets(assets.ToArray());
                        config = newConfig;
                    }
            
                    if (config != null)
                    {
                        EditorGUILayout.Space(10);
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        
                        isExpanded = EditorGUILayout.Foldout(isExpanded, "Inline Config", true, EditorStyles.foldoutHeader);

                        if (isExpanded)
                        {
                            EditorGUI.indentLevel++;
                            UnityEditor.Editor.CreateCachedEditor(config, null, ref inlineEditor);

                            if (inlineEditor != null) 
                                inlineEditor.OnInspectorGUI();

                            EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
            };
        }

        private static TConfig? TryFindOrCreate(string defaultPath)
        {
            TConfig? config = null;
            var cachedPath = SessionState.GetString(CachedPathKey, "");
            if (!string.IsNullOrEmpty(cachedPath) && GlobalObjectId.TryParse(cachedPath, out var id)) 
                config = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as TConfig;
            
            var assets = PlayerSettings.GetPreloadedAssets()?.ToList() ?? new List<Object>();
            var configs = assets.OfType<TConfig>().ToList();

            if (config != null)
            {
                if (configs.Count > 0) {
                    foreach (var asset in configs) {
                        if (asset == config) continue;
                        assets.Remove(asset);
                    }
                }

                if (!configs.Contains(config)) assets.Add(config);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
                return config;
            }

            if (configs.Count > 0)
            {
                config = configs.First();
                CacheConfigPath(config);
                if (configs.Count > 1) {
                    foreach (var asset in configs.Skip(1)) assets.Remove(asset);
                    PlayerSettings.SetPreloadedAssets(assets.ToArray());
                }
                return config;
            }
            
            config = AssetDatabase.LoadAssetAtPath<TConfig>(defaultPath);
            if (config == null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Couldn't find asset!", MessageType.Warning);
                if (GUILayout.Button("Create New Config")) {
                    config = ScriptableObject.CreateInstance<TConfig>();
                    AssetDatabase.CreateAsset(config, defaultPath);
                }
                EditorGUILayout.EndVertical();
            }
            
            if (config != null) {
                assets.Add(config);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
                CacheConfigPath(config);
                return config;
            }

            return null;
        }

        private static void CacheConfigPath(TConfig config)
        {
            SessionState.SetString(CachedPathKey, GlobalObjectId.GetGlobalObjectIdSlow(config).ToString());
        }
    }
}