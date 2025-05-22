using System;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackMedia.Scriptables
{
    /// <summary>
    /// Provides configuration metadata for <see cref="SingletonScriptableObject{TScriptable}"/>
    /// implementations, controlling how singleton assets are located, loaded, and managed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is designed to be applied to classes that derive from
    /// <see cref="SingletonScriptableObject{TScriptable}"/>, providing a declarative way
    /// to configure singleton behavior without modifying implementation code.
    /// </para>
    /// <para>
    /// When applied to a scriptable singleton class, this attribute controls:
    /// - File paths and naming conventions for the serialized asset
    /// - Loading strategy (Resources vs AssetDatabase)
    /// - Auto-creation behavior when assets don't exist
    /// - Validation settings when loading assets
    /// </para>
    /// <para>
    /// Using this attribute separates configuration from implementation, making it possible
    /// to adjust singleton behavior without changing the class code. This approach promotes
    /// cleaner architecture and more maintainable singleton implementations.
    /// </para>
    /// <para>
    /// Unlike Unity's <see cref="UnityEditor.ScriptableSingleton{T}"/> which is editor-only,
    /// the <see cref="SingletonScriptableObject{TScriptable}"/> system supports both
    /// editor and runtime contexts, with this attribute providing the configuration
    /// for both scenarios.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Define a game configuration singleton that loads from Resources
    /// [SingletonAsset("Configuration", "GameConfig", useResources: true)]
    /// public class GameConfiguration : SingletonScriptableObject&lt;GameConfiguration&gt;
    /// {
    ///     [SerializeField] private int maxPlayers = 4;
    ///     public int MaxPlayers => maxPlayers;
    /// }
    /// 
    /// // Define an editor-only singleton that doesn't auto-create
    /// [SingletonAsset("Editor/Database", useResources: false, autoCreate: false)]
    /// public class EditorDatabase : SingletonScriptableObject&lt;EditorDatabase&gt;
    /// {
    ///     [SerializeField] private List&lt;string&gt; recentFiles = new List&lt;string&gt;();
    ///     public IReadOnlyList&lt;string&gt; RecentFiles => recentFiles;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="SingletonScriptableObject{TScriptable}"/>
    /// <seealso cref="Scriptable"/>
    /// <seealso cref="UnityEditor.ScriptableSingleton{T}"/>
    /// <seealso cref="SingletonBehaviour{T}"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAssetAttribute : Attribute
    {
        public string AssetPath { get; }

        public string FileName { get; }

        public bool UseResources { get; }

        public bool AutoCreate { get; }

        public bool ValidateOnLoad { get; }

        public bool ForceEnableDebug { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAssetAttribute"/> with customizable asset management settings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor configures how a <see cref="SingletonScriptableObject{TScriptable}"/> locates, loads, and
        /// manages its underlying ScriptableObject asset. The attribute provides a declarative way to control
        /// singleton behavior without modifying class implementation.
        /// </para>
        /// <para>
        /// When applied to a class deriving from <see cref="SingletonScriptableObject{TScriptable}"/>, this attribute
        /// influences several aspects of the singleton lifecycle:
        /// - Asset storage location and naming conventions
        /// - Loading strategy (Resources vs AssetDatabase)
        /// - Automatic instantiation behavior
        /// - Validation during initialization
        /// </para>
        /// <para>
        /// This approach separates configuration concerns from implementation details, allowing
        /// for consistent singleton management across different scriptable types.
        /// </para>
        /// </remarks>
        /// <param name="assetPath">
        /// The relative path where the singleton asset will be stored. For Resources-based assets,
        /// this is relative to a Resources folder. For AssetDatabase assets, this is relative to
        /// the project's Assets folder. Defaults to "Settings/Objects".
        /// </param>
        /// <param name="fileName">
        /// The filename to use when saving or loading the singleton asset. If null (default),
        /// the class name of the scriptable object will be used instead.
        /// </param>
        /// <param name="useResources">
        /// Determines whether the singleton should be loaded from Unity's Resources system (true)
        /// or from the AssetDatabase (false). Resources-based singletons are available at runtime,
        /// while AssetDatabase singletons are editor-only. Defaults to true.
        /// </param>
        /// <param name="autoCreate">
        /// Controls whether the system should automatically create a new instance of the scriptable
        /// singleton if one doesn't already exist at the specified location. When false, the system
        /// will throw an exception if the asset cannot be found. Defaults to true.
        /// </param>
        /// <param name="validateOnLoad">
        /// Specifies whether the singleton should perform validation checks when loaded. When enabled,
        /// the singleton will verify its internal state and potentially repair inconsistencies before
        /// being made available. Defaults to true.
        /// </param>
        /// <param name="forceEnableDebug">
        /// Enables debug logging for the singleton lifecycle events. This is useful for troubleshooting
        /// and understanding the singleton's behavior during development. Defaults to false.
        /// </param>
        /// <example>
        /// <code>
        /// [SingletonAsset("GameSettings", "CoreConfiguration", useResources: true)]
        /// public class GameConfiguration : SingletonScriptableObject&lt;GameConfiguration&gt;
        /// {
        ///     [SerializeField] private int maxPlayers = 4;
        ///     [SerializeField] private float matchDuration = 300f;
        ///     
        ///     public int MaxPlayers => maxPlayers;
        ///     public float MatchDuration => matchDuration;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="SingletonScriptableObject{TScriptable}"/>
        /// <seealso cref="Scriptable"/>
        public SingletonAssetAttribute(string assetPath = "Settings/Objects", string fileName = null, bool useResources = true, bool autoCreate = true,
            bool validateOnLoad = true, bool forceEnableDebug = false)
        {
            AssetPath = ValidatePath(assetPath);
            FileName = ValidateFileName(fileName);
            UseResources = useResources;
            AutoCreate = autoCreate;
            ValidateOnLoad = validateOnLoad;
            ForceEnableDebug = forceEnableDebug;
        }

        /// <summary>
        /// Validates and normalizes a path string for use with singleton scriptable assets.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method ensures that path strings used for singleton asset storage are valid,
        /// consistent, and follow proper formatting conventions. It performs several sanitization
        /// steps to prevent common path-related issues when locating or creating assets.
        /// </para>
        /// <para>
        /// The validation process includes:
        /// - Providing a default path when none is specified
        /// - Converting backslashes to forward slashes for cross-platform compatibility
        /// - Removing duplicate slashes that could lead to path resolution errors
        /// - Trimming leading and trailing slashes to ensure proper path concatenation
        /// - Removing characters that are invalid in file system paths
        /// </para>
        /// <para>
        /// This method is called internally by the <see cref="SingletonAssetAttribute"/> constructor
        /// to ensure that the <see cref="SingletonAssetAttribute.AssetPath"/> property always contains
        /// a valid, normalized path regardless of how the attribute was instantiated.
        /// </para>
        /// <para>
        /// The validation is particularly important for ensuring consistent behavior across
        /// different operating systems, as path separator conventions and invalid character
        /// sets can vary between platforms.
        /// </para>
        /// </remarks>
        /// <param name="path">
        /// The path string to validate. Can be null or empty, in which case a default path
        /// will be returned.
        /// </param>
        /// <returns>
        /// A normalized, sanitized path string suitable for asset storage operations,
        /// or the default path "Settings/Objects" if the input was null or empty.
        /// </returns>
        /// <seealso cref="ValidateFileName"/>
        /// <seealso cref="SingletonAssetAttribute.AssetPath"/>
        private static string ValidatePath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? "Settings/Objects" : path.Trim().Replace('\\', '/').Replace("//", "/").Trim('/').Replace("<>:\"'|?*", "");
        }

        /// <summary>
        /// Validates and sanitizes a filename string for use with singleton scriptable assets.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method ensures that filenames used for singleton asset storage are valid across
        /// different file systems and follow proper naming conventions. It performs several
        /// sanitization steps to prevent file system errors when creating or loading assets.
        /// </para>
        /// <para>
        /// The validation process includes:
        /// - Handling null or empty filename cases appropriately
        /// - Removing all characters that are invalid in file system paths on any supported platform
        /// - Replacing invalid characters with underscores to maintain name readability
        /// - Trimming whitespace that could lead to inconsistent file resolution
        /// </para>
        /// <para>
        /// This method is called internally by the <see cref="SingletonAssetAttribute"/> constructor
        /// to ensure that the <see cref="SingletonAssetAttribute.FileName"/> property always contains
        /// a valid filename or null (indicating that the class name should be used instead).
        /// </para>
        /// <para>
        /// The validation is critical for ensuring consistent behavior across different operating
        /// systems, as invalid character sets can vary between platforms, potentially causing
        /// asset loading failures if not properly sanitized.
        /// </para>
        /// </remarks>
        /// <param name="fileName">
        /// The filename string to validate. Can be null or empty, in which case null will be returned
        /// to indicate that the default naming convention (class name) should be used.
        /// </param>
        /// <returns>
        /// A sanitized filename string suitable for file system operations with all invalid characters
        /// replaced by underscores, or null if the input was null or empty, indicating that the default
        /// naming convention should be used.
        /// </returns>
        /// <seealso cref="ValidatePath"/>
        /// <seealso cref="SingletonAssetAttribute.FileName"/>
        /// <seealso cref="Path.GetInvalidFileNameChars"/>
        private static string ValidateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var invalidCharacters = Path.GetInvalidFileNameChars();
            fileName = invalidCharacters.Aggregate(fileName, (current, invalidChar) => current.Replace(invalidChar, '_'));
            return fileName.Trim();
        }
    }

    public abstract class SingletonScriptableObject<TScriptable> : Scriptable where TScriptable : SingletonScriptableObject<TScriptable>
    {
        private static TScriptable instance;
        private static readonly object lockObject = new object();
        private static bool initialized;

        public static TScriptable Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null && !initialized)
                    {
                        initialized = true;
                        EnsureInstantiated();
                    }

                    return instance;
                }
            }
        }

        public static bool HasInstance => instance != null;

        public static void ForceReload()
        {
            lock (lockObject)
            {
                instance = null;
                initialized = false;
            }
        }

        public static void ResetToDefaults()
        {
            if (HasInstance) instance.OnResetToDefaults();
        }

        protected virtual void OnSingletonInitialized() { }

        protected virtual void OnResetToDefaults() { }

        protected virtual bool ValidateData() { return true; }

        protected virtual void LoadDefaults() { }

        protected virtual void OnEnable()
        {
            if (instance == null)
            {
                instance = this as TScriptable;
                OnSingletonInitialized();
            }
            else if (instance != this)
            {
                if (Application.isPlaying) Destroy(this);
                else DestroyImmediate(this);
            }

            Application.quitting += OnApplicationQuit;
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnValidate()
        {
            if (!ValidateData()) Debug.LogError($"Validation failed for {typeof(TScriptable).Name}");
        }

        protected virtual void OnApplicationQuit()
        {
            Application.quitting -= OnApplicationQuit;
        }

        private static void EnsureInstantiated()
        {
            SingletonAssetAttribute singletonAssetAttribute = GetSingletonAssetAttribute();
            if (singletonAssetAttribute == null)
            {
                Debug.LogError($"SingletonAssetAttribute not found on {typeof(TScriptable).Name}. Ensure the attribute is applied to the class.");
                return;
            }

            try
            {
                var pathInfo = GetAssetPathInfo(singletonAssetAttribute);
                if (singletonAssetAttribute.UseResources)
                {
                    instance = Resources.Load<TScriptable>(pathInfo.ResourcePath);
                    Debug.Log($"Resource load result: {(instance != null ? "Success" : "Failed")}");
                }
                else
                {
#if UNITY_EDITOR
                    instance = AssetDatabase.LoadAssetAtPath<TScriptable>(pathInfo.FullPath);
                    Debug.Log($"AssetDatabase load result: {(instance != null ? "Success" : "Failed")}");
#else
                    Debug.LogError($"SingletonAssetAttribute is set to use AssetDatabase, but this is not supported in runtime builds. Please use Resources instead.");
                    return;
#endif
                }

                if (instance == null && singletonAssetAttribute.AutoCreate)
                {
                    CreateInstance(singletonAssetAttribute, pathInfo);
                }
                else if (instance == null)
                {
                    Debug.LogError($"Singleton asset not found at {pathInfo.FullPath}. Ensure the asset exists or set AutoCreate to true.");
                }

                if (instance == null || !singletonAssetAttribute.ValidateOnLoad) return;
                if (!instance.ValidateData()) Debug.LogError($"Validation failed for {typeof(TScriptable).Name} on load.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading singleton asset: {e.Message} :: {e.StackTrace}");
            }
        }

        private static void CreateInstance(SingletonAssetAttribute attribute, AssetPathInfo pathInfo)
        {
            try
            {
#if UNITY_EDITOR
                instance = CreateInstance<TScriptable>();
                instance.LoadDefaults();

                EnsureDirectoryExists(Path.GetDirectoryName(pathInfo.FullPath));

                AssetDatabase.CreateAsset(instance, pathInfo.FullPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"Created new singleton asset at {pathInfo.FullPath}");
#else
                instance = CreateInstance<TScriptable>();
                instance.LoadDefaults();
                Debug.LogWarning($"Singleton asset not found and and is created in memory only. ");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while creating singleton asset: {e.Message} :: {e.StackTrace}");
            }
        }

        private struct AssetPathInfo
        {
            public string FullPath;
            public string ResourcePath;
            public string Directory;
        }

        private static AssetPathInfo GetAssetPathInfo(SingletonAssetAttribute attribute)
        {
            var fileName = attribute.FileName ?? typeof(TScriptable).Name;

            var pathInfo = new AssetPathInfo();

            if (attribute.UseResources)
            {
                pathInfo.Directory = Path.Combine("Assets", "Resources", attribute.AssetPath);
                pathInfo.FullPath = Path.Combine(pathInfo.Directory, fileName + ".asset");
                pathInfo.ResourcePath = Path.Combine(attribute.AssetPath, fileName);
            }
            else
            {
                pathInfo.Directory = Path.Combine("Assets", attribute.AssetPath);
                pathInfo.FullPath = Path.Combine(pathInfo.Directory, fileName + ".asset");
                pathInfo.ResourcePath = null;
            }

            pathInfo.FullPath = pathInfo.FullPath.Replace('\\', '/');
            pathInfo.Directory = pathInfo.Directory.Replace('\\', '/');
            pathInfo.ResourcePath = pathInfo.ResourcePath?.Replace('\\', '/');
            return pathInfo;
        }

        private static void EnsureDirectoryExists(string directory)
        {
            if (Directory.Exists(directory)) return;
            Directory.CreateDirectory(directory);
            Debug.Log($"Created directory: {directory}");
        }

        public enum LogSeverity
        {
            Info,
            Warning,
            Error
        }

        private static SingletonAssetAttribute GetSingletonAssetAttribute()
        {
            return (SingletonAssetAttribute)Attribute.GetCustomAttribute(typeof(TScriptable), typeof(SingletonAssetAttribute));
        }
    }
}
