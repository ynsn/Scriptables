using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace StackMedia.Scriptables.Editor.UI.Internal
{
    internal class FuzzyMenuWindow : EditorWindow
    {
        public ToolbarSearchField SearchField { get; private set; }

        public VisualElement PanelContainer { get; private set; }

        public event Action<FuzzyMenuWindow> OnEditorCreated = delegate { };

        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private readonly Queue<IEnumerator> coroutines = new Queue<IEnumerator>();
        
        public void ScheduleCoroutine(IEnumerator coroutine)
        {
            coroutines.Enqueue(coroutine);
        }
        
        private void Update()
        {
            if (!queue.TryDequeue(out Action task)) return;
            task.Invoke();
        }

        private void CreateGUI()
        {
            rootVisualElement.style.borderBottomWidth = 1;
            rootVisualElement.style.borderTopWidth = 1;
            rootVisualElement.style.borderLeftWidth = 1;
            rootVisualElement.style.borderRightWidth = 1;
            rootVisualElement.style.borderBottomColor = new Color(0, 0, 0, 0.4f);
            rootVisualElement.style.borderTopColor = new Color(0, 0, 0, 0.4f);
            rootVisualElement.style.borderLeftColor = new Color(0, 0, 0, 0.4f);
            rootVisualElement.style.borderRightColor = new Color(0, 0, 0, 0.4f);

            var margin10 = new StyleLength(4);

            SearchField = new ToolbarSearchField()
            {
                style =
                {
                    width = StyleKeyword.Auto,
                    marginLeft = margin10,
                    marginRight = margin10,
                    marginTop = margin10
                }
            };

            rootVisualElement.Add(SearchField);

            PanelContainer = new VisualElement()
            {
                name = "panel-container",
                style =
                {
                    flexGrow = 1
                }
            };

            rootVisualElement.Add(PanelContainer);
            OnEditorCreated(this);
        }
    }
}
