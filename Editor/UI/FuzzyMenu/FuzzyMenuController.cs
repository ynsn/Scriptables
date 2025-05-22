using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StackMedia.Scriptables.Editor.UI.Internal;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace StackMedia.Scriptables.Editor.UI
{
    internal class FuzzySearch
    {

    }

    internal class FuzzyMenuController<T>
    {
        private FuzzyMenuWindow window;

        private FuzzyMenuPanelView<T>[] views;
        private FuzzyMenuPanelView<T> searchView;

        private int currentViewIndex;

        private FuzzyMenuItem<T> currentItem;

        private readonly Stack<FuzzyMenuItem<T>> itemStack = new Stack<FuzzyMenuItem<T>>();

        public FuzzyMenuPanelView<T> CurrentView => views[currentViewIndex];

        public FuzzyMenuPanelView<T> BufferedView => views[1 - currentViewIndex];

        public Action<T> ItemSelected { get; set; }

        public FuzzyMenuController(FuzzyMenuItem<T> rootItem) => currentItem = rootItem;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


        private List<FuzzyMenuItem<T>> Flatten(FuzzyMenuItem<T> root)
        {
            var result = new List<FuzzyMenuItem<T>>(128);
            var stack = new Stack<FuzzyMenuItem<T>>(128);
            stack.Push(root);

            while (stack.Count > 0)
            {
                var item = stack.Pop();

                if (item.Children.Count == 0) result.Add(item);
                for (var i = item.Children.Count - 1; i >= 0; i--) stack.Push(item.Children[i]);
            }

            return result;
        }

        private List<FuzzyMenuItem<T>> Search(FuzzyMenuItem<T> root, string searchTerm, int maxDistance = 3)
        {
            if (string.IsNullOrEmpty(searchTerm) || root == null || root.Children.Count == 0) return new List<FuzzyMenuItem<T>>();

            var flattenedItems = Flatten(root).Select(x => x)
                .Where(x => x.Name.ToLower().StartsWith(searchTerm.ToLower()) || x.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
            var res = flattenedItems.OrderBy(i => i.Name).ToList();

            return res;
        }

        public void OnEditorWindowCreated(FuzzyMenuWindow w)
        {
            window = w;

            views = new[] { new FuzzyMenuPanelView<T>(), new FuzzyMenuPanelView<T>() };
            window.PanelContainer.Add(views[0]);
            window.PanelContainer.Add(views[1]);
            views[1].style.translate = new Translate(Length.Percent(100), 0);

            searchView = new FuzzyMenuPanelView<T>();
            window.PanelContainer.Add(searchView);
            searchView.style.display = DisplayStyle.None;

            CurrentView.Initialize(currentItem);
            CurrentView.OnItemSelected += OnItemSelected;
            window.SearchField.Focus();
            window.SearchField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != string.Empty)
                {
                    CurrentView.style.display = DisplayStyle.None;
                    BufferedView.style.display = DisplayStyle.None;
                    searchView.style.display = DisplayStyle.Flex;

                    var newRoot = new FuzzyMenuItem<T>("Flattened");
                    newRoot.Children.AddRange(Search(currentItem, evt.newValue, 1).AsEnumerable());

                    searchView.Initialize(newRoot);
                    searchView.OnItemSelected += OnItemSelected;

                }
                else
                {
                    CurrentView.style.display = DisplayStyle.Flex;
                    BufferedView.style.display = DisplayStyle.Flex;
                    searchView.style.display = DisplayStyle.None;

                    searchView.OnItemSelected -= OnItemSelected;
                }
            });
        }

        private readonly Translate translateHundredPercent = new Translate(Length.Percent(100), 0);
        private readonly Translate translateMinHundredPercent = new Translate(Length.Percent(-100), 0);
        private readonly Translate translateZeroPercent = new Translate(Length.Percent(0), 0);

        private readonly StyleList<TimeValue> transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });

        private IEnumerator DoSomething()
        {
            yield return null;
        }

        internal void OnItemSelected(FuzzyMenuItem<T> selectedItem)
        {
            if (selectedItem.Children.Count > 0)
            {
                PrepareView(BufferedView, 1);
                itemStack.Push(currentItem);
                currentItem = selectedItem;

                BufferedView.Initialize(selectedItem);
                BufferedView.OnItemSelected += OnItemSelected;
                BufferedView.OnBack += OnNavigateBack;

                CurrentView.style.translate = translateMinHundredPercent;
                CurrentView.OnItemSelected -= OnItemSelected;
                CurrentView.OnBack -= OnNavigateBack;

                BufferedView.style.translate = translateZeroPercent;
                SwapViews();
            }
            else
            {
                ItemSelected?.Invoke(selectedItem.UserData);
                window.Close();
            }
        }

        private void OnNavigateBack()
        {
            if (itemStack.Count == 0) return;

            PrepareView(BufferedView, 0);
            currentItem = itemStack.Pop();

            BufferedView.Initialize(currentItem);
            BufferedView.OnItemSelected += OnItemSelected;
            BufferedView.OnBack += OnNavigateBack;

            CurrentView.style.translate = translateHundredPercent;
            CurrentView.OnItemSelected -= OnItemSelected;
            CurrentView.OnBack -= OnNavigateBack;

            BufferedView.style.translate = translateZeroPercent;
            SwapViews();
        }

        private void PrepareView(FuzzyMenuPanelView<T> panelView, int type)
        {
            Assert.IsTrue(type is 0 or 1, "Invalid view type. Must be 0 or 1.");

            panelView.style.transitionDuration = StyleKeyword.Initial;
            panelView.style.translate = type == 0 ? translateMinHundredPercent : translateHundredPercent;
            panelView.style.transitionDuration = transitionDuration;
        }

        private void SwapViews() => currentViewIndex = 1 - currentViewIndex;
    }
}
