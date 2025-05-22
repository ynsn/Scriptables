using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using StackMedia.Scriptables.Internal.EasyButtons;

namespace StackMedia.Scriptables.Editor.Internal.EasyButtons
{
    internal class ButtonWithoutParamsAsync : ButtonWithoutParams
    {
        public ButtonWithoutParamsAsync(MethodInfo method, ButtonAttribute buttonAttribute)
            : base(method, buttonAttribute)
        { }

        protected async override void InvokeMethod(IEnumerable<object> targets)
        {
            foreach (object obj in targets)
            {
                Task task = (Task)Method.Invoke(obj, null);
                await task;
            }
        }
    }
}
