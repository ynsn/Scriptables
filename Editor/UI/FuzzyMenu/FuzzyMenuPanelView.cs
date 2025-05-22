using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace StackMedia.Scriptables.Editor.UI.Internal
{
    internal class FuzzyMenuPanelHeading : VisualElement
    {
        public Image BackIcon { get; private set; }

        public Label Label { get; }

        public Image MiscIcon { get; private set; }

        public bool HasBack { get; }

        public event Action OnClicked = delegate { };

        public FuzzyMenuPanelHeading(string text)
        {
            style.width = StyleKeyword.Auto;
            style.height = 24;
            style.flexShrink = 0;
            style.justifyContent = Justify.Center;
            style.alignItems = Align.Center;
            style.borderBottomWidth = 1;
            style.borderBottomColor = new Color(0, 0, 0, 0.5f);

            BackIcon = new Image()
            {
                name = "back-icon",
                image = EditorGUIUtility.IconContent("d_tab_prev").image,
                style =
                {
                    position = Position.Absolute,
                    left = 4
                }
            };

            Label = new Label(text)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };

            RegisterCallback<ClickEvent>(evt => { OnClicked.Invoke(); });

            Add((BackIcon));
            Add(Label);
        }
    }

    internal class FuzzyMenuItemView<T> : VisualElement
    {
        private FuzzyMenuItem<T> data;

        private Action<FuzzyMenuItem<T>> clickHandler;

        public Image Icon { get; }

        public Label Label { get; }

        public FuzzyMenuItem<T> Data
        {
            get => data;

            set
            {
                Label.text = value.Name;
                data = value;
                Icon.image = data.Icon;
            }
        }


        public FuzzyMenuItemView(FuzzyMenuItem<T> data, Action<FuzzyMenuItem<T>> clickHandler)
        {
            this.data = data;
            this.clickHandler = clickHandler;

            RegisterCallback<ClickEvent>(_ => clickHandler.Invoke(this.data));

            style.justifyContent = Justify.FlexStart;
            style.alignItems = Align.Center;
            style.flexDirection = FlexDirection.Row;

            Icon = new Image()
            {
                name = "item-icon",
                image = Data?.Icon,
                style =
                {
                    width = 24,
                    height = 24,
                    flexShrink = 0
                }
            };

            Label = new Label(data?.Name)
            {
                style =
                {
                    width = StyleKeyword.Auto,
                    height = StyleKeyword.Auto,
                    marginLeft = 4,
                    marginRight = 4
                }
            };

            Add(Icon);
            Add(Label);
        }
    }

    internal class FuzzyMenuPanelView<T> : VisualElement
    {
        private FuzzyMenuItem<T> data;
        private bool preventInput;

        private readonly FuzzyMenuPanelHeading heading;
        private readonly ListView listView;
        private readonly VisualElement infoContainer;

        public IReadOnlyList<FuzzyMenuItem<T>> Items => data?.Children;

        public event Action<FuzzyMenuItem<T>> OnItemSelected = delegate { };

        public event Action OnBack = delegate { };

        public FuzzyMenuPanelView()
        {
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;

            style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName>() { "translate" });
            style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction>()
                { new EasingFunction(EasingMode.Linear) });
            style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue>()
                { new TimeValue(0.2f, TimeUnit.Second) });
            style.transitionDelay = new StyleList<TimeValue>(new List<TimeValue>()
                { new TimeValue(0.0f, TimeUnit.Second) });

            heading = new FuzzyMenuPanelHeading("Create Scriptable Event");
            heading.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopImmediatePropagation();
                if (preventInput) return;

                OnBack.Invoke();
            });
            listView = new ListView
            {
                style =
                {
                    flexGrow = 1
                },
                makeItem = () => new FuzzyMenuItemView<T>(null, (e) => { OnItemSelected.Invoke(e); }),
                bindItem = (element, i) =>
                {
                    if (element is not FuzzyMenuItemView<T> item) throw new NullReferenceException("Item is null");
                    item.Data = listView!.itemsSource[i] as FuzzyMenuItem<T> ?? throw new InvalidOperationException();
                },
                selectionType = SelectionType.Single
            };

            infoContainer = new VisualElement()
            {
                name = "info-container",
                style =
                {
                    width = StyleKeyword.Auto,
                    height = new StyleLength(new Length(64, LengthUnit.Pixel)),
                    flexShrink = 0,
                    borderTopWidth = 1,
                    borderTopColor = new Color(0, 0, 0, 0.5f),
                }
            };

            Add(heading);
            Add(listView);
            Add(infoContainer);
        }

        public void Initialize(FuzzyMenuItem<T> itemData)
        {
            data = itemData;
            if (data == null) throw new NullReferenceException("Data is null");

            //var m = data.Children.OrderBy(x => x.Children.Count == 0).ToList();
            listView.itemsSource = data.Children;
            heading.Label.text = data.Name;
            listView.RefreshItems();
        }
    }
}
