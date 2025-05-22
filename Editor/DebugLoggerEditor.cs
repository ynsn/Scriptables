using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StackMedia.Scriptables.Editor
{
    [CustomEditor(typeof(DebugLogger))]
    public class DebugLoggerEditor : UnityEditor.Editor
    {
        
        
        public override bool UseDefaultMargins()
        {
            return false;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new Toolbar()
            {
                
            });
            
           
            return root;
        }
    }
}
