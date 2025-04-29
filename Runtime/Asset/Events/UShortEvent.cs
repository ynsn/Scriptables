using UnityEngine;

namespace StackMedia.Scriptables
{
    [CreateAssetMenu(menuName = "Scriptables/Events/UShort", fileName = "New UShort Event")]
    [ScriptableEvent("ushort", IsBuiltin = true)]
    public class UShortEvent : ScriptableEvent<ushort>
    {
    }
}
