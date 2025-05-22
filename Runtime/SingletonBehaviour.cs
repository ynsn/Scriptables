using UnityEngine;

namespace StackMedia.Scriptables
{
    public interface ISingletonBehaviour<T>
    { }

    public abstract class SingletonBehaviour<T> : MonoBehaviour, ISingletonBehaviour<T>
        where T : Component, ISingletonBehaviour<T>
    {
        /// <summary>
        /// Holds the instance of the singleton or null.
        /// </summary>
        private static T instance;
        
        [SerializeField] private bool persistentAcrossScenes = true;

        /// <summary>
        /// Ensures that the instance of the singleton is created and returns it.
        /// </summary>
        /// <returns>The instance of the singleton.</returns>
        private static T EnsureInstanceObject()
        {
            instance = FindAnyObjectByType<T>();
            if (instance != null) return instance;

            var singletonGameObject = new GameObject($"{typeof(T).Name} [Auto-Generated]");
            instance = singletonGameObject.AddComponent<T>();
            return instance;
        }

        /// <summary>
        /// Gets whether an instance of the singleton has been created.
        /// </summary>
        public static bool HasInstance => instance != null;

        /// <summary>
        /// Tries to get the instance of the singleton via the out parameter, returning whether a valid instance was
        /// provided or not.
        /// </summary>
        /// <param name="outInstance">The receiver of the instance.</param>
        /// <returns>True if a valid instance was provided, false otherwise.</returns>
        /// <remarks>outInstance will be assigned null if no instance was provided.</remarks>
        public static bool TryGetInstance(out T outInstance) => (outInstance = HasInstance ? instance : null) != null;

        public static T Instance => instance != null ? instance : EnsureInstanceObject();

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;
            instance = this as T;
            if (persistentAcrossScenes) DontDestroyOnLoad(instance);
        }
    }
}
