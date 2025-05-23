using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StackMedia.Scriptables.Editor.UI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables.Editor
{
    [Serializable]
    [CustomPropertyDrawer(typeof(SerializedType))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        private class AdvancedTypeDropdown : AdvancedDropdown
        {
            private Action<Type> onItemSelected;
            private List<Type> types;

            public AdvancedTypeDropdown(AdvancedDropdownState state, List<Type> types, Action<Type> onItemSelected) : base(state)
            {
                this.onItemSelected = onItemSelected;
                this.types = types;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("Types");

                var noneItem = new TypeDropdownItem(null, "[None]");
                root.AddChild(noneItem);

                var tree = new Dictionary<string, AdvancedDropdownItem>();

                foreach (Type type in types.OrderBy(t => t.FullName))
                {
                    var path = type.Namespace ?? "Global";
                    AddTypeToNamespaceTree(root, tree, path, type);
                }

                return root;
            }

            private void AddTypeToNamespaceTree(AdvancedDropdownItem root, Dictionary<string, AdvancedDropdownItem> tree, string namespacePAth, Type type)
            {
                var parts = namespacePAth.Split('.');
                var currentPath = "";
                AdvancedDropdownItem currentParent = root;

                foreach (var part in parts)
                {
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : currentPath + "." + part;

                    if (!tree.TryGetValue(currentPath, out AdvancedDropdownItem item))
                    {
                        item = new AdvancedDropdownItem(part)
                        {

                        };
                        tree[currentPath] = item;
                        currentParent.AddChild(item);
                    }

                    currentParent = item;
                }

                var typeItem = new TypeDropdownItem(type, type.Name + $" ({type.Namespace})");
                currentParent.AddChild(typeItem);
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (item is TypeDropdownItem typeDropdownItem)
                {
                    onItemSelected?.Invoke(typeDropdownItem.Type);
                }
            }

            private class TypeDropdownItem : AdvancedDropdownItem
            {
                public Type Type { get; }

                public TypeDropdownItem(Type type, string name) : base(name)
                {
                    Type = type;

                    if (type == null)
                    {
                        icon = null;
                    }
                    else if (typeof(Component).IsAssignableFrom(type))
                    {
                        icon = EditorGUIUtility.FindTexture("cs Script Icon");
                    }
                }
            }
        }


        private AdvancedTypeDropdown dropdown;
        private SerializedProperty currentProperty;

        private FuzzyMenu<Type> fuzzyMenu;

        /*protected FuzzyMenuItem<Type> BuildRoot()
        {
            var root = new FuzzyMenuItem<Type>("Types");

            var noneItem = new FuzzyMenuItem<Type>("[None]");
            root.AddChild(noneItem);

            var tree = new Dictionary<string, FuzzyMenuItem<Type>>();
            var types = GetAvailableTypes();

            foreach (Type type in types.OrderBy(t => t.FullName))
            {
                var path = type.Namespace ?? "Global";
                AddTypeToNamespaceTree(root, tree, path, type);
            }

            return root;
        }

        private void AddTypeToNamespaceTree(FuzzyMenuItem<Type> root, Dictionary<string, FuzzyMenuItem<Type>> tree, string namespacePAth, Type type)
        {
            var parts = namespacePAth.Split('.');
            var currentPath = "";
            FuzzyMenuItem<Type> currentParent = root;

            foreach (var part in parts)
            {
                currentPath = string.IsNullOrEmpty(currentPath) ? part : currentPath + "." + part;

                if (!tree.TryGetValue(currentPath, out FuzzyMenuItem<Type> item))
                {
                    item = new FuzzyMenuItem<Type>(part)
                    {
                       
                    };
                    tree[currentPath] = item;
                    currentParent.AddChild(item);
                }

                currentParent = item;
            }

            var typeItem = new FuzzyMenuItem<Type>(type.Name + $" ({type.Namespace})");
            typeItem.UserData = type;
           // if (typeof(Object).IsAssignableFrom(type))
           //{
                typeItem.Icon = EditorGUIUtility.ObjectContent(null, type).image;
           // }
            currentParent.AddChild(typeItem);
        }*/
        
        protected FuzzyMenuItem<Type> BuildRoot()
        {
            var root = new FuzzyMenuItem<Type>("Types");

            var noneItem = new FuzzyMenuItem<Type>("[None]");
            root.AddChild(noneItem);

            var tree = new Dictionary<string, FuzzyMenuItem<Type>>();
            var types = GetAvailableTypes();

            foreach (Type type in types.OrderBy(t => t.FullName))
            {
                var path = type.Namespace ?? "Global";
                AddTypeToNamespaceTree(root, tree, path, type);
            }

            return root;
        }
        
        // Items are built lazily (children are created on demand, when PopulateChildren is called)
        private void AddTypeToNamespaceTree(FuzzyMenuItem<Type> root, Dictionary<string, FuzzyMenuItem<Type>> tree, string namespacePAth, Type type)
        {
            var parts = namespacePAth.Split('.');
            var currentPath = "";
            FuzzyMenuItem<Type> currentParent = root;

            foreach (var part in parts)
            {
                currentPath = string.IsNullOrEmpty(currentPath) ? part : currentPath + "." + part;

                if (!tree.TryGetValue(currentPath, out FuzzyMenuItem<Type> item))
                {
                    item = new FuzzyMenuItem<Type>(part);
                    tree[currentPath] = item;
                    currentParent.AddChild(item);
                }

                currentParent = item;
            }

            var typeItem = new FuzzyMenuItem<Type>(type.Name + $" ({type.Namespace})");
            typeItem.UserData = type;
            // if (typeof(Object).IsAssignableFrom(type))
            //{
            typeItem.Icon = EditorGUIUtility.ObjectContent(null, type).image;
            // }
            currentParent.AddChild(typeItem);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            currentProperty = property;

            var currentType = "";
            if (currentProperty != null)
            {
                PropertyInfo prop = property.boxedValue.GetType().GetProperty(nameof(SerializedType.FullName));
                currentType = prop!.GetValue(property.boxedValue)?.ToString();
            }

            Rect buttonRect = EditorGUI.PrefixLabel(position, label);

            if (EditorGUI.DropdownButton(buttonRect, new GUIContent(currentType), FocusType.Passive))
            {
                //dropdown = new AdvancedTypeDropdown(new AdvancedDropdownState(), GetAvailableTypes(), OnTypeSelected);
                //dropdown.Show(buttonRect);
                fuzzyMenu = new FuzzyMenu<Type>(BuildRoot());
                fuzzyMenu.OnItemSelected = OnTypeSelected;
                fuzzyMenu.Show(GUIUtility.GUIToScreenRect(buttonRect));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private void OnTypeSelected(Type type)
        {
            if (currentProperty == null) return;
            SerializedProperty prop = currentProperty.FindPropertyRelative("assemblyQualifiedTypeName");
            prop.stringValue = type == null ? "" : type.AssemblyQualifiedName;
            currentProperty.serializedObject.ApplyModifiedProperties();
        }

        private List<Type> GetAvailableTypes()
        {
            //  var ubst = InterfaceAssetCache.Instance;
            //  var derivedFromAttribute = (DerivedFromAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DerivedFromAttribute));
            //   var typePropertiesAttribute = (TypePropertiesAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TypePropertiesAttribute));

            //  var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            //      .SelectMany(assembly => assembly.GetTypes())
            //  .Where(derivedFromAttribute != null ? derivedFromAttribute.Predicate : (_) => true)
            //   .Where(typePropertiesAttribute != null ? typePropertiesAttribute.Predicate : (_) => true)
            //   .ToArray();

            // var types = assemblies;
            // .Where(type => type.IsPublic)
            //   .ToList();

            //return types/*.OrderBy(t => t.FullName)*/.ToList();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsPublic)
                .ToList();

            return types;
        }
    }
}
