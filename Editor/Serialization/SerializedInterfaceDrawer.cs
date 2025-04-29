using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables.Editor
{
    internal static class PropertyEditorUtility
    {
        internal static void Show(Object obj)
        {
            Type propertyEditor = typeof(EditorWindow).Assembly.GetTypes()
                .FirstOrDefault(x => x.Name == "PropertyEditor");

            if (propertyEditor == null)
                return;

            MethodInfo openPropertyEditorMethod = propertyEditor.GetMethod("OpenPropertyEditor",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[]
                {
                    typeof(UnityEngine.Object),
                    typeof(bool)
                },
                null);

            openPropertyEditorMethod.Invoke(null,
                new object[]
                {
                    obj, true
                });
        }
    }

    [CustomPropertyDrawer(typeof(SerializedInterface<>))]
    [CustomPropertyDrawer(typeof(SerializedInterface<,>))]
    public class SerializedInterfaceDrawer : PropertyDrawer
    {
        private class InterfaceDropdown : AdvancedDropdown
        {
            private readonly Type interfaceType;
            private readonly SerializedProperty property;

            public InterfaceDropdown(AdvancedDropdownState state, Type interfaceType, SerializedProperty property) : base(state)
            {
                this.interfaceType = interfaceType;
                this.property = property;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem(GetTypeName(interfaceType));
                root.AddChild(new InterfaceDropdownItem("None"));
                root.AddChild(BuildAssetsItem());
                root.AddChild(BuildSceneItem());
                return root;
            }

            private AdvancedDropdownItem BuildAssetsItem()
            {
                var root = new AdvancedDropdownItem("Assets");
                var count = 0;

                foreach (var assetPath in InterfaceAssetCache.Instance.GetPaths(interfaceType))
                {
                    var item = new InterfaceDropdownItem(assetPath)
                    {
                        icon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D
                    };
                    root.AddChild(item);
                    count++;
                }

                root.enabled = count > 0;
                return root;
            }

            private AdvancedDropdownItem BuildSceneItem()
            {
                var root = new AdvancedDropdownItem("Scene");
                Scene scene = SceneManager.GetActiveScene();
                var rootGameObjects = scene.GetRootGameObjects();

                var count = 0;
                foreach (GameObject gameObject in rootGameObjects)
                {
                    var components = gameObject.GetComponentsInChildren(interfaceType, true);
                    foreach (Component component in components)
                    {
                        var item = new SceneRefDropdownItem(component)
                        {
                            icon = EditorGUIUtility.ObjectContent(component, interfaceType).image as Texture2D
                        };
                        root.AddChild(item);
                        count++;
                    }
                }

                root.enabled = count > 0;
                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (item is InterfaceDropdownItem dropdownItem)
                {
                    if (dropdownItem.AssetPath == "None")
                    {
                        property.objectReferenceValue = null;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    property.objectReferenceValue = AssetDatabase.LoadMainAssetAtPath(dropdownItem.AssetPath);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else if (item is SceneRefDropdownItem sceneRefItem)
                {
                    if (sceneRefItem.Object == null)
                    {
                        property.objectReferenceValue = null;
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    property.objectReferenceValue = sceneRefItem.Object;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            private string GetTypeName(Type type)
            {
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    var name = genericType.Name;
                    var components = genericType.Name.Split('`');
                    if (components.Length > 1) name = components[0];
                    return $"{name}<{string.Join(", ", type.GenericTypeArguments.Select(GetTypeName))}>";
                }

                return type.Name;
            }

            private class InterfaceDropdownItem : AdvancedDropdownItem
            {
                public string AssetPath { get; }

                public InterfaceDropdownItem(string path) : base(Path.GetFileNameWithoutExtension(path))
                {
                    AssetPath = path;
                }
            }

            private class SceneRefDropdownItem : AdvancedDropdownItem
            {
                public Object Object { get; }

                public SceneRefDropdownItem(Object obj) : base(obj.name)
                {
                    Object = obj;
                }
            }
        }

        private SerializedProperty valueProperty;
        private InterfaceDropdown dropdown;
        private Type objectType;
        private Type interfaceType;
        private Rect objectFieldRect;
        private int controlId;

        private bool HasKeyboardFocus => GUIUtility.keyboardControl == controlId;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);

            EditorGUI.BeginProperty(position, label, property);
            objectFieldRect = DrawLabel(position, label.text);
            DrawObjectField(objectFieldRect);
            EditorGUI.EndProperty();

            ProcessEvents(position, objectFieldRect, property);
        }

        private void Initialize(SerializedProperty property)
        {
            ResolveInterfaceArgs();
            valueProperty ??= property.FindPropertyRelative("value");
            dropdown ??= new InterfaceDropdown(new AdvancedDropdownState(), interfaceType, valueProperty);
        }


        private Rect DrawLabel(Rect position, string label)
        {
            controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            Rect rect = EditorGUI.PrefixLabel(position, controlId, new GUIContent(label));

            return new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        }

        private DropdownWindow drop;
        
        private void DrawObjectField(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var fieldText = valueProperty.objectReferenceValue?.ToString() ?? $"None ({interfaceType.PrettyName()})";
                Texture icon = valueProperty.objectReferenceValue != null
                    ? EditorGUIUtility.ObjectContent(valueProperty.objectReferenceValue, valueProperty.objectReferenceValue.GetType()).image
                    : null;

                EditorStyles.objectField.Draw(position, new GUIContent(fieldText, icon), position.Contains(Event.current.mousePosition), false,
                    false,
                    HasKeyboardFocus);

            }

            var buttonRect = new Rect(position);
            buttonRect.xMin = buttonRect.xMax - 20;
            buttonRect.xMax -= 1;
            buttonRect.yMin += 1;
            buttonRect.yMax -= 1;
            if (GUI.Button(buttonRect, string.Empty, "objectFieldButton"))
            {
                drop = ScriptableObject.CreateInstance<DropdownWindow>();
                var size = position.size;
                size.y += 250;
                drop.ShowAsDropDown(GUIUtility.GUIToScreenRect(position), size);
                // dropdown = new InterfaceDropdown(new AdvancedDropdownState(), interfaceType, valueProperty);
                //dropdown.Show(position);
            }
        }


        private void ProcessEvents(Rect position, Rect objectFieldPosition, SerializedProperty property)
        {
            if (objectFieldPosition.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        var isValid =
                            (from draggedObject in DragAndDrop.objectReferences where draggedObject != null select draggedObject.GetType()).Any(draggedType =>
                                draggedType.Implements(interfaceType));

                        if (isValid)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            Event.current.Use();
                        }

                        break;
                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();
                        if (DragAndDrop.objectReferences.Length == 1)
                        {
                            Object draggedObject = DragAndDrop.objectReferences[0];
                            if (draggedObject != null)
                            {
                                if (draggedObject.GetType().Implements(interfaceType))
                                {
                                    valueProperty.objectReferenceValue = draggedObject;
                                    property.serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }

                        break;
                    case EventType.MouseDown:

                        if (Event.current.button == 0)
                        {
                            GUIUtility.keyboardControl = controlId;

                            // Ping the object in the inspector
                            if (valueProperty.objectReferenceValue != null)
                            {
                                if (Event.current.clickCount == 2)
                                {
                                    if (GUIUtility.keyboardControl == controlId && valueProperty.objectReferenceValue != null)
                                    {
                                        // Open the object in the inspector
                                        Selection.activeObject = valueProperty.objectReferenceValue;
                                        return;
                                    }
                                }

                                EditorGUIUtility.PingObject(valueProperty.objectReferenceValue);
                                Helpers.RepaintActiveEditors();
                            }
                        }
                        else if (Event.current.button == 1)
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Clear"), false, () =>
                            {
                                valueProperty.objectReferenceValue = null;
                                property.serializedObject.ApplyModifiedProperties();
                            });
                            if (valueProperty.objectReferenceValue != null)
                            {
                                menu.AddItem(new GUIContent("Properties"), false, () => { PropertyEditorUtility.Show(valueProperty.objectReferenceValue); });
                            }

                            menu.ShowAsContext();
                            Event.current.Use();
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private void ResolveInterfaceArgs()
        {
            if (objectType != null && interfaceType != null) return;
            Type field = fieldInfo.FieldType;
            if (field.IsGenericType)
            {
                var genericArguments = field.GetGenericArguments();
                if (genericArguments.Length == 2)
                {
                    objectType = genericArguments[1];
                    interfaceType = genericArguments[0];
                }
                else if (genericArguments.Length == 1)
                {
                    objectType = typeof(Object);
                    interfaceType = genericArguments[0];
                }
            }
            else
            {
                objectType = typeof(Object);
                interfaceType = field;
            }
        }
    }
}
