using System.Collections.Generic;
using System.Reflection;
using StackMedia.Scriptables.Internal.EasyButtons;
using UnityEditor;
using UnityEngine;

namespace StackMedia.Scriptables.Editor.Internal.EasyButtons
{
    

    internal class ButtonWithoutParams : Button
    {
        public ButtonWithoutParams(MethodInfo method, ButtonAttribute buttonAttribute)
            : base(method, buttonAttribute) { }

        protected override void DrawInternal(IEnumerable<object> targets)
        {
            if ( ! GUILayout.Button(DisplayName))
                return;

            InvokeMethod(targets);
        }

        protected virtual void InvokeMethod(IEnumerable<object> targets)
        {
            foreach (object obj in targets) {
                Method.Invoke(obj, null);
            }
        }
    }
}