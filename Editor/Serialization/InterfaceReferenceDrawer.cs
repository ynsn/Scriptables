using System;
using System.Collections.Generic;
using System.IO;
using StackMedia.Scriptables.Editor.UI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceReference<,>), true)]
    [CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        private class ReferenceMenu : AdvancedDropdown
        {
            private Type interfaceType;
            private SerializedProperty property;

            public ReferenceMenu(AdvancedDropdownState state, Type interfaceType, SerializedProperty property) : base(state)
            {
                this.interfaceType = interfaceType;
                this.property = property;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem(interfaceType.PrettyName());

                root.AddChild(new AdvancedDropdownItem("None"));

                List<ReferenceMenuAssetItem> assetItems = new List<ReferenceMenuAssetItem>();
                var assetsGroupItem = new AdvancedDropdownItem("Assets");
                foreach (var path in InterfaceAssetCache.instance.GetPaths(interfaceType))
                {
                    var assetName = Path.GetFileNameWithoutExtension(path);
                    var item = new ReferenceMenuAssetItem(assetName, path);
                    assetsGroupItem.AddChild(item);
                }
                root.AddChild(assetsGroupItem);
                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (item is ReferenceMenuAssetItem assetItem)
                {
                    property.objectReferenceValue = AssetDatabase.LoadMainAssetAtPath(assetItem.AssetPath);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    property.objectReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private class ReferenceMenuAssetItem : AdvancedDropdownItem
        {
            public string AssetPath;

            public ReferenceMenuAssetItem(string name, string path) : base(name)
            {
                icon = AssetDatabase.GetCachedIcon(path) as Texture2D;
                AssetPath = path;
            }
        }

        private Type interfaceType;
        private SerializedProperty valueProperty;

        private FuzzyMenu<string> searchMenu;
        private ReferenceMenu menu;
        private int controlId;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            interfaceType ??= fieldInfo.FieldType.GetGenericArguments()[0];
            valueProperty ??= property.FindPropertyRelative("value");


            EditorGUI.BeginProperty(position, label, valueProperty);
            {
                Rect objectFieldRect = DrawLabel(position, label.text);
                DrawObjectField(objectFieldRect);
            }
            EditorGUI.EndProperty();


            /*if (GUI.changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }*/
        }

        private Rect DrawLabel(Rect position, string label)
        {
            controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            Rect labelRect = EditorGUI.PrefixLabel(position, controlId, new GUIContent(label));
            return new Rect(labelRect.x, labelRect.y, labelRect.width, EditorGUIUtility.singleLineHeight);
        }

        private void DrawObjectField(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var text = valueProperty.objectReferenceValue != null
                    ? $"{valueProperty.objectReferenceValue?.name} ({interfaceType.PrettyName()})"
                    : $"None ({interfaceType.PrettyName()})";
                var icon = valueProperty.objectReferenceValue != null
                    ? EditorGUIUtility.ObjectContent(valueProperty.objectReferenceValue, valueProperty.objectReferenceValue.GetType()).image
                    : null;

                EditorStyles.objectField.Draw(position, new GUIContent(text, icon), position.Contains(Event.current.mousePosition), false, false,
                    GUIUtility.keyboardControl == controlId);
            }

            var buttonRect = new Rect(position)
            {
                xMin = position.xMax - 20,
                xMax = position.xMax - 1,
                yMin = position.yMin + 1,
                yMax = position.yMax - 1
            };

            if (GUI.Button(buttonRect, string.Empty, "objectFieldButton"))
            {
                /*searchMenu = new FuzzyMenu<string>(BuildRoot());
                searchMenu.OnItemSelected = (item) =>
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        valueProperty.objectReferenceValue = null;
                        valueProperty.serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    valueProperty.objectReferenceValue = AssetDatabase.LoadMainAssetAtPath(item);
                    valueProperty.serializedObject.ApplyModifiedProperties();
                };
                searchMenu.Show(GUIUtility.GUIToScreenRect(position));*/
                TypeDatabase.AddInterfaceCache(typeof(INotifier), typeof(VoidObserver));
                TypeDatabase.Save();
                menu = new ReferenceMenu(new AdvancedDropdownState(), interfaceType, valueProperty);
                menu.Show(position);
            }
        }

        private FuzzyMenuItem<string> BuildRoot()
        {
            var root = new FuzzyMenuItem<string>(interfaceType.Name);
            /*foreach (var assetPath in InterfaceAssetCache.Instance.GetPaths(interfaceType))
            {
                var item = new FuzzyMenuItem<Object>(assetPath)
                {
                    Icon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D
                };
                root.AddChild(item);
            }*/
            

            var noneItem = root.AddChild("None");
            noneItem.UserData = null;

            var assetsGroupItem = root.AddChild("Assets");
            assetsGroupItem.Icon = EditorGUIUtility.IconContent("d_Project").image;

            foreach (var path in InterfaceAssetCache.instance.GetPaths(interfaceType))
            {
                var assetName = Path.GetFileNameWithoutExtension(path);

                var item = new FuzzyMenuItem<string>(assetName)
                {
                    Icon = AssetDatabase.GetCachedIcon(path) as Texture2D,
                    UserData = path
                };
                assetsGroupItem.AddChild(item);
            }

            var sceneGroupItem = root.AddChild("Scene");
            sceneGroupItem.Icon = EditorGUIUtility.IconContent("d_Scene@2x").image;


            var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var sceneObject in sceneObjects)
            {
                var components = sceneObject.GetComponentsInChildren(interfaceType);
                foreach (Object component in components)
                {
                    var item = new FuzzyMenuItem<string>(component.name)
                    {
                        Icon = EditorGUIUtility.ObjectContent(component, component.GetType()).image,
                        UserData = ""
                    };
                    sceneGroupItem.AddChild(item);
                }
            }

            return root;
        }
    }
}
