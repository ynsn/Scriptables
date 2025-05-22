using System;
using System.Collections.Generic;
using System.Reflection;
using StackMedia.Scriptables.Internal.EasyButtons;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = StackMedia.Scriptables.Editor.Internal.EasyButtons.Button;
using Object = UnityEngine.Object;

namespace StackMedia.Scriptables.Editor.UI
{
    [UxmlElement]
    public partial class SplitterSeparator : VisualElement
    {
        private Color color;

        public SplitterSeparator()
        {
            color = EditorGUIUtility.isProSkin ? new Color(0.12f, 0.12f, 0.12f, 1.333f) : new Color(0.6f, 0.6f, 0.6f, 1.333f);
            style.backgroundColor = color;
            style.width = Length.Percent(100f);
            style.height = 1f;
        }
    }

    [UxmlElement]
    public partial class InspectorHeader : VisualElement, INotifyValueChanged<bool>
    {
        private VisualElement header;
        private Image arrow;
        private Toggle toggle;
        private Label label;

        private VisualElement content;

        private Color headerBorderColor = new Color(0, 0, 0, 0.35f);
        private Texture2D arrowIcon;
        private Texture2D helpIcon;
        private Texture2D menuIcon;

        private Color backgroundColor;

        private SerializedProperty foldout;
        private SerializedProperty enabled;

        public InspectorHeader(GUIContent title, SerializedProperty foldoutProperty, SerializedProperty toggleProperty, Action onRemove) : this()
        {
            foldout = foldoutProperty;
            enabled = toggleProperty;
            val = foldoutProperty?.boolValue ?? false;

            arrowIcon =
                (EditorGUIUtility.isProSkin ? EditorGUIUtility.IconContent("d_forward@2x").image : EditorGUIUtility.IconContent("forward@2x").image) as Texture2D;
            arrowIcon!.filterMode = FilterMode.Bilinear;

            helpIcon = (EditorGUIUtility.isProSkin ? EditorGUIUtility.IconContent("d__Help@2x").image : EditorGUIUtility.IconContent("_Help@2x").image) as Texture2D;
            helpIcon!.filterMode = FilterMode.Bilinear;

            menuIcon = (EditorGUIUtility.isProSkin ? EditorGUIUtility.IconContent("d__Menu@2x").image : EditorGUIUtility.IconContent("_Menu").image) as Texture2D;
            menuIcon!.filterMode = FilterMode.Bilinear;

            backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f, 0.2f) : new Color(1f, 1f, 1f, 0.2f);

            header = new VisualElement()
            {
                name = "inspector-header",
                style =
                {
                    width = Length.Percent(100),
                    height = EditorGUIUtility.singleLineHeight + 2,
                    backgroundColor = backgroundColor,
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,

                    borderTopWidth = 0,
                    borderTopColor = headerBorderColor,
                    borderBottomWidth = 0,
                    borderBottomColor = headerBorderColor,

                    paddingLeft = 16,
                    paddingRight = 8
                }
            };

            header.RegisterCallback<ClickEvent>(ToggleFoldout);

            arrow = new Image()
            {
                image = arrowIcon,
                style =
                {
                    width = 12,
                    height = 12,
                    opacity = 0.5f,
                    transitionDuration = new StyleList<TimeValue>(new List<TimeValue>() { new TimeValue(0.1f) })
                }

            };

            header.Add(arrow);
            arrow.transform.rotation = Quaternion.Euler(0, 0, val ? 90 : 0);

            toggle = new Toggle()
            {
                value = enabled is { boolValue: true },
                pickingMode = PickingMode.Position,
                style =
                {
                    width = 14,
                    height = 14,
                    marginBottom = 0,
                    marginTop = 0
                }
            };

            toggle.RegisterCallback<ClickEvent>(evt => evt.StopImmediatePropagation());
            toggle.RegisterValueChangedCallback(evt =>
            {
                label.SetEnabled(evt.newValue);
                enabled.boolValue = evt.newValue;
                enabled.serializedObject.ApplyModifiedProperties();
            });
            header.Add(toggle);

            label = new Label("Some test component")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginLeft = 8,
                }
            };
            header.Add(label);
            label.SetEnabled(enabled?.boolValue ?? true);

            header.Add(new VisualElement()
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 0,
                }
            });

            var helpButton = new EditorToolbarDropdown()
            {
                iconImage = helpIcon,
                style =
                {
                    width = 20,
                    height = 20,
                    marginLeft = 0,
                    marginRight = 0,
                    paddingLeft = 0,
                    paddingRight = 0,
                    backgroundColor = Color.clear,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0,
                }
            };
            header.Add(helpButton);

            var menuButton = new EditorToolbarDropdown()
            {
                iconImage = menuIcon,
                style =
                {
                    width = 20,
                    height = 20,
                    marginLeft = 0,
                    marginRight = 0,
                    paddingLeft = 0,
                    paddingRight = 0,
                    backgroundColor = Color.clear,
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderRightWidth = 0,
                },

            };
            menuButton.RegisterCallback<ClickEvent>(evt => evt.StopImmediatePropagation());
            header.Add(menuButton);

            label.text = ObjectNames.NicifyVariableName(title.text);
            Add(new SplitterSeparator());
            Add(header);

            header.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("Remove", a => onRemove()); }));
        }

        public InspectorHeader()
        {
        }

        private void ToggleFoldout(ClickEvent evt)
        {
            value = !value;
            if (value)
            {
                // Open
                arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
                header.style.borderBottomColor = Color.clear;
            }
            else
            {
                // Close
                arrow.transform.rotation = Quaternion.Euler(0, 0, 0);
                header.style.borderBottomColor = headerBorderColor;
            }
        }

        public void SetValueWithoutNotify(bool newValue)
        {
            val = newValue;
            if (foldout == null) return;
            foldout.boolValue = newValue;
            foldout.serializedObject.ApplyModifiedProperties();
        }

        private bool val;

        public bool value
        {
            get => val;

            set
            {
                if (EqualityComparer<bool>.Default.Equals(val, value)) return;
                if (panel != null)
                {
                    using var evt = ChangeEvent<bool>.GetPooled(val, value);
                    evt.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(evt);
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }

    }

    [CustomEditor(typeof(Scriptable), true, isFallback = false)]
    public class ScriptableEditor : UnityEditor.Editor
    {
        private class ComponentMenu : AdvancedDropdown
        {
            public delegate void AddComponentAction(Type type);

            public event AddComponentAction OnAddComponent = delegate { };

            public ComponentMenu(AdvancedDropdownState state) : base(state)
            {

            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var types = TypeCache.GetTypesDerivedFrom<ScriptableBehaviour>();
                var root = new AdvancedDropdownItem("Components");
                foreach (var type in types)
                {
                    var item = new ComponentMenuItem(ObjectNames.NicifyVariableName(type.PrettyName()), type);

                    root.AddChild(item);
                }
                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (item is ComponentMenuItem componentItem)
                {
                    Type componentType = componentItem.Type;
                    OnAddComponent.Invoke(componentType);
                }
            }
        }

        private class ComponentMenuItem : AdvancedDropdownItem
        {
            public Type Type { get; private set; }

            public ComponentMenuItem(string name, Type type) : base(name)
            {
                Type = type;
            }
        }

        private SerializedProperty comment;

        private Toggle debugToggle;
        private Label headerLabel;
        private VisualElement propertyContainer;

        private VisualElement componentsListContainer;
        private List<InspectorElement> inspectorElements = new List<InspectorElement>();

        private UnityEngine.UIElements.Button addComponentButton;

        private List<MethodInfo> exposedMethods = new List<MethodInfo>();
        private List<Button> buttons = new List<Button>();

        private ComponentMenu menu;
        private AdvancedDropdownState menuState = new AdvancedDropdownState();

        private void OnEnable()
        {
            comment = serializedObject.FindProperty(nameof(Scriptable.comment));

            menu = new ComponentMenu(menuState);
            menu.OnAddComponent += AddComponent;

            // Get all methods with the ExposeMethod attribute
            var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(ExposeMethodAttribute), true);
                if (attributes.Length <= 0) continue;
                exposedMethods.Add(method);
                buttons.Add(Button.Create(method, new ButtonAttribute(method.Name)));
            }
        }

        public override bool HasPreviewGUI() => true;

        protected override bool ShouldHideOpenButton() => true;

        public override void DrawPreview(Rect previewArea)
        {
            foreach (Button button in buttons)
            {
                button?.Draw(targets);
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset visualTreeAsset = Helpers.GetEditorVisualTreeAsset("ScriptableEditor.uxml");
            TemplateContainer root = visualTreeAsset.CloneTree();
            var toolbarMenu = root.Q<ToolbarMenu>();
            toolbarMenu.menu.AppendAction("Clear Components", action =>
            {
                Clear();
                /*var allAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
                foreach (var asset in allAssets)
                {
                    if (AssetDatabase.IsMainAsset(asset)) continue;
                    AssetDatabase.RemoveObjectFromAsset(asset);
                    DestroyImmediate(asset);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                ((Scriptable)target).Clear();
                Apply();*/
            });

            /*toolbarMenu.menu.AppendAction("Reserialize from subassets", action =>
            {
                var allAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
                ((Scriptable)target).Clear();
                foreach (Object asset in allAssets)
                {
                    if (AssetDatabase.IsMainAsset(asset)) continue;

                    if (asset is ScriptableComponent typedComponent)
                    {
                        ((Scriptable)target).AddComponent(typedComponent);
                    }

                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Apply();
            });*/

            propertyContainer = root.Q<VisualElement>("property-container");

            /*SerializedProperty iter = serializedObject.GetIterator();
            while (iter.NextVisible(true))
            {
                if (iter.name is "debugEnabled" or "comment" or "m_Script") continue;

                var propertyField = new PropertyField(iter);
                propertyContainer.Add(propertyField);
            }*/

            componentsListContainer = new VisualElement();
            SerializedProperty componentsProperty = serializedObject.FindProperty("components");
            componentsListContainer.TrackPropertyValue(componentsProperty, property =>
            {
                for (var i = componentsListContainer.childCount - 1; i >= 0; i--)
                {
                    componentsListContainer.RemoveAt(i);
                }

                for (var i = 0; i < property.arraySize; i++)
                {
                    Object obj = property.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (obj is ScriptableObject scriptableObject)
                    {
                        var inspector = UnityEditor.Editor.CreateEditor(scriptableObject);
                        if (inspector == null) continue;

                        var serializedComponent = new SerializedObject(obj);
                        SerializedProperty foldoutProperty = serializedComponent.FindProperty("foldout");
                        SerializedProperty enabledProperty = serializedComponent.FindProperty("enabled");

                        var i1 = i;
                        var f = new InspectorHeader(new GUIContent(scriptableObject.name), foldoutProperty, enabledProperty, () =>
                        {
                            Undo.RecordObject(target, "Remove Behaviour");
                            RemoveComponent(i1);
                            Apply();
                        });

                        var inspectorElement = new InspectorElement(inspector)
                        {
                            style =
                            {
                                display = f.value ? DisplayStyle.Flex : DisplayStyle.None
                            }
                        };

                        f.RegisterCallback<ChangeEvent<bool>>(evt =>
                        {
                            if (evt.target == f)
                            {
                                inspectorElement.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                            }
                            //Debug.Log("From " + evt.previousValue + " to " + evt.newValue + " " + evt.target);
                        });

                        componentsListContainer.Add(f);
                        componentsListContainer.Add(inspectorElement);
                    }
                }
                componentsListContainer.Add(new SplitterSeparator());
            });
            var comp = (Scriptable)target;

            // Add inspector elements for each component
            for (var i = 0; i < comp.Components.Count; i++)
            {
                ScriptableComponent obj = comp.Components[i];
                if (obj is ScriptableObject scriptableObject)
                {
                    var inspector = CreateEditor(scriptableObject);
                    if (!inspector) continue;

                    var serializedComponent = new SerializedObject(obj);
                    SerializedProperty foldoutProperty = serializedComponent.FindProperty("foldout");
                    SerializedProperty enabledProperty = serializedComponent.FindProperty("enabled");

                    var i1 = i;
                    var f = new InspectorHeader(new GUIContent(scriptableObject.name), foldoutProperty, enabledProperty, () =>
                    {
                        Undo.RecordObject(target, "Remove Behaviour");
                        RemoveComponent(i1);
                        Apply();
                    });

                    var inspectorElement = new InspectorElement(inspector)
                    {
                        style =
                        {
                            display = f.value ? DisplayStyle.Flex : DisplayStyle.None
                        }
                    };

                    f.RegisterCallback<ChangeEvent<bool>>(evt =>
                    {
                        if (evt.target == f)
                        {
                            inspectorElement.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                        }
                        //Debug.Log("From " + evt.previousValue + " to " + evt.newValue + " " + evt.target);
                    });

                    componentsListContainer.Add(f);
                    componentsListContainer.Add(inspectorElement);
                }

                // inspectorElements.Add(inspector.);
                //componentsListContainer.Add(inspector);
            }

            componentsListContainer.Add(new SplitterSeparator());
            root.Add(componentsListContainer);

            var buttonContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.Center,
                    borderTopWidth = 0,
                    borderTopColor = Color.black,
                    paddingTop = 8
                }
            };


            addComponentButton = new UnityEngine.UIElements.Button(OnAddComponentButtonClicked)
            {
                text = "Add Component",
                style =
                {
                    width = 226,
                    height = 26
                }
            };

            buttonContainer.Add(addComponentButton);

            root.Add(buttonContainer);

            return root;
        }

        private void OnAddComponentButtonClicked()
        {
            menu.Show(addComponentButton.worldBound);
        }

        public override bool UseDefaultMargins() => false;

        private void Clear()
        {
            // Clear components editors
            for (var i = componentsListContainer.childCount - 1; i >= 0; i--)
                componentsListContainer.RemoveAt(i);

            // Clear components serialized list
            ((Scriptable)target).Clear();

            // Clear components subassets
            var allAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
            foreach (Object asset in allAssets)
            {
                if (AssetDatabase.IsMainAsset(asset)) continue;
                AssetDatabase.RemoveObjectFromAsset(asset);
                DestroyImmediate(asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Apply();
        }

        private void AddComponent(Type type)
        {
            ScriptableComponentContainer container = (Scriptable)target;
            Undo.RecordObject(target, "Add Behaviour");
            container.AddComponent(type);
            Apply();
        }

        private void RemoveComponent(int index)
        {
            ScriptableComponentContainer container = (Scriptable)target;
            Undo.RecordObject(target, "Remove Behaviour");
            container.DestroyComponentAt(index);
            Apply();
        }

        private void Apply()
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
