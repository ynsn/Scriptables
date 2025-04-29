using System;

namespace StackMedia.Scriptables
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ScriptableEventAttribute : Attribute
    {
        public string DisplayName { get; }

        public bool IsBuiltin { get; set; }

        public ScriptableEventAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
