﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StackMedia.Scriptables.Editor.Internal.EasyButtons.Utils;
using StackMedia.Scriptables.Internal.EasyButtons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables.Editor.Internal.EasyButtons
{
    using Object = Object;

    internal class ButtonWithParams : Button
    {
        protected readonly Parameter[] _parameters;
        private bool _expanded;
        private string typeString;

        public ButtonWithParams(MethodInfo method, ButtonAttribute buttonAttribute, ParameterInfo[] parameters)
            : base(method, buttonAttribute)
        {
            _parameters = parameters.Select(paramInfo => new Parameter(paramInfo)).ToArray();
            _expanded = buttonAttribute.Expanded;
            
            typeString = $"{method.ReturnType.Name} {method.Name}({string.Join(", ", parameters.Select(p => p.ParameterType.Name))})";
        }

        protected override void DrawInternal(IEnumerable<object> targets)
        {
            if (GUILayout.Button(typeString))
            {
                InvokeMethod(targets);
                return;
            }
            
            foreach (Parameter param in _parameters)
            {
                param.Draw();
            }
            
            /*_expanded = DrawUtility.DrawInFoldout(foldoutRect, _expanded, DisplayName, () =>
            {
                foreach (Parameter param in _parameters)
                {
                    param.Draw();
                }
            });

            if ( ! GUI.Button(buttonRect, "Invoke"))
                return;

            InvokeMethod(targets);*/
        }

        protected virtual void InvokeMethod(IEnumerable<object> targets) {
            var paramValues = _parameters.Select(param => param.Value).ToArray();

            foreach (object obj in targets) {
                Method.Invoke(obj, paramValues);
            }
        }

        protected readonly struct Parameter
        {
            private readonly FieldInfo _fieldInfo;
            private readonly ScriptableObject _scriptableObj;
            private readonly NoScriptFieldEditor _editor;

            public Parameter(ParameterInfo paramInfo)
            {
                Type generatedType = ScriptableObjectCache.GetClass(paramInfo.Name, paramInfo.ParameterType, paramInfo.HasDefaultValue, paramInfo.DefaultValue);
                _scriptableObj = ScriptableObject.CreateInstance(generatedType);
                _fieldInfo = generatedType.GetField(paramInfo.Name);
                _editor = CreateEditor<NoScriptFieldEditor>(_scriptableObj);
            }

            public object Value
            {
                get
                {
                    // Every time modified properties are applied, the "No script asset for ..." warning appears.
                    // Saving only once before invoking the button minimizes those warnings.
                    _editor.ApplyModifiedProperties();
                    return _fieldInfo.GetValue(_scriptableObj);
                }
            }

            public void Draw()
            {
                _editor.OnInspectorGUI();
            }

            private static TEditor CreateEditor<TEditor>(Object obj)
                where TEditor : UnityEditor.Editor
            {
                return (TEditor) UnityEditor.Editor.CreateEditor(obj, typeof(TEditor));
            }
        }
    }
}