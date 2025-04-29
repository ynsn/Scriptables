using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using StackMedia.Scriptables.Editor.Internal.EasyButtons.Utils;
using StackMedia.Scriptables.Internal.EasyButtons;
using UnityEditor;

namespace StackMedia.Scriptables.Editor.Internal.EasyButtons
{
    /// <summary>
    /// A class that holds information about a button and can draw it in the inspector.
    /// </summary>
    public abstract class Button
    {
        /// <summary> Display name of the button. </summary>
        public readonly string DisplayName;

        /// <summary> MethodInfo object the button is attached to. </summary>
        public readonly MethodInfo Method;

        private readonly ButtonSpacing _spacing;
        private readonly bool _disabled;

        protected Button(MethodInfo method, ButtonAttribute buttonAttribute)
        {
            DisplayName = string.IsNullOrEmpty(buttonAttribute.Name)
                ? ObjectNames.NicifyVariableName(method.Name)
                : buttonAttribute.Name;

            Method = method;

            _spacing = buttonAttribute.Spacing;

            bool inAppropriateMode = EditorApplication.isPlaying
                ? buttonAttribute.Mode == ButtonMode.EnabledInPlayMode
                : buttonAttribute.Mode == ButtonMode.DisabledInPlayMode;

            _disabled = !(buttonAttribute.Mode == ButtonMode.AlwaysEnabled || inAppropriateMode);
        }

        public void Draw(IEnumerable<object> targets)
        {
            using (new EditorGUI.DisabledScope(_disabled))
            {
                using (new DrawUtility.VerticalIndent(
                    _spacing.ContainsFlag(ButtonSpacing.Before),
                    _spacing.ContainsFlag(ButtonSpacing.After)))
                {
                    DrawInternal(targets);
                }
            }
        }

        internal static Button Create(MethodInfo method, ButtonAttribute buttonAttribute)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                if (method.ReturnType != typeof(Task))
                {
                    return new ButtonWithoutParams(method, buttonAttribute);
                }
                else
                {
                    return new ButtonWithoutParamsAsync(method, buttonAttribute);
                }
            }
            else
            {
                if (method.ReturnType != typeof(Task))
                {
                    return new ButtonWithParams(method, buttonAttribute, parameters);
                }
                else
                {
                    return new ButtonWithParamsAsync(method, buttonAttribute, parameters);
                }
            }
        }

        protected abstract void DrawInternal(IEnumerable<object> targets);
    }
}
